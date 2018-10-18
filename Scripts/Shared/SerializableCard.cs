using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableCard
{
    //card type
    public char cardType;

    //perma-values
    public string cardName;
    public string effText;
    public string subtypeText;

    //in game values
    public Card.CardLocation location;
    public bool friendly;
    public int BoardX;
    public int BoardY;

}
