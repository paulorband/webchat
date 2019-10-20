using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebChat.WebChat
{
	public static class MessageHelper
	{
		private static readonly string _privateMessagePattern = @"^(\/p\s)(@[a-zA-z0-9]*)";
		private static readonly string _tagMessagePattern = @"^(@[a-zA-z0-9]*)";
		public static bool CheckForTaggedMessage(string userMessage, out string taggedUserName, out string textMessage)
		{
			taggedUserName = textMessage = null;

			var regex = new Regex(_tagMessagePattern);
			Match match = regex.Match(userMessage);

			if (match.Success)
			{
				taggedUserName = match.Groups[1].ToString().Remove(0, 1);
				textMessage = regex.Replace(userMessage, string.Empty).Trim();
			}

			return match.Success;
		}

		public static bool CheckForPrivateMessage(string userMessage, out string receiverUserName, out string textMessage)
		{
			receiverUserName = textMessage = null;

			var regex = new Regex(_privateMessagePattern);
			Match match = regex.Match(userMessage);

			if (match.Success)
			{
				receiverUserName = match.Groups[2].ToString().Remove(0, 1);
				textMessage = regex.Replace(userMessage, string.Empty).Trim();
			}

			return match.Success;
		}

		public static Message PrepareMessage(Client sender, string userMessage)
		{
			var message = new Message
			{
				Sender = sender.UserName,
				TextMessage = userMessage,
				Date = DateTime.Now
			};

			if (CheckForPrivateMessage(userMessage, out string receiverUserName, out string textMessage))
			{
				message.Private = true;
				message.Receiver = receiverUserName;
				message.TextMessage = textMessage;
			}
			else if (CheckForTaggedMessage(userMessage, out string taggedUserName, out textMessage))
			{
				message.Tagged = true;
				message.Receiver = taggedUserName;
				message.TextMessage = textMessage;
			}

			return message;
		}

		public static string SerializeMessage(Message message)
		{
			return JsonConvert.SerializeObject(message, new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy hh:mm:ss" });
		}
	}
}