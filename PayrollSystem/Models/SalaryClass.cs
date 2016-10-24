using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayrollSystemLibrary;
using System.Collections;

namespace PayrollSystem.Models
{
    public class SalaryClass
    {
        PayrollSystemEntities db = new PayrollSystemEntities();

        public Employee Employee { get; set; }

        public List<GetEmployeeByEmailAddress_Result> GetEmployeeByEmailAddress { get; set; }
        public List<GetSalaryOfEmployeeByEmployeeIdMonthYear_Result> GetSalaryOfEmployeeByEmployeeIdMonthYear { get; set; }
        public string Message { get; set; }

        public List<GetAllMonths_Result> GetAllMonths { get; set; }

        public List<string> Year { get; set; }

        public List<GetMonthListByYear_Result> GetMonthListByYear { get; set; }

        public List<GetAllEmployeeList_Result> GetAllEmployeeList { get; set; }
        
    }
}