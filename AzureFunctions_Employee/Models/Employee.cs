using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionV3Learning.Models
{
    public class Employee
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Gender")]
        public string Gender { get; set; }

        [JsonProperty("Designation")]
        public string Designation { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }


        [JsonProperty("Pincode")]
        public int Pincode { get; set; }

        [JsonProperty("JoinedDate")]
        public string JoinedDate { get; set; }

        [JsonProperty("EmployeeId")]
        public string EmployeeId { get; set; }

        [JsonProperty("id")]
        public string EmployeeGuid { get; set; }

    }

    public class Employees
    {
        public Employees()
        {
            LstEmployee = new List<Employee>();
        }
        public List<Employee> LstEmployee { get; set; }
    }
}
