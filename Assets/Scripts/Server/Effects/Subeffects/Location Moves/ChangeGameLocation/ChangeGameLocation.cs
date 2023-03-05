using System.Threading.Tasks;
using KompasCore.Cards;
using KompasCore.Exceptions;

namespace KompasServer.Effects.Subeffects
{
    /// <summary>
    /// Moves cards between discard/field/etc
    /// </summary>
    public abstract class ChangeGameLocation : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == destination;

        protected abstract CardLocation destination { get; }


        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            ChangeLocation(CardTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }

        protected abstract void ChangeLocation(GameCard card);
    }
}