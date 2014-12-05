using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using DemoU2FSite.Models;
using Newtonsoft.Json.Linq;
using u2flib;
using u2flib.Data;
using u2flib.Data.Messages;

namespace DemoU2FSite.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, string> storage = new Dictionary<string, string>();
        private const string DemoAppId = "http://localhost:52701";

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
            string deviceRegistration;
            if (!ModelState.IsValid || !storage.TryGetValue(model.UserName.Trim(), out deviceRegistration))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("CustomError", "User has not been registered");
                return View("Login", model);
            }

            try
            {
                DeviceRegistration registration = DeviceRegistration.FromJson(deviceRegistration);
                StartedAuthentication startedAuthentication = U2F.StartAuthentication(DemoAppId, registration);
                storage.Add(startedAuthentication.Challenge, startedAuthentication.ToJson());
                CompleteLoginModel loginModel = new CompleteLoginModel
                                                {
                                                    AppId = startedAuthentication.AppId,
                                                    KeyHandle = startedAuthentication.KeyHandle,
                                                    Version = startedAuthentication.Version,
                                                    Challenge = startedAuthentication.Challenge,
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
            string userName;
            if (!ModelState.IsValid || !storage.TryGetValue(model.UserName, out userName))
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "User has not been registered");
                return View("FinishLogin", model);
            }

            try
            {
                DeviceRegistration registration = DeviceRegistration.FromJson(userName);
                AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(model.DeviceResponse);
                String challenge = authenticateResponse.GetClientData().Challenge;
                string theChallenge;

                if (storage.TryGetValue(challenge, out theChallenge))
                {
                    StartedAuthentication authentication = StartedAuthentication.FromJson(theChallenge);
                    storage.Remove(theChallenge);
                    U2F.FinishAuthentication(authentication, authenticateResponse, registration);

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
                // user has already tried register
                if (storage.ContainsKey(value.UserName.Trim()))
                {
                    storage.Remove(value.UserName.Trim());
                }

                StartedRegistration startedRegistration = U2F.StartRegistration(DemoAppId);
                string startedRegistrationJson = startedRegistration.ToJson();
                storage.Add(value.UserName.Trim(), startedRegistrationJson);
                JObject jObject = JObject.Parse(startedRegistrationJson);
                CompleteRegisterModel registerModel = new CompleteRegisterModel
                    {
                        UserName = value.UserName,
                        AppId = jObject["AppId"].ToString(),
                        Challenge = jObject["Challenge"].ToString(),
                        Version = jObject["Version"].ToString()
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
                    RegisterResponse registerResponse = RegisterResponse.FromJson(value.DeviceResponse);
                    String challenge = registerResponse.GetClientData().Challenge;
                    string theChallenge;
                    if (storage.TryGetValue(value.UserName, out theChallenge))
                    {
                        StartedRegistration startedRegistration = StartedRegistration.FromJson(theChallenge);                        
                        DeviceRegistration registration = U2F.FinishRegistration(startedRegistration, registerResponse);
                        storage.Remove(value.UserName);
                        storage.Add(value.UserName.Trim(), registration.ToJson());
                    }

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