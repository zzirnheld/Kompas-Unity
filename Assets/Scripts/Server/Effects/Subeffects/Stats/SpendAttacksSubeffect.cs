using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class SpendAttacksSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);

            CardTarget.AttacksThisTurn += Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
