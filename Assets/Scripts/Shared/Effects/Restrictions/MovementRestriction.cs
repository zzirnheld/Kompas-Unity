using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects
{
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
        //If this is a spell being moved, it can't be next to 2 other spells
        private const string StandardSpellMoveRestiction = "If Spell, Not Next to 2 Other Spells";
        private const string NothingHappening = "Nothing Happening";
        private const string IsNotAvatar = "Is Not Avatar";
        private const string IsFriendlyTurn = "Is Friendly Turn";
        //TODO add a "default" restriction
        #endregion Basic Movement Restrictions

        //Whether the character has been activated (for Golems)
        private const string IsActive = "Activated";

        //The actual list of restrictions, set by json.
        //Default restrictions are that only characters with enough n can move.
        public string[] normalMovementRestrictions = new string[] 
        {
            IsCharacter, CanMoveEnoughSpaces, DestinationCanMoveHere, StandardSpellMoveRestiction, NothingHappening, IsNotAvatar, IsFriendlyTurn
        };
        public string[] effectMovementRestrictions = new string[] { StandardSpellMoveRestiction };

        public GameCard Card { get; private set; }

        public void SetInfo(GameCard card)
        {
            Card = card;
        }

        private bool RestrictionValid(string restriction, int x, int y, bool isSwapTarget, bool byEffect)
        {
            switch (restriction)
            {
                //normal restrictions
                case IsCharacter: return Card.CardType == 'C';
                case CanMoveEnoughSpaces: return Card.SpacesCanMove >= Card.DistanceTo(x, y);
                case StandardSpellMoveRestiction: return Card.CardType != 'S' || Card.Game.ValidSpellSpace(x, y);
                case NothingHappening: return Card.Game.NothingHappening;
                case IsNotAvatar: return !Card.IsAvatar;
                case IsFriendlyTurn: return Card.Game.TurnPlayer == Card.Controller;
                case DestinationCanMoveHere:
                    if (isSwapTarget) return true;
                    var atDest = Card.Game.boardCtrl.GetCardAt(x, y);
                    if (atDest == null) return true;
                    if (byEffect) return atDest.MovementRestriction.EvaluateEffectMove(Card.Position, isSwapTarget: true);
                    else if (atDest.Controller != Card.Controller) return false; //TODO later allow for cards that *can* swap with enemies
                    else return atDest.MovementRestriction.EvaluateNormalMove(Card.Position, isSwapTarget: true);

                //special effect restrictions
                case IsActive: return Card.Activated;
                default: throw new System.ArgumentException($"Could not understand movement restriction {restriction}");
            }
        }

        private bool ValidIndices(int x, int y) => 0 <= x && x < 7 && 0 <= y && y < 7;

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as a normal action
        /// </summary>
        /// <param name="space">Space this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateNormalMove((int x, int y) space, bool isSwapTarget = false) => EvaluateNormalMove(space.x, space.y, isSwapTarget);

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as a normal action
        /// </summary>
        /// <param name="x">X coord this card might move to</param>
        /// <param name="y">Y coord this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateNormalMove(int x, int y, bool isSwapTarget = false)
            => ValidIndices(x, y) && normalMovementRestrictions.All(r => RestrictionValid(r, x, y, isSwapTarget, false));

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as part of an effect
        /// </summary>
        /// <param name="space">Space this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateEffectMove((int x, int y) space, bool isSwapTarget = false) => EvaluateEffectMove(space.x, space.y, isSwapTarget);

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as part of an effect
        /// </summary>
        /// <param name="x">X coord this card might move to</param>
        /// <param name="y">Y coord this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateEffectMove(int x, int y, bool isSwapTarget = false)
            => ValidIndices(x, y) && effectMovementRestrictions.All(r => RestrictionValid(r, x, y, isSwapTarget: false, byEffect: true));
    }
}