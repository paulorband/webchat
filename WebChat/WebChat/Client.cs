using System;
using System.Net.WebSockets;
using System.Runtime.Serialization;

namespace WebChat.WebChat
{
	public class Client
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string UserName { get; set; }
		
		[IgnoreDataMember]
		public WebSocket Socket { get; set; }
	}
}