using System;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class Behind : SpaceRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> card;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
			=> card.From(context).SpaceBehind(space);
	}
}