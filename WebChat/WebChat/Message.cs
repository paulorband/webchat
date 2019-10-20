using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChat.WebChat
{
	public class Message
	{
		public string Sender { get; set; }
		public string Receiver { get; set; }
		public DateTime Date { get; set; }
		public string TextMessage { get; set; }
		public bool NewUser { get; set; }
		public bool UserLeft { get; set; }
		public bool Private { get; set; }
		public bool Tagged { get; set; }
	}
}