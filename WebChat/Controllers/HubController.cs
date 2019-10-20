using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using WebChat.WebChat;

namespace WebChat.Controllers
{
	public class HubController : ApiController
	{
		public HttpResponseMessage Get()
		{
			if (HttpContext.Current.IsWebSocketRequest)
			{
				HttpContext.Current.AcceptWebSocketRequest(ProcessWebChat);
			}

			return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
		}

		private async Task ProcessWebChat(AspNetWebSocketContext context)
		{
			await new HubManager().Process(context);
		}
	}
}