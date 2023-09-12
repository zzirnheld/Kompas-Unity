using KompasCore.Effects.Identities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class ConnectedTo : SpaceRestrictionElement
	{
		[JsonProperty]
		public IIdentity<Space> space;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> anyOfTheseSpaces;
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> byRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (AllNull(space, spaces, anyOfTheseSpaces))
				throw new System.ArgumentNullException("spaces", "Failed to provide spaces for space restriction elements");

			spaces ??= new Identities.ManySpaces.Multiple() { spaces = new IIdentity<Space>[] { space } };
			spaces.Initialize(initializationContext);
			anyOfTheseSpaces?.Initialize(initializationContext);

			byRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			return spaces.From(context)
				.All(s => InitializationContext.game.BoardController.AreConnectedBySpaces(s, space, byRestriction, context));
		}
	}
}