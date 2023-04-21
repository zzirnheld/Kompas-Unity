using UnityEngine;
using UnityEngine.SceneManagement;

namespace KompasCore.UI
{
	public class MainMenuButtonUIController : MonoBehaviour
	{
		public void GoToMainMenu()
		{
			SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);
		}
	}
}