using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour
{
    public Game game;

    public const int SpacesInGrid = 7;
    public const float BoardLenOffset = 7f;
    public const float LenOneSpace = 2f;
    public const float SpaceOffset = LenOneSpace / 2;

    public static int PosToGridIndex(float pos)
    {
        return (int)((pos + BoardLenOffset) / (LenOneSpace));
    }

    public static float GridIndexToPos(int gridIndex)
    {
        return (float)((gridIndex * LenOneSpace) + SpaceOffset - BoardLenOffset);
    }

    public static Vector3 GridIndicesFromPos(int x, int y)
    {
        return new Vector3(GridIndexToPos(x), GridIndexToPos(y), 0.2f);
    }

    //private CharacterCard[,] characters = new CharacterCard[spacesOnBoard, spacesOnBoard];
    //private SpellCard[,] spells = new SpellCard[spacesOnBoard, spacesOnBoard];
    private readonly GameCard[,] cards = new GameCard[SpacesInGrid, SpacesInGrid];
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
    public GameCard GetCardAt(int x, int y)
    {
        if (!ValidIndices(x, y)) return null;
        return cards[x, y];
    }
    
    public bool ExistsCardOnBoard(Func<GameCard, bool> predicate)
    {
        foreach (var c in cards)
        {
            if (predicate(c)) return true;
        }
        return false;
    }

    public int GetNumCardsOnBoard()
    {
        int i = 0;
        foreach (GameCard card in cards){
            if (card != null) i++;
        }
        return i;
    }

    public void ResetCardsForTurn(Player turnPlayer)
    {
        foreach(GameCard c in cards) c?.ResetForTurn(turnPlayer);
    }
    #endregion

    #region game mechanics
    public void RemoveFromBoard(GameCard toRemove)
    {
        if (toRemove == null || toRemove.Location != CardLocation.Field) return;

        RemoveFromBoard(toRemove.BoardX, toRemove.BoardY);
    }

    public void RemoveFromBoard(int x, int y) => cards[x, y] = null;
    
    /// <summary>
    /// Puts the card on the board
    /// </summary>
    /// <param name="toPlay">Card to be played</param>
    /// <param name="toX">X coordinate to play the card to</param>
    /// <param name="toY">Y coordinate to play the card to</param>
    public virtual void Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
    {
        toPlay.Remove();

        Debug.Log($"In boardctrl, playing {toPlay.CardName} to {toX}, {toY}");

        if (toPlay.CardType == 'A') cards[toX, toY].AddAugment(toPlay, stackSrc);
        else cards[toX, toY] = toPlay;

        toPlay.Location = CardLocation.Field;
        toPlay.Position = (toX, toY);
        toPlay.Controller = controller;

        int i = GetNumCardsOnBoard();
        if (i > game.Leyload) game.Leyload = i;

        toPlay.gameObject.transform.localScale = Vector3.one;
    }

    //movement
    public virtual void Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
    {
        Debug.Log($"Swapping {card?.CardName} to {toX}, {toY}");

        if (!ValidIndices(toX, toY) || card == null) return;
        if (card.AugmentedCard != null) throw new NotImplementedException();

        var (tempX, tempY) = card.Position;
        GameCard temp = cards[toX, toY];
        cards[toX, toY] = card;
        cards[tempX, tempY] = temp;

        //then let the cards know they've been moved
        if (playerInitiated)
        {
            card.CountSpacesMovedTo((toX, toY));
            temp?.CountSpacesMovedTo((tempX, tempY));
        }
        card.Position = (toX, toY);
        if (temp != null) temp.Position = (tempX, tempY);
    }

    public void Move(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
    {
        if (!ValidIndices(toX, toY)) return;

        if (card.AugmentedCard != null) cards[toX, toY].AddAugment(card, stackSrc);
        else Swap(card, toX, toY, playerInitiated, stackSrc);
    }

    public void PutCardsBack()
    {
        foreach(GameCard card in cards)
        {
            if(card != null) card.PutBack();
        }
    }

    public bool ExistsCardOnBoard(CardRestriction restriction)
    {
        foreach(GameCard c in cards)
        {
            if (c != null && restriction.Evaluate(c)) return true;
        }

        return false;
    }

    public bool CanSummonTo(int playerIndex, int x, int y)
    {
        foreach(GameCard c in cards)
        {
            if (c != null && c.IsAdjacentTo(x, y) && c.ControllerIndex == playerIndex) return true;
        }

        return false;
    }

    public void DiscardSimples()
    {
        foreach(GameCard c in cards)
        {
            if (c != null && c.SpellSubtype == CardBase.SimpleSubtype) c.Discard();
        }
    }
    #endregion game mechanics

    #region cycling visible cards
    private void WhichCardsVisible(bool charsActive, bool spellsActive, bool augsActive)
    {
        //TODO check if this works
        foreach (GameCard card in cards)
        {
            if (card == null) continue;

            if (card.CardType == 'C')
            {
                card.gameObject.SetActive(charsActive);
                foreach (var augment in card.Augments)
                    augment.gameObject.SetActive(augsActive);
            }
            else if (card.CardType == 'S') card.gameObject.SetActive(spellsActive);
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
