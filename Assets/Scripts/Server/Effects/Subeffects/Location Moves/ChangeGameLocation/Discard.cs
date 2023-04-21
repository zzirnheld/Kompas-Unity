using KompasCore.Cards;
using KompasCore.Cards.Movement;

namespace KompasServer.Effects.Subeffects
{
	public class Discard : ChangeGameLocation
	{
		protected override CardLocation destination => CardLocation.Discard;

		protected override void ChangeLocation(GameCard card) => card.Discard(Effect);
	}
}