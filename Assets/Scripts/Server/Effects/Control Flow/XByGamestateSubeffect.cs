using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class XByGamestateSubeffect : ServerSubeffect
{
    public const string HandSize = "Hand Size";

    public string WhatToCount;
    
    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    public int PlayerIndex = 0;
    public Player Player
    {
        get
        {
            if (PlayerIndex == 0) return ServerEffect.ServerController;
            else return ServerEffect.ServerController.enemy;
        }
    }

    private int BaseCount
    {
        get
        {
            switch (WhatToCount)
            {
                case HandSize:
                    return Player.handCtrl.HandSize;
                default:
                    throw new System.ArgumentException($"Invalid 'what to count' string {WhatToCount} in x by gamestate value subeffect");
            }
        }
    }

    protected int Count { get { return BaseCount * Multiplier / Divisor + Modifier; } }
}
