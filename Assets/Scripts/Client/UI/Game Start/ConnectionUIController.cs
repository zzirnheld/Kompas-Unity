using System.Net;
using KompasClient.GameCore;
using KompasCore.Helpers;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
	public class ConnectionUIController : MonoBehaviour
	{
		private const string Localhost = "127.0.0.1";
		public ClientGame clientGame;
		public TMP_InputField ipInputField;

		public enum ConnectionState { ChooseServer, WaitingForServer, WaitingForPlayer, SelectDeck, DeckAccepted, FinalLoading }
		[EnumNamedArray(typeof(ConnectionState))]
		public GameObject[] connectionStateParents; //Should be in order of the above enum

		public NewDeckSelectUIController deckSelectUIController;
		public DeckAcceptedUIController deckAcceptedUIController;

		private string defaultIP;

		public void ApplySettings(ClientSettings settings)
		{
			defaultIP = settings.defaultIP;
			if (ipInputField.text == string.Empty) ipInputField.text = defaultIP;
		}

		public void Connect(bool acceptEmpty)
		{
			string ip = ipInputField.text;
			if (string.IsNullOrEmpty(ip))
			{
				if (acceptEmpty) ip = Localhost;
				else return;
			}
			else if (!IPAddress.TryParse(ip, out _)) return;

			//Stash ip
			clientGame.ClientSettings.defaultIP = defaultIP = ip;
			clientGame.clientUIController.clientUISettingsController.SaveSettings();

			Show(ConnectionState.WaitingForServer);
			clientGame.clientNetworkCtrl.Connect(ip);
		}

		public void Hide() => gameObject.SetActive(false);

		public void Show(ConnectionState connectionState) => CollectionsHelper.ShowOnly(connectionStateParents, (int)connectionState);

		public void DeckAccepted()
		{
			deckAcceptedUIController.ShowFriendlyAvatar(deckSelectUIController.AvatarFileName);
			Show(UI.ConnectionUIController.ConnectionState.DeckAccepted);
		}
	}
}