using Microsoft.AspNetCore.SignalR;

namespace NorthwindAppMvc
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            // https://localhost:7268/Home/chat
        }

        public async Task SendNewUser(string userName)
        {
            await Clients.All.SendAsync("NewUser", userName);
        }
    }
}
