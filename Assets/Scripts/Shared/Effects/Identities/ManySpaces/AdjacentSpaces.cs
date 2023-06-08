using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManySpaces
{
	public class AdjacentSpaces : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		public IIdentity<Space> adjacentTo;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			adjacentTo.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> adjacentTo.From(context, secondaryContext).AdjacentSpaces;
	}
}