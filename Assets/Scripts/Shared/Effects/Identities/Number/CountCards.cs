using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Restrictions.CardRestrictionElements;

namespace KompasCore.Effects.Identities.Numbers
{

	public class CountCards : ContextualParentIdentityBase<int>
	{
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new ManyCards.All();

		public IRestriction<GameCardBase> cardRestriction = new AlwaysValid();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext).Count(c => cardRestriction.IsValid(c, default));
	}
}