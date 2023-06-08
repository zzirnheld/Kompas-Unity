using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
	public class Defender : TriggerContextualCardIdentityBase
	{
		protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
			=> GetAttack(contextToConsider).defender;
	}
}