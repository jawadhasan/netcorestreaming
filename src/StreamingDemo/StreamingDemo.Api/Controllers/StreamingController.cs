using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingController : ControllerBase
    {

        public class FileLine
        {
            public string LineNumber { get; set; }
            public string Contet { get; set; }
        }

        [HttpPost]
        public async Task StreamRespose(IFormFile csvFile)
        {
            try
            {
                //Validation here...todo             

                //Parse file to a structure
                var lines = new List<FileLine>();
                var total = lines.Count; //count shall be >0 for processing


                float progress = 0;
                Response.ContentType = "application/json";
                await HttpContext.Response.StartAsync(default).ConfigureAwait(true);
                var bodyStream = HttpContext.Response.BodyWriter.AsStream(true);

                foreach(var line in lines)
                {
                    progress += 100 / (float)total;
                    var responseChunk = "todo"; //process the line and return the result of each processing into response chunk
                    var serializedRecord = JsonConvert.SerializeObject(new { Value = responseChunk, progress }); //seralize it to json
                    var bytes = Encoding.Default.GetBytes(serializedRecord);
                    var delimiterBytes = Encoding.Default.GetBytes("$$");

                    await bodyStream.WriteAsync(bytes.AsMemory(0, bytes.Length), default).ConfigureAwait(true);
                    await bodyStream.WriteAsync(delimiterBytes.AsMemory(0, delimiterBytes.Length), default).ConfigureAwait(true);
                    await bodyStream.FlushAsync(default(CancellationToken)).ConfigureAwait(true);

                }
                bodyStream.Close();
                await Response.CompleteAsync().ConfigureAwait(false);
                //return Ok();
            }
            catch (Exception ex)
            {
                //throw new ApplicationException
                await Task.FromResult(ex.Message);
            }
           
        }
    }
}
