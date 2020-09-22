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

        public string whatToCount;

        public CardRestriction throughRestriction = new CardRestriction();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            throughRestriction.Initialize(this);
        }

        public override int BaseCount
        {
            get
            {
                switch (whatToCount)
                {
                    case HandSize:
                        return Player.handCtrl.HandSize;
                    case DistanceToCoordsThrough:
                        var (x, y) = Space;
                        return Game.boardCtrl.ShortestPath(Source, x, y, throughRestriction);
                    case CardsFittingRestriction:
                        return Game.Cards.Where(c => throughRestriction.Evaluate(c)).Count();
                    case EffectUsesThisTurn:
                        return Effect.TimesUsedThisTurn;
                    default:
                        throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
                }
            }
        }
    }
}