using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LoopSubeffect : Subeffect
{
    protected abstract void OnLoopExit();
    protected abstract bool ShouldContinueLoop();

    public int JumpTo;

    public override void Resolve()
    {
        //let the parent know there's a loop happening
        parent.loopSubeffect = this;

        //loop again if necessary
        Debug.Log($"im in ur loop, the one that jumps to {JumpTo}");
        if (ShouldContinueLoop()) parent.ResolveSubeffect(JumpTo);
        else parent.ResolveNextSubeffect();
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
