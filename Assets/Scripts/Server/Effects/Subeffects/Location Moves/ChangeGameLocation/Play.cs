using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Play : ChangeGameLocation
    {
        protected override CardLocation destination => CardLocation.Board;

        protected override void ChangeLocation(GameCard card) => card.Play(SpaceTarget, PlayerTarget, Effect);
    }
}