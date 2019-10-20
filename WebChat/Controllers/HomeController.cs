using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebChat.WebChat;

namespace WebChat.Controllers
{
	public class HomeController : Controller
	{
		private readonly Regex ValidateUserPattern = new Regex(@"^[a-zA-Z0-9]*$");
		public ActionResult Index()
		{
			return View("Login");
		}

		[HttpPost]
		public ActionResult Index(string userName)
		{
			Validate(userName);

			if (!ModelState.IsValid)
				return View("Login", model: userName);

			return View(viewName: "Index", model: userName);
		}

		private void Validate(string userName)
		{
			if (!HubManager.UserNameIsAvailable(userName))
				ModelState.AddModelError("userName", "Apelido já está em uso!");

			if(!ValidateUserPattern.IsMatch(userName))
				ModelState.AddModelError("userName", "É permitido apenas letras e números");
		}
	}
}