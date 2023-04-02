using KompasCore.Effects;

namespace KompasServer.Effects.Subeffects
{
    public class SetXBoardRestriction: SetX
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        public override int BaseCount
            => Game.BoardController.CardsAndAugsWhere(c => cardRestriction.IsValidCard(c, ResolutionContext)).Count;
    }
}
