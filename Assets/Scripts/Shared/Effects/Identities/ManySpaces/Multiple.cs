using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.ManySpaces
{
	public class Multiple : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space>[] spaces;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var i in spaces) i.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> spaces.Select(s => s.From(context, secondaryContext)).ToArray();
	}
}