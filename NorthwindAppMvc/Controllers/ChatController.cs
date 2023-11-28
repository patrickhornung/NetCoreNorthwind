using System.Net;
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
                return Ok("Hello WORLD");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "{\"error\": \"" + ex.Message + "\"}") ;
            }
        }
    }
}
