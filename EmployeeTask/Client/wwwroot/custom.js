window.addEventListener("online", () => {
    console.log("hello budy");
    DotNet.invokeMethodAsync("EmployeeTask.Client", "HandleOnlineEvent");
});


function scrollToBottom() {
    var chatSection = document.getElementsByClassName("chat-messages")[0];
    chatSection.scrollTop = chatSection.scrollHeight;
}