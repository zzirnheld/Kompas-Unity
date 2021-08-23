using KompasCore.Cards;
using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetOtherInFightSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.Stackable is Attack attack)
            {
                GameCard newTarget = null;
                if (attack.attacker == Target) newTarget = attack.defender;
                else if (attack.defender == Target) newTarget = attack.attacker;

                if (newTarget != null)
                {
                    ServerEffect.AddTarget(newTarget);
                    return Task.FromResult(ResolutionInfo.Next);
                }
            }

            return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}