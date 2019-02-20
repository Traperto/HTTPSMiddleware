using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HTTPSMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly string[] ALLOWED_METHODS = {"GET", "DELETE", "POST", "PUT", "PATCH"};
        private readonly string[] METHODS_NEEDING_PAYLOAD = {"POST", "PUT", "PATCH"};

        public class RequestData
        {
            [Required] public string Url { get; set; }
            [Required] public string Method { get; set; }
            public object Payload { get; set; }
            public string[][] Header { get; set; }
        }

        [HttpPost("send")]
        public async Task<ActionResult> Send([FromBody] RequestData value)
        {
            var client = new HttpClient();
            HttpResponseMessage res;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!ALLOWED_METHODS.Contains(value.Method))
            {
                return BadRequest($"{value.Method} is not a valid HTTP-method.");
            }

            if (METHODS_NEEDING_PAYLOAD.Contains(value.Method) && value.Payload == null)
            {
                return BadRequest($"Method {value.Method} needs to have a payload but doesn't.");
            }

            try
            {
                AppendHeaderToClient(client, value.Header);
            }
            catch (Exception)
            {
                return BadRequest("Invalid Headers given.");
            }

            switch (value.Method)
            {
                case "GET":
                    res = await client.GetAsync(value.Url);
                    break;
                case "POST":
                    res = await client.PostAsync(value.Url,
                        new StringContent(value.Payload.ToString(), Encoding.UTF8, "application/json"));
                    break;
                case "PUT":
                    res = await client.PutAsync(value.Url,
                        new StringContent(value.Payload.ToString(), Encoding.UTF8, "application/json"));
                    break;
                case "PATCH":
                    res = await client.PatchAsync(value.Url,
                        new StringContent(value.Payload.ToString(), Encoding.UTF8, "application/json"));
                    break;
                case "DELETE":
                    res = await client.DeleteAsync(value.Url);
                    break;
                default:
                    throw new Exception($"Method {value.Method} is not implemented but allowed");
            }

            return Ok(await res.Content.ReadAsStringAsync());
        }


        private static void AppendHeaderToClient(HttpClient client, string[][] headers = null)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                if (header.Length != 2)
                {
                    throw new Exception();
                }

                client.DefaultRequestHeaders.Add(header[0], header[1]);
            }
        }
    }
}