using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    
    public const bool DEBUG_MODE = true;

    public static Game mainGame;

    //game mechanics

    //game objects
    public GameObject fieldObject;

    //ui


    public virtual void SelectCard(Card card) { }

}
