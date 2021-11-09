using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringStackableSourceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.Stackable == null) throw new KompasException("Null stackable", string.Empty);

            ServerEffect.AddTarget(Context.Stackable.Source);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}