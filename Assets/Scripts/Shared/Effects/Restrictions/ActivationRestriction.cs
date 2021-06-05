using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class ActivationRestriction
    {
        public Effect Effect { get; private set; }
        public GameCard Card => Effect.Source;

        public const string Never = "Never";

        public const string TimesPerTurn = "Max Times Per Turn";
        public const string TimesPerRound = "Max Times Per Round";
        public const string FriendlyTurn = "Friendly Turn";
        public const string EnemyTurn = "Enemy Turn";
        public const string InPlay = "In Play";
        public const string Location = "Location";
        public const string ControllerActivates = "Controller Activates";
        public const string NotNegated = "Not Negated";
        public const string CardExists = "Card Exists";
        public const string ThisFitsRestriction = "This Card Fits Restriction";
        public const string NotCurrentlyActivated = "Not Currently Activated";

        public const string Default = "Default";
        public static readonly string[] DefaultRestrictions = { ControllerActivates, NotNegated, InPlay, NotCurrentlyActivated };

        public static readonly string[] AtAllRestrictions = 
            { TimesPerTurn, TimesPerRound, FriendlyTurn, EnemyTurn, NotNegated, InPlay, Location, ThisFitsRestriction, NotCurrentlyActivated };

        public int maxTimes = 1;
        public int location = (int) CardLocation.Field;
        public CardRestriction existsRestriction;
        public CardRestriction thisCardRestriction;

        private readonly List<string> ActivationRestrictions = new List<string>();
        public string[] activationRestrictionArray = null;

        public void Initialize(Effect eff)
        {
            Effect = eff;

            if(activationRestrictionArray == null) ActivationRestrictions.Add(Never);
            else
            {
                ActivationRestrictions.AddRange(activationRestrictionArray);
                if (activationRestrictionArray.Contains("Default"))
                    ActivationRestrictions.AddRange(DefaultRestrictions);

                existsRestriction?.Initialize(eff.Source, eff);
                thisCardRestriction?.Initialize(eff.Source, eff);

                Debug.Log($"Initializing activation restriction for {Card.CardName} " +
                    $"with restrictions: {string.Join(", ", ActivationRestrictions)}");
            }
        }

        private bool RestrictionValid(string r, Player activator)
        {
            if(Card == null || Card.Game == null)
            {
                //stuff is still getting set up
                Debug.LogWarning($"checked actvation restriction while card or card's game is null");
                return false;
            }

            switch (r)
            {
                case Never: return false;
                case Default: return true;
                case TimesPerTurn: return Effect.TimesUsedThisTurn < maxTimes;
                case TimesPerRound: return Effect.TimesUsedThisRound < maxTimes;
                case FriendlyTurn: return Effect.Game.TurnPlayer == activator;
                case EnemyTurn: return Effect.Game.TurnPlayer != activator;
                case InPlay: return Effect.Source.Location == CardLocation.Field;
                case Location: return Effect.Source.Location == (CardLocation)location;
                case ControllerActivates: return activator == Card.Controller;
                case NotNegated: return !Effect.Negated;
                case CardExists: return Effect.Game.Cards.Any(existsRestriction.Evaluate);
                case ThisFitsRestriction: return thisCardRestriction.Evaluate(Card);
                case NotCurrentlyActivated: return !Effect.Game.StackEntries.Any(e => e == Effect);
                default: throw new System.ArgumentException($"Invalid activation restriction {r}");
            }
        }

        /* This exists to debug a card's activation restriction,
         * but should not be usually used because it prints a ton whenever
         * a card's effect buttons are considered, or when the game checks to see if a person has a response.
        public bool RestrictionValidWithDebug(string restriction, Player activator)
        {
            bool valid = RestrictionValid(restriction, activator);
            if (!valid) Debug.Log($"Card {Card.CardName} effect # {Effect.EffectIndex} activation restriction " +
                $"flouts restriction {restriction} for activator {activator.index}");
            return valid;
        } */

        public bool Evaluate(Player activator)
            => ActivationRestrictions.All(r => RestrictionValid(r, activator));

        public bool EvaluateAtAll(Player activator)
            => ActivationRestrictions.Intersect(AtAllRestrictions).All(r => RestrictionValid(r, activator));
    }
}