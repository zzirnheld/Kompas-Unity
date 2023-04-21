using KompasCore.Effects.Identities.Numbers;

namespace KompasServer.Effects.Subeffects
{
	public class SpendMovement : UpdateCardStats
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			spacesMoved = new EffectX() { multiplier = xMultiplier, modifier = xModifier, divisor = xDivisor };
			base.Initialize(eff, subeffIndex);
		}
	}
}