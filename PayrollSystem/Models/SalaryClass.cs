﻿using System;
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

        public string SignUpMessage { get; set; }
        public string LoginMessage { get; set; }

        public List<GetAllMonths_Result> GetAllMonths { get; set; }

        public List<string> Year { get; set; }

        public List<GetMonthListByYear_Result> GetMonthListByYear { get; set; }
    }
}