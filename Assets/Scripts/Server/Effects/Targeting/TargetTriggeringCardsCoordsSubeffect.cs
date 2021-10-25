using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardsCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.CardInfo == null)
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerEffect.AddSpace(Context.CardInfo.Position.Copy);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}