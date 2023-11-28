using System.Net;
using System.Text.Json;
using AspClass.Db;
using Microsoft.AspNetCore.Mvc;

namespace NorthwindAppMvc.Controllers
{
    public class ChatController : Controller
    {
        [HttpGet()]
        public HttpResponseMessage GetMessageRes() {
            HttpResponseMessage resMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            resMessage.Content = new StringContent("{\"Message\"}");
            return resMessage;            
        }

        [HttpGet()]
        public IActionResult GetMessage()
        {
            try
            {
                var person = new Person(){
                    Name="John"
                    , Age = 30
                    , Car = "Mercedes"
                };

                var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

                var personAsJson = JsonSerializer.Serialize(person, options);

                return Ok(personAsJson);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}") ;
            }
        }
    }
}
