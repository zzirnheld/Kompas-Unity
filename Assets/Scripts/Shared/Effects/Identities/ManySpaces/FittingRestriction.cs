using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.ManySpaces
{
	public class Restricted : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> restriction;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> spaces = new ManySpaces.All();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> spaces.From(context, secondaryContext)
				.Where(s => restriction.IsValid(s, InitializationContext.effect?.CurrentResolutionContext)).ToList();
	}
}