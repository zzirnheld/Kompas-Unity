using KompasCore.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AutoTargetCardIdentitySubeffect : ServerSubeffect
    {
        public INoActivationContextCardIdentity subeffectCardIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            subeffectCardIdentity.Initialize(initializationContext: DefaultRestrictionContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var card = subeffectCardIdentity.Card;
            if (card == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            Effect.AddTarget(card.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}