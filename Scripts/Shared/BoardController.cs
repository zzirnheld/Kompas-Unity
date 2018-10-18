using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : KompasObject
{
    public const int spacesOnBoard = 7;

    private CharacterCard[,] characters = new CharacterCard[spacesOnBoard, spacesOnBoard];
    private SpellCard[,] spells = new SpellCard[spacesOnBoard, spacesOnBoard];

    //helper methods
    private bool ValidIndices(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 7 && y < 7;
    }

    //get game data
    public CharacterCard GetCharAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return characters[x, y];
    }
    public List<AugmentCard> GetAugmentsAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return characters[x, y].Augments;
    }
    public SpellCard GetSpellAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return spells[x, y];
    }

    //game mechanics
    public void RemoveFromBoard(Card card)
    {
        //TODO
        throw new NotImplementedException();
    }
}
