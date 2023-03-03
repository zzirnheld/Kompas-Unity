using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class AnnihilateSubeffect : CardChangeStateSubeffect
    {
        protected override CardLocation destination => CardLocation.Annihilation;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Annihilate(Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}