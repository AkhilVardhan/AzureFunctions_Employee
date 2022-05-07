using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctionV3Learning.Models;
using AzureFunctionV3Learning.Common;
using Microsoft.Azure.Cosmos;

namespace AzureFunctionV3Learning.EmployeeFunctions
{
    public static class EmployeeInsert
    {
        private static object objEmployeeDetails;

        [FunctionName("EmployeeInsert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "EmployeeInsert")] HttpRequest req,
            ILogger log)
        {
            // we are initialised required variables 
            string requestBody = null;
            Employee objEmployeeDetails = null;
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Employee> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- EmployeeInsert");

            // we are reading or parsing the input request body
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objEmployeeDetails = JsonConvert.DeserializeObject<Employee>(requestBody);

            if (objEmployeeDetails != null)
            {
                // initialising Azure Cosomosdb database connection.
                AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();

                // saving new student information.
                // System need to generate one unique value for "id" property while saving new items in container
                objEmployeeDetails.EmployeeGuid = Guid.NewGuid().ToString();

                objInsertResponse = await objCosmosDBActivitiy.SaveNewEmployeeItem(objEmployeeDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while inserting information of student- {objEmployeeDetails.Name}, {objEmployeeDetails.PhoneNumber}";
                    log.LogInformation(objResponse.Message + "Error:" + objInsertResponse.StatusCode);
                }
                else
                {
                    objResponse.ErrorCode = 0;
                    objResponse.Message = "Successfully inserted information.";
                }

            }
            else
            {
                objResponse.ErrorCode = 100;
                objResponse.Message = "Failed to read or extract Student information from Request body due to bad data.";
                log.LogInformation("Failed to read or extract Student information from Request body due to bad data.");
            }
            return new OkObjectResult(objResponse);
        }
    }
}
