using KompasCore.Effects;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class SpaceTargetSubeffect : ServerSubeffect
    {
        public SpaceRestriction spaceRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceRestriction.Initialize(this);
        }

        public override async Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.serverGame.ExistsSpaceTarget(spaceRestriction))
            {
                List<(int, int)> spaces = new List<(int, int)>();
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        var space = Player.SubjectiveCoords((x, y));
                        if(spaceRestriction.Evaluate((x, y))) spaces.Add(space);
                    }
                }

                var (a, b) = (-1, -1);
                while (!SetTargetIfValid(a, b))
                {
                    (a, b) = await ServerPlayer.serverAwaiter.GetSpaceTarget(Source.CardName, spaceRestriction.blurb, spaces.ToArray());
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
            if (spaceRestriction.Evaluate(x, y))
            {
                Debug.Log($"Adding {x}, {y} as coords");
                ServerEffect.coords.Add((x, y));
                ServerPlayer.ServerNotifier.AcceptTarget();
                return true;
            }
            //else Debug.LogError($"{x}, {y} not valid for restriction {spaceRestriction}");

            return false;
        }
    }
}