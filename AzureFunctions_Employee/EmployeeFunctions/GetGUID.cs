using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionV3Learning.EmployeeFunctions
{
    public static class GetGUID
    {
        [FunctionName("GetGUID")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Calling GetGUID Function.");
            string responseMessage = Guid.NewGuid().ToString();
            return new OkObjectResult(responseMessage);
        }
    }
}
