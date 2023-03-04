using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Discard : CardChangeState
    {
        protected override CardLocation destination => CardLocation.Discard;

        protected override void Move(GameCard card) => card.Discard(Effect);
    }
}