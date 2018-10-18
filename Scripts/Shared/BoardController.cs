using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : KompasObject
{
    public const int spacesOnBoard = 7;

    private CharacterCard[,] characters = new CharacterCard[spacesOnBoard, spacesOnBoard];
    private SpellCard[,] spells = new SpellCard[spacesOnBoard, spacesOnBoard];
    /// <summary>
    /// Whether all cards, only chars, only spells, or only augs are visible
    /// </summary>
    private int visibleCards = 0;

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
    public void RemoveFromBoard(Card toRemove)
    {
        if (toRemove.Location != Card.CardLocation.Field) return;

        if (toRemove is CharacterCard)
            characters[toRemove.BoardX, toRemove.BoardY] = null;
        else if (toRemove is AugmentCard)
            (toRemove as AugmentCard).Detach();
        else if (toRemove is SpellCard)
            spells[toRemove.BoardX, toRemove.BoardY] = null;
    }

    public void Play(Card toPlay, int toX, int toY)
    {
        //TODO split into summon, augment, and cast
    }
    public void Summon(CharacterCard toSummon, int toX, int toY)
    {
        //TODO
    }
    public void Augment(AugmentCard toAugment, int toX, int toY)
    {
        //TODO
    }
    public void Cast(SpellCard toCast, int toX, int toY)
    {
        //TODO
    }

    public void Swap(Card card, int toX, int toY)
    {
        if (!ValidIndices(toX, toY) || card == null) return;

        Card temp = null;
        int tempX;
        int tempY;
        if (card is CharacterCard)
        {
            temp = characters[toX, toY];
            characters[toX, toY] = card as CharacterCard;
            characters[card.BoardX, card.BoardY] = temp as CharacterCard;
        }
        else if (card is AugmentCard) return; //TODO swap lists of augs
        else if (card is SpellCard)
        {
            temp = spells[toX, toY];
            spells[toX, toY] = card as SpellCard;
            spells[card.BoardX, card.BoardY] = temp as SpellCard;
        }

        tempX = card.BoardX;
        tempY = card.BoardY;
        //then let the cards know they've been moved
        card.MoveTo(toX, toY);
        if (temp != null) temp.MoveTo(tempX, tempY);
    }

    public void Move(Card card, int toX, int toY)
    {
        if (!ValidIndices(toX, toY)) return;

        if (card is CharacterCard)
        {
            //movement for characters always means swapping
            Swap(card, toX, toY);
            return;
        }
        else if (card is AugmentCard)
        {
            //moving an augment means attaching it to a different character, because 
            //it automatically moves with any character that it is attached to
            characters[toX, toY].AddAugment(card as AugmentCard);
        }
        else if (card is SpellCard)
        {
            //if there is a spell in the target locationt to move to, do nothing
            if (spells[toX, toY] != null) return;

            //but if there isn't go ahead and move the card there
            spells[toX, toY] = card as SpellCard;
            //and tell the game that there isn't anything where this card was.
            spells[card.BoardX, card.BoardY] = null;
        }
        //then let the card know it's been moved, for its x, y values and for its position
        card.MoveTo(toX, toY);
    }

    #region cycling visible cards
    private void WhichCardsVisible(bool charsActive, bool spellsActive, bool augsActive)
    {
        foreach (CharacterCard charCard in characters)
            if (charCard != null)
            {
                charCard.gameObject.SetActive(charsActive);
                foreach (AugmentCard augment in charCard.Augments) augment.gameObject.SetActive(augsActive);
            }
        foreach (SpellCard spellCard in spells)
            if (spellCard != null) spellCard.gameObject.SetActive(spellsActive);
    }

    public void CycleVisibleCards()
    {
        visibleCards = ++visibleCards % 4;

        switch (visibleCards)
        {
            case 0:
                //set all cards visible
                WhichCardsVisible(true, true, true);
                break;
            case 1:
                //hide all but characters
                WhichCardsVisible(true, false, false);
                break;
            case 2:
                //hide all but non-aug spells
                WhichCardsVisible(false, true, false);
                break;
            case 3:
                //hide all but augs
                WhichCardsVisible(false, false, true);
                break;
        }
    }
    #endregion


}
