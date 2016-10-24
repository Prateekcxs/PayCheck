using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollSystemLibrary;

namespace PayrollSystem.Controllers.Salary
{
    public class BaseController : Controller
    {
        public PayrollSystemEntities db = new PayrollSystemEntities();

        public ActionResult Logout()
        {
            Session.Remove("user");
            Session.Remove("InAdminSection");

            return RedirectToAction("../Salary/");
        }
        public ActionResult LoggedInNavigation()
        {
            return PartialView();
        }
        public ActionResult WelcomeHeader()
        {
            return PartialView();
        }

        public string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public bool ValidateLogin()
        {
            if (Session["user"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidateAdmin()
        {
            if (ValidateLogin())
            {
                List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result> lst = (List<PayrollSystemLibrary.GetEmployeeByEmailAndPassword_Result>)(Session["user"]);
                if (lst.FirstOrDefault().IsAdmin == true)
                {                    
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}