using System.Net;
using System.Text.Json;
using AspClass.Db;
using Microsoft.AspNetCore.Mvc;
using AspClass.Db.SqlServer.Models;
using System.Net.NetworkInformation;
using System.Net.PeerToPeer;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.Intrinsics.X86;

namespace NorthwindAppMvc.Controllers
{
    public class ChatController : Controller
    {
        private readonly NorthwindContext _db;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(NorthwindContext context, IHubContext<ChatHub> hub)
        {
            _db = context;
            _hub = hub;
        }

        [HttpGet()]
        public HttpResponseMessage GetMessageRes()
        {
            HttpResponseMessage resMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resMessage.Content = new StringContent("{\"Message\"}");
            return resMessage;
        }

        [HttpGet()]
        public IActionResult GetUsers()
        {
            try
            {


                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                // Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                // Response.Headers.Add("Access-Control-Allow-Methods", "GET");
                // Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with, Content-Type, origin, authorization, accept, client-security-token");

                var chatUsers = GetChatUsers();
                var chatUsersAsJson = JsonSerializer.Serialize(chatUsers, options);

                return Ok(chatUsersAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }

        private ICollection<ChatUser> GetChatUsers()
        {
            var chatUsers = _db.ChatUsers.ToList();

            return chatUsers;
        }

        [HttpGet()]
        public IActionResult GetMessages([FromQuery(Name = "idSender")] int pIdSender, [FromQuery(Name = "idReceiver")] int pIdReceiver)
        {
            try
            {


                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                // Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                // Response.Headers.Add("Access-Control-Allow-Methods", "GET");
                // Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with, Content-Type, origin, authorization, accept, client-security-token");

                var chatMessages = GetChatMessages(pIdSender, pIdReceiver);
                var chatMessagesAsJson = JsonSerializer.Serialize(chatMessages, options);

                return Ok(chatMessagesAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }

        private ICollection<ChatMessage> GetChatMessages(int pIdSender, int pIdReceiver)
        {

            var channelAsSender = GetChatChannel(pIdSender, pIdReceiver);
            var channelAsReceiver = GetChatChannel(pIdReceiver, pIdSender);

            var chatMessages = _db.ChatMessages.Where(i => i.IdChatChannel == channelAsSender.Id || i.IdChatChannel == channelAsReceiver.Id).OrderBy(i => i.SendDate).ToList();

            return chatMessages;

        }

        private ChatChannel GetChatChannel(int pIdSender, int pIdReceiver)
        {
            var channel = _db.ChatChannels.FirstOrDefault(i => i.IdChatUserSender == pIdSender && i.IdChatUserReceiver == pIdReceiver);

            if (channel == null)
            {
                var lastChannel = _db.ChatChannels.OrderByDescending(i => i.Id).FirstOrDefault();
                var lastId = lastChannel == null ? 1 : lastChannel.Id + 1;
                channel = new ChatChannel() { Id = lastId, IdChatUserReceiver = pIdReceiver, IdChatUserSender = pIdSender };

                _db.ChatChannels.Add(channel);
                _db.SaveChanges();
            }

            return channel;
        }

        [HttpPost()]
        public IActionResult SendMessage([FromBody] ChatMessageDto pMessageDto)
        {
            try
            {


                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var chatMessage = CreateChatMessage(pMessageDto);
                var chatMessagesAsJson = JsonSerializer.Serialize(chatMessage, options);

                return Ok(chatMessagesAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }

        private ChatMessage CreateChatMessage(ChatMessageDto pChatMessage)
        {

            var lastMessage = GetLastMessage();

            var channel = this.GetChatChannel(pChatMessage.IdSender, pChatMessage.IdReceiver);

            int nextId = lastMessage != null ? lastMessage.Id + 1 : 1;

            var message = new ChatMessage()
            {
                Id = nextId
                ,
                IdChatChannel = channel.Id
                ,
                IsDeleted = false
                ,
                Message = pChatMessage.Message
                ,
                SendDate = DateTime.Now
            };

            _db.ChatMessages.Add(message);
            _db.SaveChanges();

            var receiver = _db.ChatUsers.FirstOrDefault(i => i.Id == pChatMessage.IdReceiver);
            var sender = _db.ChatUsers.FirstOrDefault(i => i.Id == pChatMessage.IdSender);

            if (receiver != null && sender != null)
            {
                this._hub.Clients.All.SendAsync("sendMessage", sender.Name, receiver.Name, pChatMessage.Message);
            }

            return message;
        }

        private ChatMessage GetLastMessage()
        {
            var message = _db.ChatMessages.OrderByDescending(i => i.Id).FirstOrDefault();
            return message;
        }

        [HttpPost()]
        public IActionResult LoginUser([FromBody] UserDto pUserDto)
        {
            try
            {
                var users = GetChatUsers();
                var user = users.FirstOrDefault(i => i.Name.Equals(pUserDto.Name));

                if (user == null)
                {
                    user = this.CreateChatUser(pUserDto.Name);
                }
                else
                {
                    user.CurrentStatus = "online";
                    _db.SaveChanges();
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var userAsJson = JsonSerializer.Serialize(user, options);

                this._hub.Clients.All.SendAsync("newUser", pUserDto.Name);

                return Ok(userAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }

        private ChatUser CreateChatUser(string pName)
        {

            var lastUser = _db.ChatUsers.OrderByDescending(i => i.Id).FirstOrDefault();
            var nextId = lastUser == null ? 1 : lastUser.Id + 1;

            var newUser = new ChatUser() { Id = nextId, Name = pName, CurrentStatus = "online" };
            _db.ChatUsers.Add(newUser);
            _db.SaveChanges();

            return newUser;

        }

        public IActionResult LogoutUser([FromBody] UserDto pUserDto)
        {
            try
            {
                var user = _db.ChatUsers.FirstOrDefault(i => i.Name.Equals(pUserDto.Name));
                user.CurrentStatus = "offline";
                _db.SaveChanges();

                this._hub.Clients.All.SendAsync("logoutUser", pUserDto.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }
    }
}
