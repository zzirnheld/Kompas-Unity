using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetDefender : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CurrentContext.stackableCause is KompasCore.Effects.Attack attack)
            {
                ServerEffect.AddTarget(attack.defender);
                return Task.FromResult(ResolutionInfo.Next);
            }
            else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}