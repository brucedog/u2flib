using System;
using System.Web.Mvc;
using BaseLibrary;

namespace DemoU2FSite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ActionResult Index(string username)
        {
            var user = _userRepository.FindUser(username);
           
            return View("Index", user);
        }

        public void DeleteDevice(int id)
        {
            // TODO
            Console.Write(id);
        }
    }
}