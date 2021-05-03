using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class SetXByGamestateSubeffect : SetXSubeffect
    {
        public const string HandSize = "Hand Size";
        public const string DistanceToCoordsThrough = "Distance to Coords Through";
        public const string CardsFittingRestriction = "Cards Fitting Restriction";
        public const string EffectUsesThisTurn = "Effect Uses This Turn";
        public const string MaxStatAmongRestriction = "Max Stat Among Restriction";
        public const string NumberOfTargets = "Number of Targets";

        public string whatToCount;

        public string stat;
        public CardRestriction throughRestriction;
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            throughRestriction = throughRestriction ?? new CardRestriction();
            cardRestriction = cardRestriction ?? new CardRestriction();
            throughRestriction.Initialize(this);
            cardRestriction.Initialize(this);
        }

        public override int BaseCount
        {
            get
            {
                switch (whatToCount)
                {
                    case HandSize: return Player.handCtrl.HandSize;
                    case DistanceToCoordsThrough:
                        var (x, y) = Space;
                        return Game.boardCtrl.ShortestPath(Source, x, y, throughRestriction);
                    case CardsFittingRestriction:
                        return Game.Cards.Where(c => cardRestriction.Evaluate(c)).Count();
                    case EffectUsesThisTurn: return Effect.TimesUsedThisTurn;
                    case MaxStatAmongRestriction:
                        return Game.Cards.Where(c => cardRestriction.Evaluate(c)).Max(c => c.GetStat(stat));
                    case NumberOfTargets: return Effect.Targets.Count();
                    default:
                        throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
                }
            }
        }
    }
}