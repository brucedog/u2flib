using System;
using System.Web.Http;
using System.Web.Mvc;
using DataModels;
using Services;

namespace DemoU2FSite.Controllers
{
    public class HomeController : Controller
    {
        private const string DemoAppId = "http://localhost:52701";
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
            if ((string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))||
                (!_memeberShipService.IsUserRegistered(model.UserName.Trim())
                && !_memeberShipService.IsValidUserNameAndPassword(model.UserName.Trim(), model.Password.Trim())))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("CustomError", "User has not been registered");
                return View("Login", model);
            }

            try
            {
                ServerChallenge serverChallenge = _memeberShipService.GenerateServerChallenge(model.UserName.Trim());

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
            if (!_memeberShipService.IsUserRegistered(model.UserName.Trim()))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered");
                return View("FinishLogin", model);
            }

            try
            {
                if (_memeberShipService.AuthenticateUser(model.UserName.Trim(), model.DeviceResponse.Trim()))
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
            return View("Register");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginRegister([FromBody] RegisterModel value)
        {
            if (!string.IsNullOrWhiteSpace(value.Password)
                && !string.IsNullOrWhiteSpace(value.UserName)
                && value.Password.Equals(value.ConfirmPassword))
            {
                ServerRegisterResponse serverRegisterResponse = _memeberShipService.GenerateServerRegistration(value.UserName.Trim(), value.Password.Trim());
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
                    _memeberShipService.CompleteRegistration(value.UserName.Trim(), value.DeviceResponse.Trim());

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