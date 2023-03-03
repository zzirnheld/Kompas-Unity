using KompasCore.Effects;

namespace KompasServer.Effects.Subeffect
{
    public class SetXBoardRestrictionSubeffect : SetXSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        public override int BaseCount
            => Game.BoardController.CardsAndAugsWhere(c => cardRestriction.IsValidCard(c, CurrentContext)).Count;
    }
}
