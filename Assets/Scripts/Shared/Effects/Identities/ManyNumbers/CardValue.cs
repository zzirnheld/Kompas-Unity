using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ManyNumbers
{
	public class FromCardValue : ContextualParentIdentityBase<IReadOnlyCollection<int>>
	{
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards;
		public CardValue cardValue;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<int> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext)
					.Select(c => cardValue.GetValueOf(c))
					.ToArray();
	}
}