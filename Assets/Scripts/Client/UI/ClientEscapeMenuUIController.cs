using KompasCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KompasClient.UI
{
	public class ClientEscapeMenuUIController : EscapeMenuUIController
	{
		public const string YouWon = "You Won!";
		public const string YouLost = "You Lost!";

		public TMP_Text winnerText;

		public void Update()
		{
			//if this is active and the player hits escape, go away
			if (Input.GetKeyDown(KeyCode.Escape)) Cancel();
		}

		public ClientSettingsUIController settingsCtrl;

		public override void Enable() => gameObject.SetActive(true);

		public void Enable(bool won)
		{
			winnerText.text = won ? YouWon : YouLost;
			winnerText.gameObject.SetActive(true);
			Enable();
		}

		public void GoToMainMenu() => SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);

		public void Rematch()
		{
			SceneManager.LoadScene(MainMenuUICtrl.ClientScene);
		}

		public void Settings() => settingsCtrl.Show();

		public void Exit() => Application.Quit();

		public void Cancel() => gameObject.SetActive(false);
	}
}