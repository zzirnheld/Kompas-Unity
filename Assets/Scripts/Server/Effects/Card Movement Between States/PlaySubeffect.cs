using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class PlaySubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == CardLocation.Field;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            CardTarget.Play(SpaceTarget, PlayerTarget, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}