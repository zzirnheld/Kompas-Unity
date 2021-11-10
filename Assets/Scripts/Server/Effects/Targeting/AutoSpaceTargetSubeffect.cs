using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class AutoSpaceTargetSubeffect : ServerSubeffect
    {
        public SpaceRestriction spaceRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceRestriction ??= new SpaceRestriction();
            spaceRestriction.Initialize(this);
        }
        public IEnumerable<Space> ValidSpaces => Space.Spaces
                .Where(s => spaceRestriction.Evaluate(s, Context, theoreticalTarget: Target))
                .Select(s => Player.SubjectiveCoords(s));

        public override Task<ResolutionInfo> Resolve()
        {
            try
            {
                Space potentialTarget = Space.Spaces.Single(s => spaceRestriction.Evaluate(s, Context));
                ServerEffect.AddSpace(potentialTarget);
                return Task.FromResult(ResolutionInfo.Next);
            }
            catch (System.InvalidOperationException)
            {
                Debug.LogError($"More than one space fit the space restriction {spaceRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}");
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
            }
        }
    }
}