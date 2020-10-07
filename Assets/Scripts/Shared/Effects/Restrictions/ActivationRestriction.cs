using System.Collections.Generic;
using KompasCore.Cards;
using KompasServer.GameCore;
using System.Linq;
using UnityEngine;
using KompasClient.Effects;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class ActivationRestriction
    {
        public Effect Effect { get; private set; }
        public GameCard Card => Effect.Source;

        public const string TimesPerTurn = "Max Times Per Turn";
        public const string TimesPerRound = "Max Times Per Round";
        public const string FriendlyTurn = "Friendly Turn";
        public const string EnemyTurn = "Enemy Turn";
        public const string InPlay = "In Play";
        public const string Location = "Location";
        //public const string StackEmpty = "Stack Empty";
        public const string ControllerActivates = "Controller Activates";
        public const string NotNegated = "Not Negated";
        public const string CardExists = "Card Exists";

        public const string Default = "Default";
        public static readonly string[] DefaultRestrictions = { ControllerActivates, NotNegated, InPlay };

        public static readonly string[] AtAllRestrictions = { TimesPerTurn, TimesPerRound, FriendlyTurn, EnemyTurn, NotNegated, InPlay };

        public int maxTimes = 1;
        public int location = (int) CardLocation.Field;
        public CardRestriction existsRestriction = new CardRestriction();

        public List<string> activationRestrictions = new List<string>{ "Default" };

        public void Initialize(Effect eff)
        {
            Effect = eff;
            existsRestriction.Initialize(eff.Source, eff.Controller, eff);
            if (activationRestrictions.Contains("Default")) activationRestrictions.AddRange(DefaultRestrictions);
            Debug.Log($"Initializing activation restriction for {Card.CardName} with restrictions: {string.Join(", ", activationRestrictions)}");
            //Debug.Log($"Serialized version: {JsonUtility.ToJson(this)}");
        }

        private bool RestrictionValid(string r, Player activator)
        {
            //Debug.Log($"Considering activation restriction {r} for {Effect.Source.CardName}");
            if(Card == null || Card.Game == null)
            {
                //stuff is still getting set up
                Debug.LogWarning($"checked actvation restriction while card or card's game is null");
                return false;
            }

            switch (r)
            {
                case Default: return true;
                case TimesPerTurn: return Effect.TimesUsedThisTurn < maxTimes;
                case TimesPerRound: return Effect.TimesUsedThisRound < maxTimes;
                case FriendlyTurn: return Effect.Game.TurnPlayer == activator;
                case EnemyTurn: return Effect.Game.TurnPlayer != activator;
                case InPlay: return Effect.Source.Location == CardLocation.Field;
                case Location: return Effect.Source.Location == (CardLocation)location;
                //case StackEmpty: return Effect.Game.CurrStackEntry == null; //TODO make client game actually track current stack entry
                case ControllerActivates: return activator == Card.Controller;
                case NotNegated: return !Effect.Negated;
                case CardExists: return Effect.Game.Cards.Any(c => existsRestriction.Evaluate(c));
                default: throw new System.ArgumentException($"Invalid activation restriction {r}");
            }
        }

        public bool RestrictionValidWithDebug(string restriction, Player activator)
        {
            bool valid = RestrictionValid(restriction, activator);
            /*if (!valid) Debug.Log($"Card {Card.CardName} effect # {Effect.EffectIndex} activation restriction " +
                $"flouts restriction {restriction} for activator {activator.index}");*/
            return valid;
        }

        public bool Evaluate(Player activator)
            => activationRestrictions.All(r => RestrictionValidWithDebug(r, activator));

        public bool EvaluateAtAll(Player activator)
            => activationRestrictions.Intersect(AtAllRestrictions).All(r => RestrictionValidWithDebug(r, activator));
    }
}