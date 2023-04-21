using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyNumbers
{
	public class Distances : ContextualParentIdentityBase<ICollection<int>>
	{
		public IIdentity<Space> origin;
		public IIdentity<IReadOnlyCollection<Space>> destinations;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			origin.Initialize(initializationContext);
			destinations.Initialize(initializationContext);
		}

		protected override ICollection<int> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var origin = this.origin.From(context, secondaryContext);
			var destinations = this.destinations.From(context, secondaryContext);
			return destinations.Select(dest => origin.DistanceTo(dest)).ToArray();
		}
	}
}