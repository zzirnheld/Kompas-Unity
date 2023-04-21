using KompasCore.Effects.Selectors;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.Spaces
{

	public class SelectFromMany : ContextualParentIdentityBase<Space>
	{
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		public ISelector<Space> selector;// = new RandomSelector<Space>();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> selector.Select(spaces.From(context, secondaryContext));
	}
}