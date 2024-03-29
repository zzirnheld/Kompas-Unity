using System.Collections.Generic;
using System.Linq;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	/// <summary>
	/// Gets the distance between the described origin point and the space to be tested,
	/// gets the described number,
	/// and compares the distance to the number with the given comparison.
	/// </summary>
	public class CompareDistance : SpaceRestrictionElement
	{
		[JsonProperty]
		public bool shortestEmptyPath = false;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> distanceTo;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> number;
		[JsonProperty]
		public INumberRelationship comparison = new Relationships.NumberRelationships.Equal();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			distanceTo.Initialize(initializationContext);
			number.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var origin = this.distanceTo.From(context);
			int distance = shortestEmptyPath
				? InitializationContext.game.BoardController.ShortestEmptyPath(origin, space)
				: origin.DistanceTo(space);

			int number = this.number.From(context);

			return comparison.Compare(distance, number);
		}
	}

	public class Towards : SpaceRestrictionElement
	{
		//Whether the space to be tested's distance to the destination
		//is closer than other's distance to the destination
		public IIdentity<Space> destination;
		public IIdentity<Space> origin;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space item, IResolutionContext context)
		{
			var destination = this.destination.From(context);
			return destination.DistanceTo(item) < destination.DistanceTo(origin.From(context));
		}
	}

	public class TowardsAny : SpaceRestrictionElement
	{
		public IIdentity<IReadOnlyCollection<Space>> anyDestination;
		public IRestriction<Space> anyDestinationRestriction;
		public IIdentity<Space> origin;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			anyDestination ??= new Identities.ManySpaces.Restricted() { restriction = anyDestinationRestriction };
			anyDestination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space item, IResolutionContext context)
		{
			var origin = this.origin.From(context);
			var destinations = anyDestination.From(context);
			return destinations.Any(destination => destination.DistanceTo(item) < destination.DistanceTo(origin));
		}
	}

	public class AwayFrom : SpaceRestrictionElement
	{
		//Whether the space to be tested's distance to the destination
		//is further than other's distance to the destination
		public IIdentity<Space> destination;
		public IIdentity<Space> origin;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination.Initialize(initializationContext);
			origin.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space item, IResolutionContext context)
		{
			var destination = this.destination.From(context);
			return destination.DistanceTo(item) > destination.DistanceTo(origin.From(context));
		}
	}
}