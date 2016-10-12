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

namespace PayrollSystem.Controllers.SalaryAdmin
{
    public class SalaryAdminController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result> lst = (List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result>)(Session["user"]);
                if (lst.FirstOrDefault().IsAdmin == true)
                {
                    return View();
                }
                return RedirectToAction("../Salary/GeneratePaySlip/");
            }
            else
            {
                return RedirectToAction("../Salary/");
            }
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["SalaryExcelFolderPath"]), fileName);
                string Extension = Path.GetExtension(file.FileName);
                file.SaveAs(path);
                Import_To_Grid(path, Extension);
            }
            return RedirectToAction("Index");
        }

        private void Import_To_Grid(string FilePath, string Extension)
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

            /////////////

            foreach (DataRow row in dt.Rows)
            {
                db.AddSalary(int.Parse(row["EmployeeId"].ToString().Trim()), int.Parse(row["MonthId"].ToString().Trim()), row["year"].ToString().Trim(), row["actual_basic"].ToString().Trim(), 
                    row["actual_hra"].ToString().Trim(), row["actual_ca"].ToString().Trim(),
                    row["actual_ma"].ToString().Trim(), row["actual_ia"].ToString().Trim(), row["actual_sa"].ToString().Trim(),
                    row["earning_basic"].ToString().Trim(), row["earning_hra"].ToString().Trim(), row["earning_ca"].ToString().Trim(), row["earning_ma"].ToString().Trim(), row["earning_ia"].ToString().Trim(),
                    row["earning_sa"].ToString().Trim(), row["deduction_pf"].ToString().Trim(), row["deduction_pt"].ToString().Trim(),
                    row["deduction_tds"].ToString().Trim(), int.Parse(row["paid_days"].ToString().Trim()), int.Parse(row["present_days"].ToString().Trim()), int.Parse(row["w_off"].ToString().Trim()),
                    int.Parse(row["leave"].ToString().Trim()), int.Parse(row["absent"].ToString().Trim()));

            }

            /////////////

            connExcel.Close();
        }


    }
}