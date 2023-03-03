using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffect
{
    public class PlaySubeffect : CardChangeStateSubeffect
    {
        protected override CardLocation destination => CardLocation.Board;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            Debug.Log($"Stack source is {Effect}");
            CardTarget.Play(SpaceTarget, PlayerTarget, stackSrc: Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}