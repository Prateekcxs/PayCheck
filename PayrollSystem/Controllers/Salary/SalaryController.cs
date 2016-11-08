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
            if (!ValidateLogin())
            {
                SalaryClass model = new SalaryClass();
                return View(model);
            }
            else
            {
                return RedirectToAction("GeneratePaySlip");
            }
        }

        [HttpPost]
        public ActionResult Index(SalaryClass model, FormCollection form)
        {
            if (form["submit"].ToString().Trim() == "Sign up")
            {
                if (String.IsNullOrEmpty(model.Employee.Name) || String.IsNullOrEmpty(model.Employee.EmailAddress) || String.IsNullOrEmpty(model.Employee.Password) || String.IsNullOrEmpty(model.Employee.MobileNo) || String.IsNullOrEmpty(model.Employee.PAN_no))
                {
                    model.Message = "Please fill out all fields";
                    return View(model);
                }
                if (db.GetEmployeeByEmailAddress(model.Employee.EmailAddress.Trim()).Count() > 0)
                {
                    model.Message = "You are already registered, please login";
                    return View(model);
                }
                else
                {
                    db.AddEmployee(model.Employee.Name.Trim(), model.Employee.EmployeeCode, model.Employee.EmailAddress.Trim(),
                        FormsAuthentication.HashPasswordForStoringInConfigFile(model.Employee.Password, "MD5"), model.Employee.Gender, model.Employee.PAN_no.Trim(), model.Employee.Address,
                        model.Employee.MobileNo.Trim(), model.Employee.DesignationId, model.Employee.DepartmentId,
                        model.Employee.JoiningDate, model.Employee.TerminationDate, model.Employee.EmployeePFCode, false);

                    model.Message = "Registered successfully, please login";
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
                    model.Message = "Username or password is incorrect";
                    return View(model);
                }
            }
            else
            {
                model.Message = "Unknown action !!!";
                return View(model);
            }
        }


        public ActionResult GeneratePaySlip()
        {
            if (ValidateLogin())
            {
                SalaryClass model = new SalaryClass();

                List<string> y_lst = new List<string>();

                y_lst = db.GetYearList().ToList();

                model.Year = y_lst;

                if (model.Year.Count == 0)
                {
                    ViewData["Message"] = "No salary data for you has been uploaded yet";
                }

                Session.Remove("InAdminSection");

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public ActionResult GeneratePaySlip(FormCollection form)
        {
            if (ValidateLogin())
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

                        output_path_pdf = Server.MapPath("/SalaryPDFFiles/" + "Salary Slip_" + model.GetEmployeeByEmailAddress.FirstOrDefault().Name + "_" +
                                                                   model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + "_" +
                                                                   model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year + ".pdf");



                        if (System.IO.File.Exists(output_path_pdf))
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();
                            System.IO.File.Delete(output_path_pdf);
                        }
                        SalarySlipHtmlContent = ViewToString("SalarySlipHtml", model);



                        ///////////////////////////////////////////////// Generate PDF from HTML //////////////////////////////

                        HtmlToPdfConverter htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();

                        byte[] pdfBytes = htmlToPdf.GeneratePdf(SalarySlipHtmlContent);

                        System.IO.File.WriteAllBytes(output_path_pdf, pdfBytes);

                        /////////////////////////////////////End Generate PDF from HTML //////////////////////////////





                        mm1.Subject += model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + ", ";
                        mm1.Body += model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().MonthName + ", ";
                        mm1.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                        mm1.SubjectEncoding = System.Text.Encoding.GetEncoding("utf-8");
                        Attachment file = new Attachment(output_path_pdf);

                        mm1.Attachments.Add(file);

                    }

                    mm1.Subject = mm1.Subject.Trim().TrimEnd(',');
                    mm1.Body = mm1.Body.Trim().TrimEnd(',');
                    // mm1.Subject += " " + model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year;
                    //   mm1.Body += " " + model.GetSalaryOfEmployeeByEmployeeIdMonthYear.FirstOrDefault().Year;
                    //    mm1.IsBodyHtml = true;
                    mm1.Priority = MailPriority.Normal;

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            MailObj.Send(mm1);
                            model.Message = "Your payslips for selected months has been emailed to your registered email id";
                            break;
                        }
                        catch (Exception ex)
                        {
                            model.Message = "Error occured: " + ex.Message;
                            continue;
                        }
                    }

                }



                List<string> y_lst = new List<string>();

                y_lst = db.GetYearList().ToList();

                model.Year = y_lst;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult MonthList(int? year)
        {
            if (ValidateLogin())
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
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditProfile()
        {
            if (ValidateLogin())
            {
                List<GetEmployeeByEmailAndPassword_Result> lst = (List<GetEmployeeByEmailAndPassword_Result>)(Session["user"]);

                SalaryClass model = new SalaryClass();
                model.GetEmployeeByEmailAddress = db.GetEmployeeByEmailAddress(lst.FirstOrDefault().EmailAddress).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(SalaryClass model)
        {
            if (ValidateLogin())
            {
                db.UpdateEmployee(model.GetEmployeeByEmailAddress.FirstOrDefault().EmployeeId, model.GetEmployeeByEmailAddress.FirstOrDefault().Name, model.GetEmployeeByEmailAddress.FirstOrDefault().EmployeeCode,
                                  FormsAuthentication.HashPasswordForStoringInConfigFile(model.GetEmployeeByEmailAddress.FirstOrDefault().Password, "MD5"), model.GetEmployeeByEmailAddress.FirstOrDefault().Gender,
                                  model.GetEmployeeByEmailAddress.FirstOrDefault().PAN_no, model.GetEmployeeByEmailAddress.FirstOrDefault().Address, model.GetEmployeeByEmailAddress.FirstOrDefault().MobileNo,
                                  model.GetEmployeeByEmailAddress.FirstOrDefault().DesignationId, model.GetEmployeeByEmailAddress.FirstOrDefault().DepartmentId, model.GetEmployeeByEmailAddress.FirstOrDefault().JoiningDate,
                                  model.GetEmployeeByEmailAddress.FirstOrDefault().TerminationDate, model.GetEmployeeByEmailAddress.FirstOrDefault().EmployeePFCode, model.GetEmployeeByEmailAddress.FirstOrDefault().IsAdmin);
                model.Message = "Updated successfully";

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}