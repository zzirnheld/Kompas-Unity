using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetDirectionSubeffect : ServerSubeffect
    {
        public int secondarySpaceIndex = -2;

        public override Task<ResolutionInfo> Resolve()
        {
            var secondarySpace = Effect.GetSpace(secondarySpaceIndex);

            if (Space == null || secondarySpace == null)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

            var displacement = secondarySpace.DirectionFromThisTo(Space);
            Debug.Log($"Displacement from {secondarySpace} to {Space} is {displacement}");

            Effect.AddSpace(displacement);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}