using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Restrictions.GamestateRestrictionElements;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class AutoTargetSpaceIdentity : ServerSubeffect
	{
		public IIdentity<Space> subeffectSpaceIdentity;

		public IRestriction<Space> spaceRestriction = new AlwaysValid();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			subeffectSpaceIdentity.Initialize(initializationContext: DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			var space = subeffectSpaceIdentity.From(ResolutionContext, default);
			
			if (space == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
			if (!spaceRestriction.IsValid(space, ResolutionContext)) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			Effect.AddSpace(space);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}