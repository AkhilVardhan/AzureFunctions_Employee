using AzureFunctionV3Learning.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionV3Learning.Common
{
    public class AzureCosmoDBActivity
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "MyLearning";
        // private string containerId = "EmployeeContainer";
        private string containerId = "EmployeeLatest";
        internal Task<Employee> objEmployeeDetails;

        public List<Employee> EmployeeId { get; private set; }

        public async Task InitiateConnection()
        {
            // Create a new instance of the Cosmos Client 
            //configuring Azure Cosmosdb sql api details
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }


        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/StudentName" as the partition key since we're storing Student information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            //this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/StudentName");
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/EmployeeId");
        }

        public async Task<ItemResponse<Employee>> SaveNewEmployeeItem(Employee objEmployee)
        {
            ItemResponse<Employee> employeeResponse = null;
            try
            {
                //  studentResponse = await this.container.CreateItemAsync<Student>(objStudent, new PartitionKey(objStudent.Name));
                employeeResponse = await this.container.CreateItemAsync<Employee>(objEmployee, new PartitionKey(objEmployee.EmployeeId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return employeeResponse;
        }

        public async Task<ItemResponse<Employee>> ModifyEmployeeItem(Employee objEmployee)
        {
            ItemResponse<Employee> EmployeeResponse = null;
            try
            {
                /* Note : Partition Key value should not change */
                // studentResponse = await this.container.ReplaceItemAsync<Student>(objStudent, objStudent.StudentId, new PartitionKey(objStudent.Name));
                EmployeeResponse = await this.container.ReplaceItemAsync<Employee>(objEmployee, objEmployee.EmployeeGuid, new PartitionKey(objEmployee.EmployeeId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return EmployeeResponse;
        }


        public async Task<ItemResponse<Employee>> GetEmployeeItem(string EmployeeId, int partionKey)
        {
            ItemResponse<Employee> EmployeeResponse = null;
            try
            {
                EmployeeResponse = await this.container.ReadItemAsync<Employee>(EmployeeId, new PartitionKey(partionKey));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return EmployeeResponse;
        }

        public async Task<ItemResponse<Employee>> DeleteEmployeeItem(string EmployeeId, int partitionKey)
        {
            ItemResponse<Employee> EmployeeResponse = null;
            try
            {
                EmployeeResponse = await this.container.ReadItemAsync<Employee>(EmployeeId, new PartitionKey(partitionKey));
                if (EmployeeResponse != null)
                    EmployeeResponse = await this.container.DeleteItemAsync<Employee>(EmployeeId, new PartitionKey(EmployeeResponse.Resource.EmployeeId));
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
            return EmployeeResponse;
        }


        public async Task<List<Employee>> GetAllEmployees()
        {
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Employee> queryResultSetIterator = this.container.GetItemQueryIterator<Employee>(queryDefinition);

            List<Employee> lstEmployees = new List<Employee>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Employee> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                lstEmployees = currentResultSet.Select(r => new Employee()
                {
                    Name = r.Name,
                    Gender = r.Gender,
                    Designation = r.Designation,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    City = r.City,
                    Address = r.Address,
                    Pincode = r.Pincode,
                    JoinedDate = r.JoinedDate,
                    EmployeeGuid = r.EmployeeGuid,
                    EmployeeId = r.EmployeeId
                }).ToList();
               
            }
            return lstEmployees;
        }

    }
}
