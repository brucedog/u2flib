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
        private const string DemoAppId = "http://localhost:52701/Home";

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Index()
        {
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

            return View("FinishLogin");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompletedLogin(CompleteLoginModel model)
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

                return View("CompletedLogin", model);
            }

            ModelState.AddModelError("", "Error finding challenge");
            return View(model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult BeginRegister([FromBody]RegisterModel value)
        {

            if (value.Password.Equals(value.ConfirmPassword) 
                && !string.IsNullOrWhiteSpace(value.Password)
                && !string.IsNullOrWhiteSpace(value.UserName))
            {

                StartedRegistration startedRegistration = u2flib.U2F.StartRegistration(DemoAppId);
                storage.Add(startedRegistration.Challenge, value.UserName.Trim());
                JObject jObject = JObject.Parse(startedRegistration.ToJson());
                RegisterModel registerModel = new RegisterModel
                                              {
                                                  UserName = value.UserName,
                                                  AppId = jObject["AppId"].ToString(),
                                                  Challenge = jObject["Challenge"].ToString(),
                                                  Version = jObject["Version"].ToString()
                                              };

                return View("CompleteRegister", registerModel);
            }
            else
            {
                return View("CompleteRegister", value);
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteRegister([FromBody]RegisterModel value)
        {
            if (!string.IsNullOrWhiteSpace(value.TokenResponse)
                && !string.IsNullOrWhiteSpace(value.UserName))
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
                }

                return View("FinishRegister");
            }
            else
            {
                return View("CompleteRegister", value);
            }
        }
    }
}
