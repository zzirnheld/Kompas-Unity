using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LoopSubeffect : Subeffect
{
    protected abstract bool ShouldContinueLoop();
    protected abstract void OnLoopExit();

    public int JumpTo;

    public override void Resolve()
    {
        //let the parent know there's a loop happening
        parent.loopSubeffect = this;

        //loop again if necessary
        if(ShouldContinueLoop()) parent.ResolveSubeffect(JumpTo);
    }

    /// <summary>
    /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
    /// </summary>
    public void ExitLoop()
    {
        //let parent know the loop is over
        parent.loopSubeffect = null;

        //do anything necessary to clean up the loop
        OnLoopExit();

        //then skip to after the loop
        parent.ResolveNextSubeffect();
    }
}
