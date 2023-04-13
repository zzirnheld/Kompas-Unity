﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions.elements;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class SpaceTarget : ServerSubeffect
    {
        public SpaceRestriction spaceRestriction;

        private bool ForPlay => spaceRestriction.elements.Any(elem => elem is CanPlayCard);

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceRestriction.Initialize(DefaultInitializationContext);
        }

        public IEnumerable<Space> ValidSpaces => Space.Spaces
                .Where(s => spaceRestriction.IsValidSpace(s, ResolutionContext, theoreticalTarget: CardTarget))
                .Select(s => PlayerTarget.SubjectiveCoords(s));

        public override bool IsImpossible() => ValidSpaces.Count() == 0;

        /// <summary>
        /// Whether this space target subeffect will be valid if the given theoretical target is targeted.
        /// </summary>
        /// <param name="theoreticalTarget">The card to theoretically be targeted.</param>
        /// <returns><see langword="true"/> if there's a valid space,
        /// assuming you pick <paramref name="theoreticalTarget"/>,
        /// <see langword="false"/> otherwise</returns>
        public bool WillBePossibleIfCardTargeted(GameCard theoreticalTarget)
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if (spaceRestriction.IsValidSpace((x, y), ResolutionContext, theoreticalTarget)) return true;
                }
            }

            return false;
        }

        public override async Task<ResolutionInfo> Resolve()
        {
            var spaces = ValidSpaces.Select(s => (s.x, s.y)).ToArray();
            var recommendedSpaces
                = ForPlay
                ? spaces.Where(s => CardTarget.PlayRestriction.IsRecommendedPlay(s, PlayerTarget, ResolutionContext, normal: false)).ToArray()
                : spaces;
            if (spaces.Length > 0)
            {
                var (a, b) = (-1, -1);
                while (!SetTargetIfValid(a, b))
                {
                    (a, b) = await ServerPlayer.awaiter.GetSpaceTarget(Source.CardName, spaceRestriction.blurb, spaces, recommendedSpaces);
                    if ((a, b) == (-1, -1) && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
                }
                return ResolutionInfo.Next;
            }
            else
            {
                Debug.Log($"No valid coords exist for {ThisCard.CardName} effect");
                return ResolutionInfo.Impossible(NoValidSpaceTarget);
            }
        }

        public bool SetTargetIfValid(int x, int y)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (Space.IsValidSpace(x, y) && spaceRestriction.IsValidSpace((x, y), ResolutionContext))
            {
                Debug.Log($"Adding {x}, {y} as coords");
                ServerEffect.AddSpace((x, y));
                ServerPlayer.notifier.AcceptTarget();
                return true;
            }
            //else Debug.LogError($"{x}, {y} not valid for restriction {spaceRestriction}");

            return false;
        }
    }
}