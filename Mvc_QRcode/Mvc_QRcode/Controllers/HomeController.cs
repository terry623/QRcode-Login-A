using System;
using System.Web;
using System.Web.Mvc;
using Mvc_QRcode.Models;
using QRcodeTransfer;
using QRcodeTransfer.Repository;


namespace Mvc_QRcode.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var isUat = System.Configuration.ConfigurationManager.AppSettings["IsUAT"];
            string server_url;
            if (isUat.ToLower() == "y") server_url = System.Configuration.ConfigurationManager.AppSettings["mobile_assign"];
            else server_url = "http://localhost:53432";
            ViewBag.url = server_url;
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string userName, string userPassword)
        {
            var conn = new UserRepository();
            var response = conn.UserLogin(userName, userPassword);
            if (response == "Login Success") 
            {
                Session["UserName"] = userName;
                Session["Encode"] = "";
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult Register(string userName, string userPassword)
        {
            var conn = new UserRepository();
            var response = conn.UserRegister(userName, userPassword);
            return Json(response);
        }

        [AuthorizeFilter]
        public ActionResult Murri_Index()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult btnGoOnclick()
        {
            return View("GamePage");
        }

        public ActionResult Qr_click(string userName)
        {
            var isUat = System.Configuration.ConfigurationManager.AppSettings["IsUAT"];
            string server_url;
            if (isUat.ToLower() == "y") server_url = System.Configuration.ConfigurationManager.AppSettings["BUrl"];
            else server_url = "http://localhost:53432/";

            var key = System.Configuration.ConfigurationManager.AppSettings["aes_Key"];
            var encryptedText = AesAction.Encryption(userName, key);

            ViewBag.HtmlStr = QRcode.GenerateQRcode(encryptedText, server_url);
            TempData["encode"] = encryptedText.Replace(" ", "+");
            var conn = new UserRepository();
            conn.SetURL(userName,encryptedText); 
            return View("Murri_Index");
        }
    
    }
}
