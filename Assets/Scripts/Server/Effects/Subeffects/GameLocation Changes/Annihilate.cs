using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Annihilate : CardChangeState
    {
        protected override CardLocation destination => CardLocation.Annihilation;

        protected override void Move(GameCard card) => card.Annihilate(Effect);
    }
}