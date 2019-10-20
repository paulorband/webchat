using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace WebChat.WebChat
{
	public class HubManager
	{
		public static ConcurrentDictionary<string, Client> Clients = new ConcurrentDictionary<string, Client>();

		public static bool UserNameIsAvailable(string userName) => !Clients.ContainsKey(userName);
		public async Task Process(AspNetWebSocketContext context)
		{
			var userName = context.QueryString.Get("userName");

			if (!Clients.TryGetValue(userName, out var currentClient))
			{
				currentClient = AddNewClient(context, userName);

				await NotifyAbouNewUser(currentClient);
			}

			while (true)
			{
				ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
				WebSocketReceiveResult result = await currentClient.Socket.ReceiveAsync(buffer, CancellationToken.None);

				if (currentClient.Socket.State == WebSocketState.CloseReceived)
				{
					await RemoveClient(currentClient);

					await NotifyAbouUserLeft(currentClient);

					continue;
				}

				Message message = PrepareMessageToSend(currentClient, result, buffer);

				if (message.Private)
				{
					await SendPrivate(currentClient, message);

					continue;
				}

				await SendToEveryOne(message);
			}
		}

		private Client AddNewClient(AspNetWebSocketContext context, string userName)
		{
			Client currentClient = new Client { UserName = userName, Socket = context.WebSocket };

			Clients.TryAdd(userName, currentClient);

			return currentClient;
		}

		private async Task SendToEveryOne(Message message)
		{
			foreach (var client in Clients)
			{
				if (client.Value.Socket.State == WebSocketState.Open)
				{
					WebSocket socket = client.Value.Socket;

					string messageSerialized = MessageHelper.SerializeMessage(message);

					await SendMessage(client.Value, messageSerialized);
				}
			}
		}

		private async Task SendPrivate(Client currentClient, Message message)
		{
			if (Clients.TryGetValue(message.Receiver, out Client receiver))
			{
				if (currentClient.UserName != receiver.UserName)
				{
					string messageSerialized = MessageHelper.SerializeMessage(message);

					await SendMessage(receiver, messageSerialized);
					await SendMessage(currentClient, messageSerialized);
				}
			}
		}

		private async Task RemoveClient(Client currentClient)
		{
			WebSocket socket = currentClient.Socket;

			await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, CancellationToken.None);

			Clients.TryRemove(currentClient.UserName, out Client _);
		}

		private async Task NotifyAbouNewUser(Client currentClient)
		{
			foreach (var client in Clients)
			{
				WebSocket socket = client.Value.Socket;

				if (socket.State == WebSocketState.Open)
				{
					var message = new Message { NewUser = true, Sender = currentClient.UserName };

					await SendMessage(client.Value, MessageHelper.SerializeMessage(message));
				}
			}
		}

		private async Task NotifyAbouUserLeft(Client currentClient)
		{
			foreach (var client in Clients)
			{
				WebSocket socket = client.Value.Socket;

				if (socket.State == WebSocketState.Open)
				{
					var message = new Message { UserLeft = true, Sender = currentClient.UserName };

					await SendMessage(client.Value, MessageHelper.SerializeMessage(message));
				}
			}
		}

		private async Task SendMessage(Client client, string message)
		{
			var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

			await client.Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
		}

		private Message PrepareMessageToSend(Client sender, WebSocketReceiveResult result, ArraySegment<byte> buffer)
		{
			string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
			
			Message message = MessageHelper.PrepareMessage(sender, userMessage);

			return message;
		}
	}
}