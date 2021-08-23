using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringStackableSourceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.Stackable == null)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerEffect.AddTarget(Context.Stackable.Source);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}