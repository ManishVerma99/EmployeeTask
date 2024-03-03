window.addEventListener("online", () => {
    console.log("hello budy");
    DotNet.invokeMethodAsync("EmployeeTask.Client", "HandleOnlineEvent");
});