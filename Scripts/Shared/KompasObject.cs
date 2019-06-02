using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KompasObject : MonoBehaviour {

    public Game game;
    
    public const float spacesInGrid = 7;
    public const float boardLenOffset = 0.45f;

    protected int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Divide by 0.9f because the range of accepted position values is 0 to 0.9f (0.45 - -0.45).
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)((pos + boardLenOffset) * spacesInGrid / 0.9f);
    }
    protected float GridIndexToPos(int gridIndex)
    {
        /* first, cast the index to a float to make sure the math works out.
         * then, divide by the grid length to board ratio to get a number (0,1) that makes
         * sense in the context of the board's local lenth of one.
         * then, subtract the board length offset to get a number that makes sense
         * in the actual board's context of values (-0.45, 0.45) (legal local coordinates)
         * finally, add 0.025 to account for the 0.05 space on either side of the legal 0.45 area
         */
        return (((float)gridIndex) / spacesInGrid - boardLenOffset + 0.025f);
    }

    public virtual void OnClick() { }
    public virtual void OnHover() { }
    public virtual void OnDrag(Vector3 mousePos) { }
    public virtual void OnDragEnd(Vector3 mousePos) { }
}
