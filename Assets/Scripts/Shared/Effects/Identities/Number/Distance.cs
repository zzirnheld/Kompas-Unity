using System;

namespace KompasCore.Effects.Identities.Numbers
{
	public class Distance : ContextualParentIdentityBase<int>
	{
		public IIdentity<Space> firstSpace;
		public IIdentity<Space> secondSpace;

		public IRestriction<Space> throughRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstSpace.Initialize(initializationContext);
			secondSpace.Initialize(initializationContext);

			throughRestriction?.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			Space first = firstSpace.From(context, secondaryContext);
			Space second = secondSpace.From(context, secondaryContext);

			if (first == null || second == null) return -1;

			if (throughRestriction == null) return first.DistanceTo(second);

			var contextToConsider = base.secondaryContext ? secondaryContext : context;
			Func<Space, bool> predicate = s => throughRestriction.IsValid(s, contextToConsider);
			return InitializationContext.game.BoardController.ShortestPath(first, second, predicate);
		}
	}
}