using System.Linq;
using System.Web.Mvc;
using BaseLibrary;
using DataModels;
using u2flib.Util;

namespace DemoU2FSite.Controllers
{
    [Authorize]
    [LegacyAuthorize]
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemeberShipService _memeberShipService;

        public ProfileController(IUserRepository userRepository, IMemeberShipService memeberShipService)
        {
            _userRepository = userRepository;
            _memeberShipService = memeberShipService;
        }

        public ActionResult Index()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User has timed out.");
                RedirectToAction("Login", "Home");
            }
            var user = _userRepository.FindUser(HttpContext.User.Identity.Name);
            return View("Index", user);
        }

        public void AddDevice(string deviceResponse)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User has timed out.");
                RedirectToAction("Login", "Home");
            }
            _memeberShipService.CompleteRegistration(HttpContext.User.Identity.Name, deviceResponse);
        }

        public JsonResult GetChallenge()
        {
            ServerRegisterResponse serverRegisterResponse = _memeberShipService.GenerateServerChallenge(HttpContext.User.Identity.Name);
            CompleteRegisterModel registerModel = new CompleteRegisterModel
            {
                UserName = HttpContext.User.Identity.Name,
                AppId = serverRegisterResponse.AppId,
                Challenge = serverRegisterResponse.Challenge,
                Version = serverRegisterResponse.Version
            };

            JsonResult result = new JsonResult
            {
                Data = registerModel
            };
            return result;
        }

        public JsonResult DeviceInfo(int deviceId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User has timed out.");
                RedirectToAction("Login", "Home");
            }

            var user = _userRepository.FindUser(HttpContext.User.Identity.Name);
            var device = user.DeviceRegistrations.FirstOrDefault(f => f.Id == deviceId);
            dynamic formattedResult = new 
            {
                Id = device.Id,
                KeyHandle = Utils.ByteArrayToBase64String(device.KeyHandle),
                PublicKey = Utils.ByteArrayToBase64String(device.PublicKey),
                Counter = device.Counter,
                UpdatedOn = device.UpdatedOn.ToShortDateString()
            };
            JsonResult result = new JsonResult
            {
                Data = formattedResult
            };
            return result;
        }
    }
}