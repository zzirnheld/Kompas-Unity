using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementRestriction
{
    #region Basic Movement Restrictions
    //Might seem a bit dumb, but it means that some spells will be able to move themselves
    private const string IsCharacter = "Is Character";
    //Does the character have enough N?
    private const string CanMoveEnoughSpaces = "Can Move Enough Spaces";
    //If the space to be moved to isn't empty, can the other card there move to here?
    //In other words, if we're swapping, can the other card also move here?
    //Also checks that that other card is friendly
    private const string DestinationCanMoveHere = "Destination is Empty or Friendly";
    //TODO maybe a "destination can move here" to force swaps with enemy characters?
    #endregion Basic Movement Restrictions

    //Whether the character has been activated (for Golems)
    private const string IsActive = "Activated";

    //The actual list of restrictions, set by json.
    //Default restrictions are that only characters with enough n can move.
    public string[] Restrictions = new string[] { IsCharacter, CanMoveEnoughSpaces, DestinationCanMoveHere };
    
    public GameCard Card { get; private set; }

    public void SetInfo(GameCard card)
    {
        Card = card;
    }

    /// <summary>
    /// Checks whether the card this is attached to can move to (x, y)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="isSwapTarget">Whether this card is the target of a swap. 
    /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
    /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
    public bool Evaluate(int x, int y, bool isSwapTarget = false)
    {
        if (x < 0 || y < 0 || x > 6 || y > 6) return false;

        foreach(string r in Restrictions)
        {
            switch (r)
            {
                case IsCharacter:
                    if (Card.CardType != 'C') return false;
                    break;
                case CanMoveEnoughSpaces:
                    if (Card.SpacesCanMove < Card.DistanceTo(x, y)) return false;
                    break;
                case DestinationCanMoveHere:
                    if (isSwapTarget) break;
                    var atDest = Card.Game.boardCtrl.GetCardAt(x, y);
                    if (atDest == null) break;
                    if (atDest.Controller != Card.Controller) return false;
                    if (!atDest.MovementRestriction.Evaluate(Card.BoardX, Card.BoardY, isSwapTarget: true)) return false;
                    break;
                case IsActive:
                    if (!Card.Activated) return false;
                    break;
                default:
                    throw new System.ArgumentException($"Could not understand movement restriction {r}");
            }
        }

        return true;
    }
}
