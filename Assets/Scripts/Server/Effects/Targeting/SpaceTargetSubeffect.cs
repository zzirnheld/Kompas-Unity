using KompasCore.Effects;
using System.Collections.Generic;
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

        public override bool Resolve()
        {
            if (ServerEffect.serverGame.ExistsSpaceTarget(spaceRestriction))
            {
                List<(int, int)> spaces = new List<(int, int)>();
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        var space = Controller.SubjectiveCoords((x, y));
                        if(spaceRestriction.Evaluate((x, y))) spaces.Add(space);
                    }
                }
                EffectController.ServerNotifier.GetSpaceTarget(Source.CardName, spaceRestriction.blurb, spaces.ToArray());
                return false;
            }
            else
            {
                Debug.Log($"No valid coords exist for {ThisCard.CardName} effect");
                return ServerEffect.EffectImpossible();
            }
        }

        public bool SetTargetIfValid(int x, int y)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (spaceRestriction.Evaluate(x, y))
            {
                Debug.Log($"Adding {x}, {y} as coords");
                ServerEffect.Coords.Add((x, y));
                EffectController.ServerNotifier.AcceptTarget();
                ServerEffect.ResolveNextSubeffect();
                return true;
            }
            else Debug.LogError($"{x}, {y} not valid for restriction {spaceRestriction}");

            return false;
        }
    }
}