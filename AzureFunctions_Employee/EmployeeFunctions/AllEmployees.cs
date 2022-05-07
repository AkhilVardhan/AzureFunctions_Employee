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
using System.Collections.Generic;

namespace AzureFunctionV3Learning.EmployeeFunctions
{
    public static class AllEmployee
    {
        [FunctionName("AllEmployee")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "allEmployees")] HttpRequest req,
            ILogger log)
        {
            //declaring the response
            List<Employee> lstEmployees = null;
            log.LogInformation("Calling Azure Function -- EmployeeGetId");
            // initialising Azure Cosomosdb database connection.
            AzureCosmoDBActivity objCosmosDBActivitiy = new AzureCosmoDBActivity();
            await objCosmosDBActivitiy.InitiateConnection();
            // retriving existing student information based on employeeGuid and partition key i.e. EmployeeId value
            lstEmployees = await objCosmosDBActivitiy.GetAllEmployees();
            return new OkObjectResult(lstEmployees);
        }
    }
}

