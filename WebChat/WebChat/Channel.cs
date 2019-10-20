using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebChat.WebChat
{
	public class Channel
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string ChannelName { get; set; }
		public bool Private { get; set; }

		[JsonIgnore]
		[IgnoreDataMember]
		public List<Client> Clients { get; set; } = new List<Client>();
	}
}