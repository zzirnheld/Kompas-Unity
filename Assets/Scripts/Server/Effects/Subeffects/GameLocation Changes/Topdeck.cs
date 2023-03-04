using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Topdeck : CardChangeState
    {
        public override bool IsImpossible() => CardTarget == null;
        protected override CardLocation destination => CardLocation.Deck;

        protected override void Move(GameCard card) => card.Topdeck(card.Owner, Effect);
    }
}