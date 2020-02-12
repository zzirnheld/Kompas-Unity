using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUICtrl : MonoBehaviour
{
    public const int MainMenuScene = 0;
    public const int ClientScene = 1;
    public const int ServerScene = 2;
    public const int DeckbuildScene = 3;

    public void StartServer()
    {
        //load the server scene
        SceneManager.LoadScene(ServerScene);
    }

    public void StartClient()
    {
        //load the client scene
        SceneManager.LoadScene(ClientScene);
    }

    public void BuildDeck()
    {
        //load the deck building scene
        SceneManager.LoadScene(DeckbuildScene);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
