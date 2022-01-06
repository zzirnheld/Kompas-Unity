using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class SetXByGamestateSubeffect : SetXSubeffect
    {
        public const string HandSize = "Hand Size";
        public const string HandSizeLimit = "Hand Size Limit";
        public const string DistanceToCoordsThrough = "Distance to Coords Through";
        public const string DistanceBetweenTargetAndCoords = "Distance Between Target and Target Space";
        public const string DistanceFromSourceToTarget = "Distance From Source to Target";

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
            cardValue?.Initialize(eff.Source);
        }

        public override int BaseCount
        {
            get
            {
                return whatToCount switch
                {
                    HandSize                => PlayerTarget.handCtrl.HandSize,
                    HandSizeLimit           => PlayerTarget.HandSizeLimit,
                    DistanceToCoordsThrough => Game.boardCtrl.ShortestPath(Source, SpaceTarget, throughRestriction, Context),
                    DistanceBetweenTargetAndCoords => CardTarget.DistanceTo(SpaceTarget),
                    DistanceFromSourceToTarget     => Source.DistanceTo(CardTarget),

                    CardsFittingRestriction 
                        => Game.Cards.Where(c => cardRestriction.Evaluate(c, Context)).Count(),
                    TotalCardValueOfCardsFittingRestriction 
                        => Game.Cards.Where(c => cardRestriction.Evaluate(c, Context)).Sum(cardValue.GetValueOf),
                    MaxCardValueOfCardsFittingRestriction 
                        => Game.Cards.Where(c => cardRestriction.Evaluate(c, Context)).Max(cardValue.GetValueOf),

                    EffectUsesThisTurn  => Effect.TimesUsedThisTurn,
                    NumberOfTargets     => Effect.CardTargets.Count(),

                    _ => throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect"),
                };
            }
        }
    }
}