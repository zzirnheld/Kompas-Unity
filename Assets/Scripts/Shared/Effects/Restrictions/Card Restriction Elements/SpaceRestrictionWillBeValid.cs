using KompasCore.Cards;
using KompasServer.Effects.Subeffects;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class SpaceRestrictionWillBeValid : CardRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
		public int subeffectIndex;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> InitializationContext.effect.Subeffects[subeffectIndex] is SpaceTarget spaceTgtSubeff
					&& spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: card?.Card);
	}
}