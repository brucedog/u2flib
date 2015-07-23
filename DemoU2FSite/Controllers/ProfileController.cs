using System.Web.Mvc;
using BaseLibrary;

namespace DemoU2FSite.Controllers
{
    [LegacyAuthorize]
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        public ActionResult Index(string username)
        {
            var user = _userRepository.FindUser(HttpContext.User.Identity.Name);

            return View("Index", user);
        }
    }
}