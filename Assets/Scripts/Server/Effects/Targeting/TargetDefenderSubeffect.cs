using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetDefenderSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.stackableCause is Attack attack)
            {
                ServerEffect.AddTarget(attack.defender);
                return Task.FromResult(ResolutionInfo.Next);
            }
            else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}