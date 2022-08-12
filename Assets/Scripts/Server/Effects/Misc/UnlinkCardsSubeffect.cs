using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class UnlinkCardsSubeffect : ServerSubeffect
    {
        public int cardLinkIndex = -1;

        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.DestroyCardLink(cardLinkIndex);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}