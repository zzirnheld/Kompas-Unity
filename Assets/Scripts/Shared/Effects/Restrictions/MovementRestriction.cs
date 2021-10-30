using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class MovementRestriction
    {
        private const string Default = "Default";

        #region Basic Movement Restrictions
        private const string InPlay = "In Play";
        private const string DistinctSpace = "Distinct Space";
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
        private const string IsFriendlyTurn = "Is Friendly Turn";
        //TODO add a "default" restriction
        #endregion Basic Movement Restrictions

        //Whether the character has been activated (for Golems)
        private const string IsActive = "Activated";

        //Default restrictions are that only characters with enough n can move.
        public static readonly string[] defaultNormalMovementRestrictions = new string[]
        {
            InPlay,
            DistinctSpace, IsCharacter,
            CanMoveEnoughSpaces, DestinationCanMoveHere,
            StandardSpellMoveRestiction,
            NothingHappening, IsFriendlyTurn
        };

        public static readonly string[] defaultEffectMovementRestrictions = new string[]
        {
            InPlay,
            DistinctSpace,
            StandardSpellMoveRestiction
        };

        /// <summary>
        /// The array to be loaded in and defaults addressed
        /// </summary>
        public string[] normalRestrictionsFromJson = new string[] { Default };
        /// <summary>
        /// The array of restrictions to ignore, from the default restrictions (or other future groups of retrictions)
        /// </summary>
        public string[] normalRestrictionsToIgnore = new string[0];
        /// <summary>
        /// The array to be loaded in and defaults addressed
        /// </summary>
        public string[] effectRestrictionsFromJson = new string[] { Default };

        /// <summary>
        /// The actual list to use
        /// </summary>
        public readonly List<string> normalRestrictions = new List<string>();
        /// <summary>
        /// The actual list to use
        /// </summary>
        public readonly List<string> effectRestrictions = new List<string>();

        public GameCard Card { get; private set; }

        /// <summary>
        /// Sets the restriction's info.
        /// This is a distinct function in case the required parameters changes,
        /// to catch any other required intitialization at compile time.
        /// </summary>
        /// <param name="card"></param>
        public void SetInfo(GameCard card)
        {
            Card = card;

            normalRestrictions.AddRange(normalRestrictionsFromJson);
            if (normalRestrictionsFromJson.Contains(Default)) 
                normalRestrictions.AddRange(defaultNormalMovementRestrictions);
            normalRestrictions.RemoveAll(s => normalRestrictionsToIgnore.Contains(s));

            effectRestrictions.AddRange(effectRestrictionsFromJson);
            if (effectRestrictionsFromJson.Contains(Default))
                effectRestrictions.AddRange(defaultEffectMovementRestrictions);
        }

        private bool RestrictionValid(string restriction, Space space, bool isSwapTarget, bool byEffect)
        {
            switch (restriction)
            {
                case Default: return true;
                //normal restrictions
                case InPlay: return Card.Location == CardLocation.Field;
                case DistinctSpace: return Card.Position != space;
                case IsCharacter: return Card.CardType == 'C';
                case CanMoveEnoughSpaces: return Card.SpacesCanMove >= Card.Game.boardCtrl.ShortestEmptyPath(Card, space);
                case StandardSpellMoveRestiction: return Card.Game.ValidSpellSpaceFor(Card, space);
                case NothingHappening: return Card.Game.NothingHappening;
                case IsFriendlyTurn: return Card.Game.TurnPlayer == Card.Controller;
                case DestinationCanMoveHere:
                    if (isSwapTarget) return true;
                    var atDest = Card.Game.boardCtrl.GetCardAt(space);
                    if (atDest == null) return true;
                    if (byEffect) return atDest.MovementRestriction.EvaluateEffectMove(Card.Position, isSwapTarget: true);
                    else if (atDest.Controller != Card.Controller) return false; //TODO later allow for cards that *can* swap with enemies
                    else return atDest.MovementRestriction.EvaluateNormalMove(Card.Position, isSwapTarget: true);

                //special effect restrictions
                case IsActive: return Card.Activated;
                default: throw new System.ArgumentException($"Could not understand movement restriction {restriction}");
            }
        }

        /* This exists to debug a card's movement restriction,
         * but should not be usually used because it prints a ton whenever
         * the game checks to see if a person has a response.*/
        private bool RestrictionValidWithDebug(string restriction, Space space, bool isSwapTarget, bool byEffect)
        {
            bool valid = RestrictionValid(restriction, space, isSwapTarget, byEffect);
            if (!valid) Debug.LogWarning($"{Card.CardName} cannot move to {space} because it flouts the movement restriction {restriction}");
            return valid;
        }

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as a normal action
        /// </summary>
        /// <param name="x">X coord this card might move to</param>
        /// <param name="y">Y coord this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateNormalMove(Space space, bool isSwapTarget = false)
            => space.Valid && normalRestrictions.All(r => RestrictionValidWithDebug(r, space, isSwapTarget, false));

        /// <summary>
        /// Checks whether the card this is attached to can move to (x, y) as part of an effect
        /// </summary>
        /// <param name="x">X coord this card might move to</param>
        /// <param name="y">Y coord this card might move to</param>
        /// <param name="isSwapTarget">Whether this card is the target of a swap. <br></br>
        /// If this is true, ignores "Destination Can Move Here" restriction, because otherwise you would have infinite recursion.</param>
        /// <returns><see langword="true"/> if the card can move to (x, y); <see langword="false"/> otherwise.</returns>
        public bool EvaluateEffectMove(Space space, bool isSwapTarget = false)
            => space.Valid && effectRestrictions.All(r => RestrictionValidWithDebug(r, space, isSwapTarget: false, byEffect: true));
    }
}