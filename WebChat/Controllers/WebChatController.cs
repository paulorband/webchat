using System.Collections.Generic;
using System.Web.Http;
using WebChat.WebChat;

namespace WebChat.Controllers
{
	public class WebChatController : ApiController
	{
		
		[HttpGet]
		public IEnumerable<Client> UsersConnected()
		{
			return HubManager.Clients.Values;
		}
	}
}