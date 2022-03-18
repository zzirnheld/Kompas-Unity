using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class PlaySubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == CardLocation.Board;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            Debug.Log($"Stack source is {Effect}");
            CardTarget.Play(SpaceTarget, PlayerTarget, stackSrc: Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}