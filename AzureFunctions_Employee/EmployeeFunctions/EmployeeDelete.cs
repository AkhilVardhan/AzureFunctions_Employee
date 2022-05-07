using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctionV3Learning.Models;
using AzureFunctionV3Learning.Common;
using Microsoft.Azure.Cosmos;
using System;

namespace AzureFunctionV3Learning.EmployeeFunctions
{
    public static class EmployeeDelete
    {
        [FunctionName("EmployeeDelete")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "EmployeeDelete/{EmployeeGUId}/{partitionKey}")] HttpRequest req,
            string EmployeeGUId,
            int partitionKey,
            ILogger log)
        {
            // we are initialised required variables 
            MyAzureFunctionResponse objResponse = new MyAzureFunctionResponse();
            ItemResponse<Employee> objInsertResponse = null;
            log.LogInformation("Calling Azure Function -- Employee Delete");
            // initialising Azure Cosomosdb database connection.
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // deleting existing Employee information based on id and partition key value
            
            try
            {
                objInsertResponse = await objCosmosDBActivitiy.DeleteEmployeeItem(EmployeeGUId, partitionKey);
                if (objInsertResponse == null)
                {
                    objResponse.ErrorCode = 100;
                    objResponse.Message = $"Error occured while deleting information of student- {EmployeeGUId}, {partitionKey}";
                    log.LogInformation(objResponse.Message + "Error:" + objInsertResponse.StatusCode);
                }
                else
                {
                    objResponse.ErrorCode = 0;
                    objResponse.Message = "Successfully deleted information.";
                }
            }
            catch(Exception ex)
            {
                log.LogInformation("Error Occured while deleting employee:" + ex.Message);
                objResponse.ErrorCode = 500;
                objResponse.Message = "Error Occured while deleting employee.";
            }
            return new OkObjectResult(objResponse);
        }
    }
}