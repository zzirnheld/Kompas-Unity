using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : KompasObject
{

    private CharacterCard[,] characters = new CharacterCard[7, 7];
    private SpellCard[,] spells = new SpellCard[7, 7];

    public void RemoveFromBoard(Card card)
    {
        //TODO
        throw new NotImplementedException();
    }
}
