using KompasCore.Effects;
using KompasCore.Effects.Identities.Cards;
using KompasCore.Effects.Identities.Numbers;

namespace KompasServer.Effects.Subeffects
{
	public class PayPipsTargetCost : PayPips
	{
		public int multiplier = 1;
		public int modifier = 0;
		public int divisor = 1;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			pipCost = new FromCardValue() {
				cardValue = new CardValue() {
					value = CardValue.Cost,
					multiplier = multiplier,
					modifier = modifier,
					divisor = divisor
				},
				card = new TargetIndex()
			};
			base.Initialize(eff, subeffIndex);
		}
	}
}