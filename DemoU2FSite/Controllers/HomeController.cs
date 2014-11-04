using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using DemoU2FSite.Models;
using Newtonsoft.Json.Linq;
using u2flib.Data;
using u2flib.Data.Messages;

namespace DemoU2FSite.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, string> storage = new Dictionary<string, string>();
        private const string DemoAppId = "Demo MVC .NET application.";

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View("Login");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginLogin(BeginLoginModel model)
        {
            string userName;
            if (!ModelState.IsValid || !storage.TryGetValue(model.UserName, out userName))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered");
                return View("Login", model);
            }

            DeviceRegistration registration = DeviceRegistration.FromJson(userName);
            StartedAuthentication startedAuthentication = u2flib.U2F.StartAuthentication(DemoAppId, registration);
            storage.Add(startedAuthentication.Challenge, startedAuthentication.ToJson());

            return View("FinishLogin", model);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteLogin(CompleteLoginModel model)
        {
            string userName;
            if (!ModelState.IsValid || !storage.TryGetValue(model.UserName, out userName))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered");
                return View(model);
            }

            DeviceRegistration registration = DeviceRegistration.FromJson(userName);
            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(model.TokenResponse);
            String challenge = authenticateResponse.GetClientData().Challenge;
            string theChallenge;
            
            if (storage.TryGetValue(challenge, out theChallenge))
            {
                StartedAuthentication authentication = StartedAuthentication.FromJson(theChallenge);
                storage.Remove(theChallenge);
                u2flib.U2F.FinishAuthentication(authentication, authenticateResponse, registration);

                return View("LoginCompleted", model);
            }

            ModelState.AddModelError("", "Error finding challenge");
            return View(model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register()
        {
            StartedRegistration startedRegistration = u2flib.U2F.StartRegistration(DemoAppId);
            storage.Add(startedRegistration.Challenge, startedRegistration.ToJson());
            JObject jObject = JObject.Parse(startedRegistration.ToJson());

            RegisterModel registerModel = new RegisterModel
                {
                    AppId = jObject["AppId"].ToString(),
                    Challenge = jObject["Challenge"].ToString(),
                    Version = jObject["Version"].ToString(),
                    UserName = string.Empty,
                    TokenResponse = string.Empty
                };

            return View(registerModel);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteRegister([FromBody]RegisterModel value)
        {
            // TODO need to figure this out.
            if (ModelState.IsValid)
            {
                RegisterResponse registerResponse = RegisterResponse.FromJson(value.TokenResponse);
                String challenge = registerResponse.GetClientData().Challenge;
                string theChallenge;
                if (storage.TryGetValue(challenge, out theChallenge))
                {
                    StartedRegistration startedRegistration = StartedRegistration.FromJson(theChallenge);
                    DeviceRegistration registration = u2flib.U2F.FinishRegistration(startedRegistration, registerResponse);
                    storage.Add(value.UserName, registration.ToJson());
                    storage.Remove(challenge);

                    return View("CompleteRegister");
                }
                return View("Register", value);
            }
            else
            {
                return View("Register", value);
            }
        }
    }
}
