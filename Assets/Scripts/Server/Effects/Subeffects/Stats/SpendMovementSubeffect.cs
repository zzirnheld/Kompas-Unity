using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class SpendMovementSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.SpacesCanMove < Count;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (CardTarget.SpacesCanMove < Count)
                return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

            CardTarget.SpacesMoved += Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}