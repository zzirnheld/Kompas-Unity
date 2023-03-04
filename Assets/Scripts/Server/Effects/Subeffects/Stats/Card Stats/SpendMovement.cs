using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class SpendMovement : ServerSubeffect
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