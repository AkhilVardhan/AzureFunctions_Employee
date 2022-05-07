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
    public static class EmployeeUpdate
    {
        private static object objCosmosDBActivitiy;

        [FunctionName("EmployeeUpdate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EmployeeUpdate/{EmployeeGuid}")] HttpRequest req,
            string EmployeeGuid,
            ILogger log)
        {
            // we are initialised required variables 
            string requestBody = null;
            Employee objEmployeeDetails = null;
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Employee> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- Employee" +
                "Update");

            // we are reading or parsing the input request body
            requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            objEmployeeDetails = JsonConvert.DeserializeObject<Employee>(requestBody);

            if (objEmployeeDetails != null)
            {
                log.LogInformation($"Successfully Read Employee Details from body- {objEmployeeDetails.Name}, {objEmployeeDetails.PhoneNumber}" + $"{objEmployeeDetails.EmployeeId},{objEmployeeDetails.Email}");

                // initialising Azure Cosomosdb database connection.
                AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
                await objCosmosDBActivitiy.InitiateConnection();

                // modifying existing student information.
                // the id value is retriving from routing parameter.
                objEmployeeDetails.EmployeeGuid = EmployeeGuid;
                objInsertResponse = await objCosmosDBActivitiy.ModifyEmployeeItem(objEmployeeDetails);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while modifying information of student- {objEmployeeDetails.Name}, {objEmployeeDetails.PhoneNumber}" + $"{objEmployeeDetails.EmployeeId},{objEmployeeDetails.Email}";
                    log.LogInformation(objResponse.Message + "Error:" + objInsertResponse.StatusCode);
                }
                else
                {
                    objResponse.ErrorCode = 0;
                    objResponse.Message = "Successfully modified information.";
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
