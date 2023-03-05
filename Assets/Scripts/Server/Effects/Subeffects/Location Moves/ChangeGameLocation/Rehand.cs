using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
    public class Rehand : ChangeGameLocation
    {
        protected override CardLocation destination => CardLocation.Hand;

        protected override void ChangeLocation(GameCard card) => card.Rehand(card.Owner, Effect);
    }
}