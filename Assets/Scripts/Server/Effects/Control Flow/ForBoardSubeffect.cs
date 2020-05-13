using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//note for use: put this before the section to loop. 
//place a break after the looped section to break back to this. 
//fill in the value of "jump to when done" with the index of the subeffect after the looped section
//the looped card will be found as the last target (could simplify this whole thing by:
/* removing "running"
 * having a condition in any subeff that requires targeting that just uses the last target in the list
 */ 
public class ForBoardSubeffect : ServerSubeffect
{
    public int JumpToWhenDone;
    public BoardRestriction restriction;

    private int xCount = 0;
    private int yCount = 0;
    private bool running = false;

    public override void Resolve()
    {
        for (; xCount < 7; xCount++)
        {
            for (; yCount < 7; yCount++)
            {
                Card c = ServerEffect.serverGame.boardCtrl.GetCardAt(xCount, yCount);
                if (restriction.Evaluate(c))
                {
                    //if we haven't found a first target yet, add the first target to the list
                    if (!running)
                    {
                        ServerEffect.Targets.Add(c);
                        running = true;
                    }
                    else
                    {
                        ServerEffect.Targets[ServerEffect.Targets.Count - 1] = c;
                    }
                    //jump to the next effect to resolve
                    ServerEffect.ResolveNextSubeffect();
                    //and return (in case there's a targeting effect in the looped section somewhere)
                    return;
                } //end if card fits restriction
            } //end y loop
        } //end x loop

        //if we ever found a valid target, remove it from the list of targets because it's not a real target
        if (running)
        {
            ServerEffect.Targets.RemoveAt(ServerEffect.Targets.Count - 1);
        }
        //in case the effect runs again, reset the flag
        running = false;
        //then jump to the
        ServerEffect.ResolveSubeffect(JumpToWhenDone);
    } //end resolve
}
