using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardsCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.CardInfo == null) throw new NullCardException(TargetWasNull);
            else if (!Context.CardInfo.Position.Valid) throw new InvalidSpaceException(Context.CardInfo.Position, NoValidSpaceTarget);

            ServerEffect.AddSpace(Context.CardInfo.Position.Copy);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}