using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour
{
    public Game game;

    public const int spacesOnBoard = 7;
    public const float spacesInGrid = 7;
    public const float boardLenOffset = 0.45f;

    protected static int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Divide by 0.9f because the range of accepted position values is 0 to 0.9f (0.45 - -0.45).
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)((pos + boardLenOffset) * spacesInGrid / 0.9f);
    }
    protected static float GridIndexToPos(int gridIndex)
    {
        /* first, cast the index to a float to make sure the math works out.
         * then, divide by the grid length to board ratio to get a number (0,1) that makes
         * sense in the context of the board's local lenth of one.
         * then, subtract the board length offset to get a number that makes sense
         * in the actual board's context of values (-0.45, 0.45) (legal local coordinates)
         * finally, add 0.025 to account for the 0.05 space on either side of the legal 0.45 area
         */
        return (((float)gridIndex) / spacesInGrid - boardLenOffset + 0.025f);
    }

    //private CharacterCard[,] characters = new CharacterCard[spacesOnBoard, spacesOnBoard];
    //private SpellCard[,] spells = new SpellCard[spacesOnBoard, spacesOnBoard];
    private Card[,] cards = new Card[spacesOnBoard, spacesOnBoard];
    /// <summary>
    /// Whether all cards, only chars, only spells, or only augs are visible
    /// </summary>
    private int visibleCards = 0;

    //helper methods
    #region helper methods
    public bool ValidIndices(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 7 && y < 7;
    }

    //get game data
    public Card GetCardAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y];
    }

    public CharacterCard GetCharAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y] as CharacterCard;
    }

    public SpellCard GetSpellAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y] as SpellCard;
    }

    public List<AugmentCard> GetAugmentsAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        //if the card at x y is null, or not a character card, returns null
        return (cards[x, y] as CharacterCard)?.Augments;
    }

    public int GetNumCardsOnBoard()
    {
        int i = 0;
        foreach (Card card in cards){
            if (card != null) i++;
        }
        return i;
    }

    public void ResetCardsForTurn(Player turnPlayer)
    {
        foreach(Card c in cards)
        {
            c?.ResetForTurn(turnPlayer);
        }
    }
    #endregion

    #region game mechanics
    public void RemoveFromBoard(Card toRemove)
    {
        if (toRemove == null || toRemove.Location != CardLocation.Field) return;

        if (toRemove is CharacterCard || toRemove is SpellCard)
            cards[toRemove.BoardX, toRemove.BoardY] = null;
        else if (toRemove is AugmentCard augToRemove)
            augToRemove.Detach();
    }

    public void RemoveFromBoard(int x, int y) { RemoveFromBoard(GetCardAt(x, y)); }

    //playing. these methods don't check whether it's client or server. that's the Game methods' jobs. 
    // these just do it (they're called by ClientNetworkController when ordered by the server)
    /// <summary>
    /// Actually summons the card. DO NOT call directly from player interaction
    /// </summary>
    public void Summon(CharacterCard toSummon, int toX, int toY, Player controller)
    {
        cards[toX, toY] = toSummon;
        toSummon.SetLocation(CardLocation.Field);
        toSummon.MoveTo(toX, toY, false);
        toSummon.ChangeController(controller);
    }

    /// <summary>
    /// Actually augments the card. DO NOT call directly from player interaction
    /// </summary>
    public void Augment(AugmentCard toAugment, int toX, int toY, Player controller)
    {
        GetCharAt(toX, toY).AddAugment(toAugment);
        toAugment.SetLocation(CardLocation.Field);
        toAugment.MoveTo(toX, toY, false);
        toAugment.ChangeController(controller);
    }

    /// <summary>
    /// Actually casts the card. DO NOT call directly from player interaction
    /// </summary>
    public void Cast(SpellCard toCast, int toX, int toY, Player controller)
    {
        cards[toX, toY] = toCast;
        toCast.SetLocation(CardLocation.Field);
        toCast.MoveTo(toX, toY, false);
        toCast.ChangeController(controller);
    }

    /// <summary>
    /// Calls the appropriate summon/augment/cast method for the card
    /// </summary>
    /// <param name="toPlay">Card to be played</param>
    /// <param name="toX">X coordinate to play the card to</param>
    /// <param name="toY">Y coordinate to play the card to</param>
    public void Play(Card toPlay, int toX, int toY, Player controller)
    {
        Debug.Log($"In boardctrl, playing {toPlay.CardName} to {toX}, {toY}");

        if (toPlay is CharacterCard charToPlay) Summon(charToPlay, toX, toY, controller);
        else if (toPlay is AugmentCard augmentToPlay) Augment(augmentToPlay, toX, toY, controller);
        else if (toPlay is SpellCard spellToPlay) Cast(spellToPlay, toX, toY, controller);
        else Debug.Log("Can't play a card that isn't a character, augment, or spell.");

        int i = GetNumCardsOnBoard();
        if (i > game.MaxCardsOnField) game.MaxCardsOnField = i;

        toPlay.gameObject.transform.localScale = new Vector3(1f / 9f, 1f / 9f, 1);
    }

    //movement
    public void Swap(Card card, int toX, int toY, bool playerInitiated)
    {
        Debug.Log($"Swapping {card.CardName} to {toX}, {toY}");

        if (!ValidIndices(toX, toY) || card == null) return;
        if (card is AugmentCard) throw new NotImplementedException();

        Card temp = null;
        int tempX;
        int tempY;
        temp                            = cards[toX, toY];
        cards[toX, toY]                 = card;
        cards[card.BoardX, card.BoardY] = temp;
        
        tempX = card.BoardX;
        tempY = card.BoardY;

        //then let the cards know they've been moved
        card.MoveTo(toX, toY, playerInitiated);
        if (temp != null) temp.MoveTo(tempX, tempY, playerInitiated);
    }

    public void Move(Card card, int toX, int toY, bool playerInitiated)
    {
        if (!ValidIndices(toX, toY)) return;

        if (card is AugmentCard augCard)
        {
            augCard.Detach();
            GetCharAt(toX, toY).AddAugment(augCard);
        }
        else Swap(card, toX, toY, playerInitiated);
    }

    public void PutCardsBack()
    {
        foreach(Card card in cards)
        {
            if(card != null) card.PutBack();
        }
    }

    public bool ExistsCardOnBoard(CardRestriction restriction)
    {
        foreach(Card c in cards)
        {
            if (c != null && restriction.Evaluate(c)) return true;
        }

        return false;
    }

    public bool CanSummonTo(int playerIndex, int x, int y)
    {
        foreach(Card c in cards)
        {
            if (c != null && c.IsAdjacentTo(x, y) && c.ControllerIndex == playerIndex) return true;
        }

        return false;
    }

    public void DiscardSimples()
    {
        foreach(Card c in cards)
        {
            if(c != null && c is SpellCard spellC && spellC.SpellSubtype == SpellCard.SimpleSubtype)
            {
                game.Discard(c);
            }
        }
    }
    #endregion game mechanics

    #region cycling visible cards
    private void WhichCardsVisible(bool charsActive, bool spellsActive, bool augsActive)
    {
        //TODO check if this works
        foreach (Card card in cards)
        {
            if (card == null) continue;

            if (card is CharacterCard charCard)
            {
                card.gameObject.SetActive(charsActive);
                foreach (AugmentCard augment in charCard.Augments)
                    augment.gameObject.SetActive(augsActive);
            }
            else if (card is SpellCard) card.gameObject.SetActive(spellsActive);
        }
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

    public void OnMouseDown()
    {
        //select nothing
        game.uiCtrl.SelectCard(null, true);

        if (game.targetMode != Game.TargetMode.SpaceTarget) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var intersection = transform.InverseTransformPoint(hit.point);

            int xIntersection = PosToGridIndex(intersection.x);
            int yIntersection = PosToGridIndex(intersection.y);
            //then, if the game is a clientgame, request a space target
            game.OnClickBoard(xIntersection, yIntersection);
        }
    }
}
