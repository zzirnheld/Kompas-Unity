using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasServer.Effects.Subeffects
{
    public class SetXBoardRestriction: SetX
    {
        public IRestriction<GameCardBase> cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        public override int BaseCount
            => Game.BoardController.CardsAndAugsWhere(c => cardRestriction.IsValid(c, ResolutionContext)).Count;
    }
}
