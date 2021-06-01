using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Space
{
    public int x;
    public int y;

    public Space(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Valid => x >= 0 && y >= 0 && x < 7 && y < 7;
    public bool IsCorner => (x == 0 || x == 6) && (y == 0 || y == 6);

    public int Index => 7 * x + y;
    public Space Inverse => new Space(6 - x, 6 - y);

    public int DistanceTo(Space other) => Math.Max(Math.Abs(x - other.x), Math.Abs(y - other.y));

    public bool AdjacentTo(Space other) => DistanceTo(other) == 1;

    public bool SameColumn(Space other) => x - y == other.x - other.y;
    public bool SameDiagonal(Space other) => x == other.x || y == other.y;

    public bool NorthOf(Space other) => x > other.x || y > other.y;
    public Space DueNorth => new Space(x + 1, y + 1);


    public override string ToString() => $"{x}, {y}";

    public override bool Equals(object obj) => obj is Space spc && this == spc;
    public static bool operator ==(Space a, Space b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Space a, Space b) => !(a == b);
    public static bool operator ==(Space a, (int x, int y) b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Space a, (int x, int y) b) => !(a == b);
    public static bool operator ==((int x, int y) a, Space b) => a.x == b.x && a.y == b.y;
    public static bool operator !=((int x, int y) a, Space b) => !(a == b);
}
