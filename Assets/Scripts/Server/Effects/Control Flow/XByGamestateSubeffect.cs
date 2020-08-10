using KompasCore.Effects;
using KompasServer.GameCore;
using System.Linq;

namespace KompasServer.Effects
{
    public abstract class XByGamestateSubeffect : ServerSubeffect
    {
        public const string HandSize = "Hand Size";
        public const string DistanceToCoordsThrough = "Distance to Coords Through";
        public const string CardsFittingRestriction = "Cards Fitting Restriction";

        public string whatToCount;

        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public int PlayerIndex = 0;
        public ServerPlayer Player
        {
            get
            {
                if (PlayerIndex == 0) return ServerEffect.ServerController;
                else return ServerEffect.ServerController.ServerEnemy;
            }
        }

        public CardRestriction throughRestriction = new CardRestriction();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            throughRestriction.Initialize(this);
        }

        private int BaseCount
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
                    default:
                        throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
                }
            }
        }

        protected int Count { get { return BaseCount * multiplier / divisor + modifier; } }
    }
}