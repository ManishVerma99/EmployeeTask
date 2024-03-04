using System.Net.Http;
using System.Net.Http.Headers;

namespace EmployeeTask.Client.Services
{
    public class SavedToken
    {
        public IEnumerable<Claim> Claims { get; set; }
        public LogInUserDetailViewModel SavedUser { get; set; } = new LogInUserDetailViewModel();
    }
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider, ITaskAuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authState = new ClaimsPrincipal(new ClaimsIdentity());
            SavedToken savedToken = await GetTokenAsync();

            if (string.IsNullOrWhiteSpace(savedToken.SavedUser.Token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            else
            {
                var claims = JwtParserHelper.ParseClaimsFromJwt(savedToken.SavedUser.Token);
                savedToken.SavedUser.UserFullName = claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
                authState = await SetUserClaims(savedToken);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authState)));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken.SavedUser.Token);
                var timeSpan = DateTime.Parse(claims.Where(x => x.Type == ClaimTypes.Expiration).Select(x => x.Value).FirstOrDefault()) - DateTime.UtcNow;
                Timer refreshTimer = new Timer(async (_) =>
                {
                    await MarkUserAsLoggedOut();
                }, null, timeSpan, Timeout.InfiniteTimeSpan);

            }
            return new AuthenticationState(authState);
        }
        public async Task MarkUserAsAuthenticated(LogInUserDetailViewModel user)
        {
            SavedToken st = await ParseToken(user);
            await MarkUserAsAuthenticated(st);
        }
        private async Task MarkUserAsAuthenticated(SavedToken savedToken)
        {
            var authState = Task.FromResult(new AuthenticationState(await SetUserClaims(savedToken)));
            await _localStorage.SetItemAsync("token", savedToken.SavedUser.Token);
            await _localStorage.SetItemAsync("tokenExpire", savedToken.SavedUser.ExpirationDate);
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("tokenExpire");
            await _localStorage.ClearAsync();
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<SavedToken> GetTokenAsync()
        {
            var savedToken = await _localStorage.GetItemAsStringAsync("token");
            var expireDate = await _localStorage.GetItemAsync<DateTime>("tokenExpire");
            var user = await _localStorage.GetItemAsync<LogInUserDetailViewModel>("user");
            if (user != null)
            {
                return await ParseToken(user);
            }
            else
            {
                return await ParseToken(new LogInUserDetailViewModel()
                {
                    Token = savedToken,
                    ExpirationDate = expireDate
                });
            }
        }

        private async Task<SavedToken> ParseToken(LogInUserDetailViewModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Token))
            {
                return new SavedToken();
            }
            var tokenExpired = IsTokenExpired(user.ExpirationDate);
            if (tokenExpired)
            {
                await MarkUserAsLoggedOut();
                return new SavedToken();
            }
            var claims = JwtParserHelper.ParseClaimsFromJwt(user.Token);
            await _localStorage.SetItemAsync("user", user);
            string userId = claims.Where(x => x.Type == "UserId").Select(x => x.Value).FirstOrDefault();
            string userFullName = claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
            return new SavedToken()
            {
                Claims = claims,
                SavedUser = new LogInUserDetailViewModel()
                {
                    UserId = userId,
                    Token = user.Token,
                    ExpirationDate = user.ExpirationDate,
                    UserFullName = userFullName,
                    RoleNames = user.RoleNames,
                }
            };
        }

        private bool IsTokenExpired(DateTime expireDate)
        {
            return expireDate < DateTime.UtcNow;
        }

        private async Task<ClaimsPrincipal> SetUserClaims(SavedToken savedToken)
        {
            //create a claims
            var claimUserName = new Claim(ClaimTypes.Name, savedToken.SavedUser.UserFullName);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(savedToken.SavedUser.UserId));
            var claimMembers = new Claim(ClaimTypes.Expiration, savedToken.SavedUser.ExpirationDate.ToString());
            var currentRoles = new List<string>();
            currentRoles.Add(savedToken.SavedUser.RoleNames);
            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimUserName, claimNameIdentifier, claimMembers }, "apiauth");

            foreach (var role in currentRoles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                
            }
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, savedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value));
            //create claimsPrincipal
            var authenticatedUser = new ClaimsPrincipal(claimsIdentity);
            return authenticatedUser;

        }
    }

}
