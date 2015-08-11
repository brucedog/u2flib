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

        public ActionResult Index(string username)
        {
            //var user = _userRepository.FindUser(HttpContext.User.Identity.Name);
            var user = _userRepository.FindUser(username);
            return View("Index", user);
        }

        public void AddDevice(string username, string deviceResponse)
        {
            _memeberShipService.CompleteRegistration(username, deviceResponse);
        }

        public JsonResult GetChallenge(string username)
        {
            ServerRegisterResponse serverRegisterResponse = _memeberShipService.GenerateServerChallenge(username);
            CompleteRegisterModel registerModel = new CompleteRegisterModel
            {
                UserName = username,
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

        public JsonResult DeviceInfo(int deviceId, string username)
        {
            var user = _userRepository.FindUser(username);
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