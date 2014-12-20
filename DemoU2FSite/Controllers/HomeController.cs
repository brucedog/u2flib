using System;
using System.Web.Http;
using System.Web.Mvc;
using DemoU2FSite.Models;

namespace DemoU2FSite.Controllers
{
    public class HomeController : Controller
    {
        private const string DemoAppId = "http://localhost:52701";
        private IMemeberShipService memeberShipService;

        public HomeController()
        {
             memeberShipService = new MemeberShipService();
        }
        
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login()
        {
            return View("Login");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginLogin(BeginLoginModel model)
        {
            if (!memeberShipService.IsUserRegistered(model.UserName.Trim(), model.Password.Trim()))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("CustomError", "User has not been registered");
                return View("Login", model);
            }

            try
            {
                ServerChallenge serverChallenge = memeberShipService.GenerateServerChallenge(model.UserName.Trim());

                CompleteLoginModel loginModel = new CompleteLoginModel
                                                {
                                                    AppId = serverChallenge.AppId,
                                                    KeyHandle = serverChallenge.KeyHandle,
                                                    Version = serverChallenge.Version,
                                                    Challenge = serverChallenge.Challenge,
                                                    UserName = model.UserName.Trim()
                                                };
                return View("FinishLogin", loginModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ModelState.AddModelError("CustomError", "User has not been registered");
            return View("Login", model);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompletedLogin(CompleteLoginModel model)
        {
            if (!memeberShipService.IsUserRegistered(model.UserName.Trim(), string.Empty))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered");
                return View("FinishLogin", model);
            }

            try
            {
                if (memeberShipService.AuthenticateUser(model.UserName.Trim(), model.DeviceResponse.Trim()))
                {
                    return View("CompletedLogin", model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                ModelState.AddModelError("", "Error finding challenge");
                return View("FinishLogin", model);
            }

            return View("FinishLogin", model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginRegister([FromBody] RegisterModel value)
        {
            if (value.Password.Equals(value.ConfirmPassword)
                && !string.IsNullOrWhiteSpace(value.Password)
                && !string.IsNullOrWhiteSpace(value.UserName))
            {
                ServerRegisterResponse serverRegisterResponse = memeberShipService.GenerateServerRegisteration(value.UserName.Trim(), value.Password.Trim());
                CompleteRegisterModel registerModel = new CompleteRegisterModel
                    {
                        UserName = value.UserName,
                        AppId = serverRegisterResponse.AppId,
                        Challenge = serverRegisterResponse.Challenge,
                        Version = serverRegisterResponse.Version
                    };

                return View("FinishRegister", registerModel);
            }

            ModelState.AddModelError("CustomError", "invalid input");
            return View("Register");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteRegister([FromBody] CompleteRegisterModel value)
        {
            if (!string.IsNullOrWhiteSpace(value.DeviceResponse)
                && !string.IsNullOrWhiteSpace(value.UserName))
            {
                try
                {
                    memeberShipService.CompleteRegisteration(value.UserName.Trim(), value.DeviceResponse.Trim());

                    return View("CompletedRegister", new CompleteRegisterModel{UserName = value.UserName});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ModelState.AddModelError("CustomError", e.Message);
                    return View("FinishRegister", value);
                }
            }
            
            ModelState.AddModelError("CustomError", "bad username/device response");
            return View("FinishRegister", value);
        }
    }
}