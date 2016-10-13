using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollSystemLibrary;
using PayrollSystem.Models;
using System.Web.Security;
using System.IO;
using System.Web.UI;
using NReco.PdfGenerator;
using System.Net.Mail;

namespace PayrollSystem.Controllers.Salary
{
    public class SalaryController : BaseController
    {
        string SalarySlipHtmlContent = "";
        string output_path_pdf = "";
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
                        FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.Password, "MD5"), model.Employee.Gender, model.Employee.PAN_no.Trim(), model.Employee.Address,
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
            if (Session["user"] == null)
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
            SalaryClass model = new SalaryClass();
            List<GetEmployeeByEmailAndPassword_Result> lst = (List<GetEmployeeByEmailAndPassword_Result>)(Session["user"]);
            if (form["btnSubmit"].ToString().Trim() == "Generate payslip")
            {

                model.GetEmployeeByEmailAddress = db.GetEmployeeByEmailAddress(lst.FirstOrDefault().EmailAddress).ToList();

                SmtpClient MailObj = new SmtpClient();
                MailMessage mm1 = new MailMessage(new MailAddress("prateek@cynexis.com", "Salary Slip"), new MailAddress(model.GetEmployeeByEmailAddress.FirstOrDefault().EmailAddress, model.GetEmployeeByEmailAddress.FirstOrDefault().EmailAddress));
                mm1.Subject = "Salary slip for month of ";
                mm1.Body = "Please find the attachment of your salary slip for month of ";


                string[] selectedMonths = form["chkMonthList"].Split(',');


                foreach (string selectedMonth in selectedMonths)
                {

                    model.GetSalaryOfEmployeeByEmployeeIdMonthYear = db.GetSalaryOfEmployeeByEmployeeIdMonthYear(lst.FirstOrDefault().EmployeeId, int.Parse(selectedMonth), form["SelectedYear"].ToString()).ToList();

                    SalarySlipHtmlContent = ViewToString("SalarySlipHtml", model);

                    mm1.Subject += model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + ", ";
                    mm1.Body += model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + ", ";


                    ///////////////////////////////////////////////// Generate PDF from HTML //////////////////////////////

                    HtmlToPdfConverter htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                    byte[] pdfBytes = htmlToPdf.GeneratePdf(SalarySlipHtmlContent);

                    output_path_pdf = Server.MapPath("/SalaryPDFFiles/" + "Salary Slip_" + model.GetEmployeeByEmailAddress.FirstOrDefault().Name + "_" +
                                                                model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + "_" +
                                                                model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year + ".pdf");

                    System.IO.File.WriteAllBytes(output_path_pdf, pdfBytes);

                    /////////////////////////////////////End Generate PDF from HTML //////////////////////////////


                    Attachment file = new Attachment(output_path_pdf);

                    mm1.Attachments.Add(file);

                }

                mm1.Subject = mm1.Subject.Trim().TrimEnd(',');
                mm1.Body = mm1.Body.Trim().TrimEnd(',');
                // mm1.Subject += " " + model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year;
                //   mm1.Body += " " + model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year;
                mm1.IsBodyHtml = true;
                mm1.Priority = MailPriority.Normal;
                MailObj.Send(mm1);

            }




            List<string> y_lst = new List<string>();

            y_lst = db.GetYearList().ToList();

            model.Year = y_lst;
            return View(model);
        }

        public ActionResult MonthList(int? year)
        {
            if (year == null)
            {
                return PartialView();
            }
            SalaryClass model = new SalaryClass();


            List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result> lst = (List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result>)(Session["user"]);
            model.GetMonthListByYear = (List<GetMonthListByYear_Result>)db.GetMonthListByYear(year.ToString(), lst.FirstOrDefault().EmployeeId).ToList();

            return PartialView("MonthList", model);
        }

    }
}