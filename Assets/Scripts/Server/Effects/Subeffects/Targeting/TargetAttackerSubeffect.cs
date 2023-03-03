using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class TargetAttackerSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CurrentContext.stackableCause is Attack attack)
            {
                ServerEffect.AddTarget(attack.attacker);
                return Task.FromResult(ResolutionInfo.Next);
            }
            else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}