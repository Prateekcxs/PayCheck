//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PayrollSystemLibrary
{
    using System;
    
    public partial class GetSalaryOfEmployeeByEmployeeIdMonthYear_Result
    {
        public int SalaryId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public int MonthId { get; set; }
        public string Year { get; set; }
        public string actual_basic { get; set; }
        public string actual_hra { get; set; }
        public string actual_ca { get; set; }
        public string actual_ma { get; set; }
        public string actual_ia { get; set; }
        public string actual_sa { get; set; }
        public string earning_basic { get; set; }
        public string earning_hra { get; set; }
        public string earning_ca { get; set; }
        public string earning_ma { get; set; }
        public string earning_ia { get; set; }
        public string earning_sa { get; set; }
        public string deduction_pf { get; set; }
        public string deduction_pt { get; set; }
        public string deduction_tds { get; set; }
        public Nullable<int> paid_days { get; set; }
        public Nullable<int> present_days { get; set; }
        public Nullable<int> w_off { get; set; }
        public Nullable<int> leave { get; set; }
        public Nullable<int> absent { get; set; }
        public string MonthName { get; set; }
        public Nullable<int> Gross { get; set; }
        public Nullable<int> Gross_Total { get; set; }
        public Nullable<int> Net_Total { get; set; }
        public Nullable<int> Deduction_Total { get; set; }
    }
}
