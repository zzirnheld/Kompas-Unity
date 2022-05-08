using KompasServer.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AutoTargetCardIdentitySubeffect : ServerSubeffect
    {
        public SubeffectCardIdentity subeffectCardIdentity;

        public override Task<ResolutionInfo> Resolve()
        {
            var card = subeffectCardIdentity.GetCard();
            if (card == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            Effect.AddTarget(card.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}