using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class PayPipsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            int toPay = Count;
            if (Player.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

            Player.Pips -= toPay;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}