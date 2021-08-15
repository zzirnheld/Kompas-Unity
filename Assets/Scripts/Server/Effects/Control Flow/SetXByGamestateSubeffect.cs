﻿using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class SetXByGamestateSubeffect : SetXSubeffect
    {
        public const string HandSize = "Hand Size";
        public const string DistanceToCoordsThrough = "Distance to Coords Through";

        public const string CardsFittingRestriction = "Cards Fitting Restriction";
        public const string TotalCardValueOfCardsFittingRestriction = "Total Card Value of Cards Fitting Restriction";
        public const string MaxCardValueOfCardsFittingRestriction = "Max Card Value of Cards Fitting Restriction";

        public const string EffectUsesThisTurn = "Effect Uses This Turn";
        public const string NumberOfTargets = "Number of Targets";

        public string whatToCount;

        public CardValue cardValue;

        public CardRestriction throughRestriction;
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            throughRestriction?.Initialize(this);
            cardRestriction?.Initialize(this);
        }

        public override int BaseCount
        {
            get
            {
                switch (whatToCount)
                {
                    case HandSize: return Player.handCtrl.HandSize;
                    case DistanceToCoordsThrough:
                        return Game.boardCtrl.ShortestPath(Source, Space, throughRestriction);

                    case CardsFittingRestriction:
                        return Game.Cards.Where(cardRestriction.Evaluate).Count();
                    case TotalCardValueOfCardsFittingRestriction:
                        return Game.Cards.Where(cardRestriction.Evaluate).Sum(cardValue.GetValueOf);
                    case MaxCardValueOfCardsFittingRestriction:
                        return Game.Cards.Where(cardRestriction.Evaluate).Max(cardValue.GetValueOf);

                    case EffectUsesThisTurn: return Effect.TimesUsedThisTurn;
                    case NumberOfTargets: return Effect.Targets.Count();
                    default:
                        throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
                }
            }
        }
    }
}