using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollSystemLibrary;
using PayrollSystem.Models;
using System.Web.Security;

namespace PayrollSystem.Controllers.Salary
{
    public class SalaryController : BaseController
    {
       
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("GeneratePaySlip");
            }
            else
            {
                SalaryClass model = new SalaryClass();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Index(SalaryClass model, FormCollection form)
        {
            if (form["submit"].ToString().Trim() == "Sign up")
            {
                if (String.IsNullOrEmpty(model.Employee.Name) || String.IsNullOrEmpty(model.Employee.EmailAddress) || String.IsNullOrEmpty(model.Employee.Password) || String.IsNullOrEmpty(model.Employee.MobileNo) || String.IsNullOrEmpty(model.Employee.PAN_no))
                {
                    model.SignUpMessage = "Please fill out all fields";
                    return View(model);
                }
                if (db.GetEmployeeByEmailAddress(model.Employee.EmailAddress.Trim()).Count() > 0)
                {
                    model.SignUpMessage = "You are already registered, please login";
                    return View(model);
                }
                else
                {
                    db.AddEmployee(model.Employee.Name.Trim(), model.Employee.EmployeeCode, model.Employee.EmailAddress.Trim(),
                        FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.Password, "MD5"), model.Employee.Gender, FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.PAN_no.Trim(), "MD5"), model.Employee.Address,
                        model.Employee.MobileNo.Trim(), model.Employee.DesignationId, model.Employee.DepartmentId,
                        model.Employee.JoiningDate, model.Employee.TerminationDate, model.Employee.EmployeePFCode, false);

                    model.SignUpMessage = "Registered successfully";
                    return View(model);
                }
            }
            if (form["submit"].ToString().Trim() == "Login")
            {
                if (db.GetEmployeeByEmailAndPassword(model.Employee.EmailAddress.Trim(), FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.Password, "MD5")).Count() > 0)
                {
                    Session["user"] = db.GetEmployeeByEmailAndPassword(model.Employee.EmailAddress.Trim(), FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.Password, "MD5")).ToList();
                    return RedirectToAction("GeneratePaySlip");
                }
                else
                {
                    model.LoginMessage = "Username or password is incorrect";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult GeneratePaySlip()
        {
            if(Session["user"] ==null)
            {
                return RedirectToAction("Index");
            }
            SalaryClass model = new SalaryClass();           

            List<string> y_lst = new List<string>();

            y_lst = db.GetYearList().ToList();

            model.Year = y_lst;

            return View(model);
        }

        [HttpPost]
        public ActionResult GeneratePaySlip(FormCollection form)
        {
            string SelectedYear = form["SelectedYear"].ToString();
            SalaryClass model = new SalaryClass();
            List<string> y_lst = new List<string>();

            y_lst = db.GetYearList().ToList();

            model.Year = y_lst;

            model.GetMonthListByYear = (List<GetMonthListByYear_Result>)db.GetMonthListByYear(SelectedYear).ToList();

            return View(model);
        }
    }
}