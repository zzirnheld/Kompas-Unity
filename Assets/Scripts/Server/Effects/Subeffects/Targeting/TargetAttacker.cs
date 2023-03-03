using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAttacker : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CurrentContext.stackableCause is KompasCore.Effects.Attack attack)
            {
                ServerEffect.AddTarget(attack.attacker);
                return Task.FromResult(ResolutionInfo.Next);
            }
            else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}