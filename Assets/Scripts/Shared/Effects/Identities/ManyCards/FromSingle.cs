using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyCards
{
	public class FromSingle : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
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