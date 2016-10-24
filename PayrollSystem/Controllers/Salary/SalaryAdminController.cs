using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using PayrollSystem.Controllers.Salary;
using System.IO;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using PayrollSystem.Models;
using PayrollSystemLibrary;

namespace PayrollSystem.Controllers.SalaryAdmin
{
    public class SalaryAdminController : BaseController
    {
        public ActionResult Index()
        {
            if (ValidateLogin())
            {
                if (ValidateAdmin())
                {
                    SalaryClass model = new SalaryClass();
                    model.Year = db.GetYearList().ToList();
                    model.GetAllMonths = db.GetAllMonths().ToList();

                    Session["InAdminSection"] = "true";

                    return View(model);
                }
                return RedirectToAction("../Salary/GeneratePaySlip/");
            }
            else
            {
                return RedirectToAction("../Salary/");
            }
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, FormCollection form)
        {
            SalaryClass model = new SalaryClass();

            model.Year = db.GetYearList().ToList();
            model.GetAllMonths = db.GetAllMonths().ToList();


            if (ValidateAdmin())
            {
                if (form["submit"].ToString() == "Upload")
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["SalaryExcelFolderPath"]), fileName);
                        string Extension = Path.GetExtension(file.FileName);
                        file.SaveAs(path);

                        model.Message = Load_excel_to_db(path, Extension);

                        model.GetSalaryOfEmployeeByEmployeeIdMonthYear = db.GetSalaryOfEmployeeByEmployeeIdMonthYear(null, DateTime.Now.Month, DateTime.Now.Year.ToString()).ToList();                       
                    }

                }
                else if (form["submit"].ToString() == "Search")
                {
                    string month = form["Month"].ToString();
                    string year = form["SalaryYearAdmin"].ToString();

                    if (month == "" || year == "")
                    {
                        model.GetSalaryOfEmployeeByEmployeeIdMonthYear = db.GetSalaryOfEmployeeByEmployeeIdMonthYear(null, DateTime.Now.Month, DateTime.Now.Year.ToString()).ToList();
                    }
                    else
                    {
                        model.GetSalaryOfEmployeeByEmployeeIdMonthYear = db.GetSalaryOfEmployeeByEmployeeIdMonthYear(null, int.Parse(month), year).ToList();

                    }


                }

                return View(model);

            }
            else
            {
                return RedirectToAction("../Salary/");
            }
        }

        private string Load_excel_to_db(string FilePath, string Extension)
        {
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"]
                             .ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"]
                              .ConnectionString;
                    break;
            }
            conStr = String.Format(conStr, FilePath, "Yes");
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            cmdExcel.Connection = connExcel;

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();

            //Read Data from First Sheet
            connExcel.Open();
            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);

            connExcel.Close();
            /////////////
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (db.GetSalaryOfEmployeeByEmployeeIdMonthYear(int.Parse(row["EmployeeId"].ToString().Trim()), int.Parse(row["MonthId"].ToString().Trim()), row["year"].ToString().Trim()).Count() > 0)
                    {
                        db.UpdateSalary(int.Parse(row["EmployeeId"].ToString().Trim()), int.Parse(row["MonthId"].ToString().Trim()), row["year"].ToString().Trim(), row["actual_basic"].ToString().Trim(),
                            row["actual_hra"].ToString().Trim(), row["actual_ca"].ToString().Trim(),
                            row["actual_ma"].ToString().Trim(), row["actual_ia"].ToString().Trim(), row["actual_sa"].ToString().Trim(),
                            row["earning_basic"].ToString().Trim(), row["earning_hra"].ToString().Trim(), row["earning_ca"].ToString().Trim(), row["earning_ma"].ToString().Trim(), row["earning_ia"].ToString().Trim(),
                            row["earning_sa"].ToString().Trim(), row["deduction_pf"].ToString().Trim(), row["deduction_pt"].ToString().Trim(),
                            row["deduction_tds"].ToString().Trim(), int.Parse(row["paid_days"].ToString().Trim()), int.Parse(row["present_days"].ToString().Trim()), int.Parse(row["w_off"].ToString().Trim()),
                            int.Parse(row["leave"].ToString().Trim()), int.Parse(row["absent"].ToString().Trim()));
                    }
                    else
                    {

                        db.AddSalary(int.Parse(row["EmployeeId"].ToString().Trim()), int.Parse(row["MonthId"].ToString().Trim()), row["year"].ToString().Trim(), row["actual_basic"].ToString().Trim(),
                            row["actual_hra"].ToString().Trim(), row["actual_ca"].ToString().Trim(),
                            row["actual_ma"].ToString().Trim(), row["actual_ia"].ToString().Trim(), row["actual_sa"].ToString().Trim(),
                            row["earning_basic"].ToString().Trim(), row["earning_hra"].ToString().Trim(), row["earning_ca"].ToString().Trim(), row["earning_ma"].ToString().Trim(), row["earning_ia"].ToString().Trim(),
                            row["earning_sa"].ToString().Trim(), row["deduction_pf"].ToString().Trim(), row["deduction_pt"].ToString().Trim(),
                            row["deduction_tds"].ToString().Trim(), int.Parse(row["paid_days"].ToString().Trim()), int.Parse(row["present_days"].ToString().Trim()), int.Parse(row["w_off"].ToString().Trim()),
                            int.Parse(row["leave"].ToString().Trim()), int.Parse(row["absent"].ToString().Trim()));
                    }

                }
                return "Excel data uploaded successfully";
            }
            catch(Exception ex)
            {
                return "Invalid file";
            }

            /////////////

        }

        public ActionResult EmployeeList()
        {
            SalaryClass model = new SalaryClass();

            model.GetAllEmployeeList = db.GetAllEmployeeList().ToList();

            return View(model);
        }
    }
}