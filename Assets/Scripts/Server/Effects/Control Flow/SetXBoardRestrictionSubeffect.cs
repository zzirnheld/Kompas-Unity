using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXBoardRestrictionSubeffect : SetXSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(DefaultRestrictionContext);
        }

        public override int BaseCount
            => Game.boardCtrl.CardsAndAugsWhere(c => cardRestriction.IsValidCard(c, CurrentContext)).Count;
    }
}
