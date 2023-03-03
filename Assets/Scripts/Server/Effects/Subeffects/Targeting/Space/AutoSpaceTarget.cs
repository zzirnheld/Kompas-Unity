using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class AutoSpaceTarget : ServerSubeffect
    {
        public SpaceRestriction spaceRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            spaceRestriction.Initialize(DefaultInitializationContext);
        }
        public IEnumerable<Space> ValidSpaces => Space.Spaces
                .Where(s => spaceRestriction.IsValidSpace(s, CurrentContext, theoreticalTarget: CardTarget))
                .Select(s => PlayerTarget.SubjectiveCoords(s));

        public override Task<ResolutionInfo> Resolve()
        {
            try
            {
                Space potentialTarget = Space.Spaces.Single(s => spaceRestriction.IsValidSpace(s, CurrentContext));
                ServerEffect.AddSpace(potentialTarget);
                return Task.FromResult(ResolutionInfo.Next);
            }
            catch (System.InvalidOperationException ioe)
            {
                Debug.LogError($"Zero, or more than one space fit the space restriction {spaceRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}. Exception {ioe}");
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
            }
        }
    }
}