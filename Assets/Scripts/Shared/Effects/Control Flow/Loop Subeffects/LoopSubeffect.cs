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
        //loop again if necessary
        Debug.Log($"im in ur loop, the one that jumps to {JumpTo}");
        if (ShouldContinueLoop()) Effect.ResolveSubeffect(JumpTo);
        else ExitLoop();
    }

    /// <summary>
    /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
    /// </summary>
    public void ExitLoop()
    {
        //let parent know the loop is over
        if(Effect.OnImpossible == this) Effect.OnImpossible = null;

        //do anything necessary to clean up the loop
        OnLoopExit();

        //then skip to after the loop
        Effect.ResolveSubeffect(SubeffIndex + 1);
    }
}
