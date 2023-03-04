using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Play : CardChangeState
    {
        protected override CardLocation destination => CardLocation.Board;

        protected override void Move(GameCard card) => card.Play(SpaceTarget, PlayerTarget, Effect);
    }
}