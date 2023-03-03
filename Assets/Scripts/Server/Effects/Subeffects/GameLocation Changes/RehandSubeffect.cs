using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class RehandSubeffect : CardChangeStateSubeffect
    {
        protected override CardLocation destination => CardLocation.Hand;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Rehand(CardTarget.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}