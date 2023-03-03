using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class AutoTargetCardIdentity : ServerSubeffect
    {
        public IActivationContextIdentity<GameCardBase> subeffectCardIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            subeffectCardIdentity.Initialize(initializationContext: DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var card = subeffectCardIdentity.From(CurrentContext, default);
            if (card == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            Effect.AddTarget(card.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}