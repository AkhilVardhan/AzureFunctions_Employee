using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using AzureFunctionV3Learning.Common;
using AzureFunctionV3Learning.Models;

namespace AzureFunctions_Employee.EmployeeFunctions
{
    public static class GetEmployeeById
    {
        [FunctionName("GetEmployeeById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetEmployeeById/{employeeGUID}/{partitionkey}")] HttpRequest req,
            string employeeGUID, int partitionkey,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - GetEmployeeById.");

            ItemResponse<Employee> objEmployeeResponse = null;
            log.LogInformation("Calling Azure Function -- GetEmployeeById");
            // initialising Azure Cosomosdb database connection.
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing employee information based on employee GUId and partition key i.e. employeeId value
            objEmployeeResponse = await objCosmosDBActivitiy.GetEmployeeItem(employeeGUID, partitionkey);
            return new OkObjectResult(objEmployeeResponse.Resource);
        }
    }
}
