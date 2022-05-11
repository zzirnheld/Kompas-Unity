using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class AttackRestriction : ContextInitializeableBase
    {
        public const string ThisIsCharacter = "This is Character";
        public const string DefenderIsCharacter = "Defender is Character";
        public const string DefenderIsAdjacent = "Defender is Adjacent";
        public const string DefenderIsEnemy = "Defender is Enemy";
        public const string FriendlyTurn = "Friendly Turn";
        public const string MaxPerTurn = "Maximum Per Turn";
        public const string NothingHappening = "Nothing Happening";
        public const string InPlay = "In Play";

        public const string Default = "Default";

        public const string ThisIsActive = "This is Activated";
        public const string NotNormally = "Not Normally";
        public const string DefenderFitsRestriction = "Defender Fits Card Restriction";

        public static readonly string[] DefaultAttackRestrictions = { ThisIsCharacter, DefenderIsCharacter, DefenderIsAdjacent, DefenderIsEnemy,
            FriendlyTurn, MaxPerTurn, NothingHappening, InPlay };

        public static readonly string[] AtAllRestrictions = { ThisIsCharacter, FriendlyTurn, MaxPerTurn, InPlay };

        public readonly List<string> attackRestrictions = new List<string> { Default };
        public string[] attackRestrictionsToIgnore = { };
        public int maxAttacks = 1;

        public CardRestriction defenderRestriction;

        public GameCard Card => RestrictionContext.source;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);

            if (attackRestrictions.Contains(Default)) attackRestrictions.AddRange(DefaultAttackRestrictions);
            attackRestrictions.RemoveAll(attackRestrictionsToIgnore.Contains);

            defenderRestriction?.Initialize(restrictionContext);

            //Debug.Log($"Finished initializing attack restriction for {Card.CardName} with restrictions: {string.Join(", ", attackRestrictions)}");
        }

        private bool RestrictionValid(string restriction, GameCard defender, IStackable stackSrc) => restriction switch
        {
            Default => true,

            ThisIsCharacter => Card.CardType == 'C',
            DefenderIsCharacter => defender.CardType == 'C',
            DefenderIsAdjacent => Card.IsAdjacentTo(defender),
            DefenderIsEnemy => Card.Controller != defender.Controller,
            FriendlyTurn => Card.Controller == Card.Game.TurnPlayer,
            MaxPerTurn => Card.AttacksThisTurn < maxAttacks,
            NothingHappening => Card.Game.NothingHappening,

            ThisIsActive => Card.Activated,
            InPlay => Card.Location == CardLocation.Board,
            NotNormally => stackSrc != null,

            _ => throw new System.ArgumentException($"Could not understand attack restriction {restriction}"),
        };

        /* This exists to debug a card's attack restriction,
         * but should not be usually used because it prints a ton whenever
         * the game checks to see if a person has a response.
        private bool RestrictionValidWithDebug(string restriction, GameCard defender)
        {
            bool valid = RestrictionValid(restriction, defender);
            //if (!valid) Debug.LogWarning($"{Card.CardName} cannot attack {defender} because it flouts the attack restriction {restriction}");
            return valid;
        } */

        private bool IsGameSetUp() => Card != null && Card.Game != null;

        public bool IsValidAttack(GameCard defender, IStackable stackSrc)
        {
            ComplainIfNotInitialized();
            return IsGameSetUp() && defender != null && attackRestrictions.All(r => RestrictionValid(r, defender, stackSrc));
        }

        /// <summary>
        /// Checks to see if this card could attack, if there were to ever be a valid attack target.
        /// <br></br> Used to check if the card should have a visual indicator of "Can Attack"
        /// </summary>
        /// <returns><see langword="true"/> If this character can attack at all, 
        /// <see langword="false"/> otherwise.</returns>
        public bool CouldAttackValidTarget(IStackable stackSrc)
        {
            ComplainIfNotInitialized();
            return IsGameSetUp() && attackRestrictions.Intersect(AtAllRestrictions).All(r => RestrictionValid(r, null, stackSrc));
        }

        /// <summary>
        /// Checks to see if this card can currently attack (any card).
        /// </summary>
        /// <returns><see langword="true"/> If any card in the game fits this card's atack restriction, 
        /// <see langword="false"/> otherwise.</returns>
        public bool CanAttackAnyCard(IStackable stackSrc)
        {
            ComplainIfNotInitialized();
            return IsGameSetUp() && Card.Game.boardCtrl.ExistsCardOnBoard(c => IsValidAttack(c, stackSrc));
        }
    }
}