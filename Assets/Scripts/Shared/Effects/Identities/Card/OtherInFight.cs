using KompasCore.Cards;
using KompasCore.Exceptions;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Cards
{
	public class OtherInFight : ContextualParentIdentityBase<GameCardBase>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> other;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override GameCardBase AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			Attack attack = GetAttack(ContextToConsider(context, secondaryContext).TriggerContext);
			var otherCard = other.From(context, secondaryContext);

			if (attack.attacker == otherCard) return attack.defender;
			if (attack.defender == otherCard) return attack.attacker;

			throw new NullCardException($"Neither card of attack {attack} was {otherCard}");
		}
	}
}