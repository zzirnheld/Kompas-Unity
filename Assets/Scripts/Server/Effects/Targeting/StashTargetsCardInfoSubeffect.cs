using KompasCore.Cards;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class StashTargetsCardInfoSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(NoValidCardTarget);

            ServerEffect.cardInfoTargets.Add(GameCardInfo.CardInfoOf(CardTarget));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
