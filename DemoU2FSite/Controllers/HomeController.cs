using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using BaseLibrary;
using DataModels;
using Newtonsoft.Json;

namespace DemoU2FSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemeberShipService _memeberShipService;

        public HomeController(IMemeberShipService memeberShipService)
        {
            _memeberShipService = memeberShipService;
        }
        
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Index()
        {
            return View("Index");
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
            if ((string.IsNullOrWhiteSpace(model.Password))
                || !_memeberShipService.IsUserRegistered(model.UserName.Trim()))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("CustomError", "User has not been registered.");
                return View("Login", model);
            }

            if (!_memeberShipService.IsValidUserNameAndPassword(model.UserName.Trim(), model.Password.Trim()))
            {
                ModelState.AddModelError("CustomError", "User/Password is not invalid.");
                return View("Login", model);
            }

            try
            {
                List<ServerChallenge> serverChallenge = _memeberShipService.GenerateServerChallenges(model.UserName.Trim());

                if(serverChallenge == null || serverChallenge.Count == 0)
                    throw new Exception("No server challenges were generated.");

                var challenges = JsonConvert.SerializeObject(serverChallenge);
                CompleteLoginModel loginModel = new CompleteLoginModel
                                                {
                                                    AppId = serverChallenge.First().appId,
                                                    Version = serverChallenge.First().version,
                                                    UserName = model.UserName.Trim(),
                                                    Challenges = challenges
                                                };
                return View("FinishLogin", loginModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                ModelState.AddModelError("CustomError", e.Message);
                return View("Login", model);
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompletedLogin(CompleteLoginModel model)
        {
            if (!_memeberShipService.IsUserRegistered(model.UserName.Trim()))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered.");
                return View("FinishLogin", model);
            }

            try
            {
                if (!_memeberShipService.AuthenticateUser(model.UserName.Trim(), model.DeviceResponse.Trim()))
                    throw new Exception("Device response did not work with user.");

                FormsAuthentication.SetAuthCookie(model.UserName, true);
                return RedirectToAction("Index", "Profile", new {userName = model.UserName});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                ModelState.AddModelError("", "Error authenticating");
                return View("FinishLogin", model);
            }
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register()
        {
            return View("Register");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginRegister([FromBody] RegisterModel value)
        {
            if (_memeberShipService.IsUserRegistered(value.UserName))
            {
                ModelState.AddModelError("CustomError", "User is already registered.");
                return View("Register", value);
            }

            if (!string.IsNullOrWhiteSpace(value.Password)
                && !string.IsNullOrWhiteSpace(value.UserName)
                && value.Password.Equals(value.ConfirmPassword))
            {
                try
                {
                    _memeberShipService.SaveNewUser(value.UserName, value.Password);
                    ServerRegisterResponse serverRegisterResponse = _memeberShipService.GenerateServerChallenge(value.UserName.Trim());
                    CompleteRegisterModel registerModel = new CompleteRegisterModel
                    {
                        UserName = value.UserName,
                        AppId = serverRegisterResponse.AppId,
                        Challenge = serverRegisterResponse.Challenge,
                        Version = serverRegisterResponse.Version
                    };

                    return View("FinishRegister", registerModel);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ModelState.AddModelError("CustomError", e.Message);

                    return View("Register", value);
                }
            }

            ModelState.AddModelError("CustomError", "invalid input");
            return View("Register", value);
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
                    value.DeviceResponse = _memeberShipService.CompleteRegistration(value.UserName.Trim(),
                        value.DeviceResponse.Trim())
                        ? "Registration was successful."
                        : "Registration failed.";

                    return View("CompletedRegister", new CompleteRegisterModel{UserName = value.UserName, DeviceResponse = value.DeviceResponse});
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