using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class XByGamestateSubeffect : ServerSubeffect
{
    public const string HandSize = "Hand Size";
    public const string DistanceToCoordsThrough = "Distance to Coords Through";

    public string WhatToCount;
    
    public int Multiplier = 1;
    public int Divisor = 1;
    public int Modifier = 0;

    public int PlayerIndex = 0;
    public ServerPlayer Player
    {
        get
        {
            if (PlayerIndex == 0) return ServerEffect.ServerController;
            else return ServerEffect.ServerController.ServerEnemy;
        }
    }

    public BoardRestriction ThroughRestriction = new BoardRestriction();

    private int BaseCount
    {
        get
        {
            switch (WhatToCount)
            {
                case HandSize:
                    return Player.handCtrl.HandSize;
                case DistanceToCoordsThrough:
                    var (x, y) = Space;
                    return Game.boardCtrl.ShortestPath(Source, x, y, ThroughRestriction);
                default:
                    throw new System.ArgumentException($"Invalid 'what to count' string {WhatToCount} in x by gamestate value subeffect");
            }
        }
    }

    protected int Count { get { return BaseCount * Multiplier / Divisor + Modifier; } }
}
