using System.Net;
using System.Text.Json;
using AspClass.Db;
using Microsoft.AspNetCore.Mvc;
using AspClass.Db.SqlServer.Models;
using System.Net.NetworkInformation;

namespace NorthwindAppMvc.Controllers
{
    public class ChatController : Controller
    {
        private readonly NorthwindContext _db;

        public ChatController(NorthwindContext context)
        {
            _db = context;
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
                Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                Response.Headers.Add("Access-Control-Allow-Methods", "GET");
                Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with, Content-Type, origin, authorization, accept, client-security-token");

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
                Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                Response.Headers.Add("Access-Control-Allow-Methods", "GET");
                Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with, Content-Type, origin, authorization, accept, client-security-token");

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
            return channel;
        }

                [HttpPost()]
        public IActionResult SendMessage([FromBody]ChatMessageDto pMessage)
        {
            try
            {

                        if (Request.Method.Equals("OPTIONS")) {
                            Response.Headers.Add("content-type", "application/json");
                            Response.StatusCode = 200;
                        }
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                Response.Headers.Add("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, OPTIONS");
                Response.Headers.Add("Access-Control-Max-Age", "1000");
                Response.Headers.Add("Access-Control-Allow-Headers", "x-requested-with, Content-Type, authorization");

                var chatMessage = CreateChatMessage(pMessage);
                var chatMessagesAsJson = JsonSerializer.Serialize(chatMessage, options);

                return Ok(chatMessagesAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}");
            }
        }

        private ChatMessage CreateChatMessage(ChatMessageDto pMessageDto){

var lastMessage = GetLastMessage();
int nextId = lastMessage!= null ? lastMessage.Id + 1 : 1;

            var message = new ChatMessage(){
                Id = nextId
                , IdChatChannel = 1
                , IsDeleted = false
                , Message = pMessageDto.Message
                , SendDate = DateTime.Now
            };

            _db.ChatMessages.Add(message);
            _db.SaveChanges();

            return message;
        }

        private ChatMessage GetLastMessage(){
            var message = _db.ChatMessages.OrderByDescending(i => i.Id).FirstOrDefault();
            return message;
        }
    }
}
