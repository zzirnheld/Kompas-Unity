using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByTargetValueSubeffect : ServerSubeffect
{
    public const string Cost = "Cost";
    public const string S = "S";
    public const string W = "W";
    public const string C = "C";
    public const string DistanceToTarget = "Distance to Target";

    public string WhatToCount;

    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    private int BaseCount
    {
        get
        {
            switch (WhatToCount)
            {
                case Cost:
                    return Target.Cost;
                case S:
                    if (Target is CharacterCard charSTarget) return charSTarget.S;
                    else throw new System.ArgumentException($"Asked for S of non-character target");
                case W:
                    if (Target is CharacterCard charWTarget) return charWTarget.W;
                    else throw new System.ArgumentException($"Asked for W of non-character target");
                case C:
                    if (Target is SpellCard spellCTarget) return spellCTarget.C;
                    else throw new System.ArgumentException($"Asked for C of non-spell target");
                case DistanceToTarget:
                    return Source.DistanceTo(Target);
                default:
                    throw new System.ArgumentException($"Invalid 'what to count' string {WhatToCount} in x by gamestate value subeffect");
            }
        }
    }

    protected int Count => (BaseCount * Multiplier / Divisor) + Modifier;

    public override void Resolve()
    {
        Effect.X += Count;
        ServerEffect.ResolveNextSubeffect();
    }
}
