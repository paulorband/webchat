using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebChat.WebChat;

namespace WebChat.Tests
{
	[TestClass]
	public class MessageHelperTest
	{
		[TestMethod]
		public void Quando_Uma_Mesagem_Tiver_Uma_Citacao_Privada_Deve_Ser_Possivel_Definir_O_Usuario_De_Destino()
		{
			var userMessage = "/p @jose Podemos conversar?";

			MessageHelper.CheckForPrivateMessage(userMessage, out string receiverUserName, out _);

			Assert.AreEqual("jose", receiverUserName);
		}

		[TestMethod]
		public void Quando_Uma_Mesagem_Tiver_Uma_Citacao_Privada_Deve_Ser_Possivel_Definir_A_Mensagem_Sem_A_Citacao()
		{
			var userMessage = "/p @jose Podemos conversar?";

			MessageHelper.CheckForPrivateMessage(userMessage, out _, out string textMessage);

			Assert.AreEqual("Podemos conversar?", textMessage);
		}

		[TestMethod]
		public void Quando_Uma_Mesagem_Tiver_Uma_Citacao_Publica_Deve_Ser_Possivel_Definir_O_Usuario_Citado()
		{
			var userMessage = "@jose Podemos conversar?";

			MessageHelper.CheckForTaggedMessage(userMessage, out string receiverUserName, out _);

			Assert.AreEqual("jose", receiverUserName);
		}

		[TestMethod]
		public void Quando_Uma_Mesagem_Tiver_Uma_Citacao_Publica_Deve_Ser_Possivel_Definir_A_Mensagem_Sem_A_Citacao()
		{
			var userMessage = "@jose Podemos conversar?";

			MessageHelper.CheckForTaggedMessage(userMessage, out _, out string textMessage);

			Assert.AreEqual("Podemos conversar?", textMessage);
		}
	}
}
