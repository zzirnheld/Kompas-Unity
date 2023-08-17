using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyCards
{
	public class Concat : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase>[] cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var i in cards) i.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.Select(s => s.From(context, secondaryContext)).ToArray();
	}
}