using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUICtrl : MonoBehaviour
{
	public const int MainMenuScene = 0;
	public const int ClientScene = 1;
	public const int ServerScene = 2;
	public const int DeckbuildScene = 3;

	//load the server scene
	public void StartServer() => SceneManager.LoadScene(ServerScene);

	//load the client scene
	public void StartClient() => SceneManager.LoadScene(ClientScene);

	//load the deck building scene
	public void BuildDeck() => SceneManager.LoadScene(DeckbuildScene);

	public void CloseGame() => Application.Quit();
}
