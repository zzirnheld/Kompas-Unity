using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Space
{
    public const int BoardLen = 7;
    public const int MaxIndex = BoardLen - 1;

    public int x;
    public int y;

    public Space(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Space NearCorner => new Space(0, 0);
    public static Space FarCorner => new Space(MaxIndex, MaxIndex);

    public bool Valid => x >= 0 && y >= 0 && x < BoardLen && y < BoardLen;
    public bool IsCorner => (x == 0 || x == MaxIndex) && (y == 0 || y == MaxIndex);
    public bool IsEdge => x == 0 || x == MaxIndex || y == 0 || y == MaxIndex;

    public int Index => BoardLen * x + y;
    public Space Inverse => new Space(MaxIndex - x, MaxIndex - y);

    public int TaxicabDistanceTo(Space other) => Math.Abs(x - other.x) + Math.Abs(y - other.y);
    public int RadialDistanceTo(Space other) => Math.Max(Math.Abs(x - other.x), Math.Abs(y - other.y));
    public int DistanceTo(Space other) => TaxicabDistanceTo(other);

    public bool AdjacentTo(Space other) => DistanceTo(other) == 1;
    public IEnumerable<Space> AdjacentSpaces
    {
        get
        {
            List<Space> list = new List<Space>();
            var offsets = new int[] { -1, 1 };
            var x = this.x;
            var y = this.y;
            var xs = offsets.Select(o => o + x);
            var ys = offsets.Select(o => o + y);
            foreach (var xCoord in xs)
            {
                Space s = (xCoord, y);
                if (s.Valid) list.Add(s);
            }
            foreach (var yCoord in ys)
            {
                Space s = (x, yCoord);
                if (s.Valid) list.Add(s);
            }
            //Debug.Log($"Spaces adjacent to {this} are {string.Join(", ", list.Select(s => s.ToString()))}");
            return list;
        }
    }

    public bool SameColumn(Space other) => x - y == other.x - other.y;
    public bool SameDiagonal(Space other) => x == other.x || y == other.y;

    public bool NorthOf(Space other) => x > other.x || y > other.y;
    public Space DueNorth => new Space(x + 1, y + 1);

    public Space DirectionFromThisTo(Space other)
    {
        (int x, int y) diff = (x - other.x, y - other.y);
        if (diff.x == 0 || diff.y == 0) return (Math.Sign(diff.x), Math.Sign(diff.y));
        else if (diff.x % diff.y == 0) return (Math.Sign(diff.x) * (diff.x / diff.y), Math.Sign(diff.y));
        else if (diff.y % diff.x == 0) return (Math.Sign(diff.x), Math.Sign(diff.y) * (diff.y / diff.x));
        else return diff;
    }

    public static Space operator *(Space s, int i) => (s.x * i, s.y * i);

    public static bool operator ==(Space a, Space b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Space a, Space b) => !(a == b);
    public static bool operator ==(Space a, (int x, int y) b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Space a, (int x, int y) b) => !(a == b);
    public static bool operator ==((int x, int y) a, Space b) => a.x == b.x && a.y == b.y;
    public static bool operator !=((int x, int y) a, Space b) => !(a == b);

    public static implicit operator (int x, int y)(Space space) => (space.x, space.y);
    public static implicit operator Space((int x, int y) s) => new Space(s.x, s.y);

    public void Deconstruct(out int xCoord, out int yCoord)
    {
        xCoord = x;
        yCoord = y;
    }

    public override bool Equals(object obj) => obj is Space spc && this == spc;
    public override string ToString() => $"{x}, {y}";
    public override int GetHashCode() => x + BoardLen * y;
}
