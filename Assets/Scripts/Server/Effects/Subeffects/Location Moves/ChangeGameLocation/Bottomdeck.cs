using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Bottomdeck : ChangeGameLocation
    {
        public override bool IsImpossible() => CardTarget == null;
        protected override CardLocation destination => CardLocation.Deck;

        protected override void ChangeLocation(GameCard card) => card.Bottomdeck(card.Owner, Effect);
    }
}