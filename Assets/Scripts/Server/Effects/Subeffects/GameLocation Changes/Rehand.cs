using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Rehand : CardChangeState
    {
        protected override CardLocation destination => CardLocation.Hand;

        protected override void Move(GameCard card) => card.Rehand(card.Owner, Effect);
    }
}