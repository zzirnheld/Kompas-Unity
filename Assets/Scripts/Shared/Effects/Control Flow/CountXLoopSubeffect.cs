using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountXLoopSubeffect : Subeffect
{
    public int IndexToJumpTo;

    public override void Resolve()
    {
        //let the parent know there's a loop happening
        parent.loopSubeffect = this;

        //count the number of times this happens
        parent.X++;

        //tell the client to enable the button to exit the loop
        ServerGame.serverNotifier.EnableDecliningTarget(parent.EffectController);

        //and resolve the next effect
        parent.ResolveNextSubeffect();
    }

    /// <summary>
    /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
    /// </summary>
    public void ExitLoop()
    {
        //let parent know the loop is over
        parent.loopSubeffect = null;

        //we'll otherwise be off by one, 
        // since it increments every time the loop is entered, before the client gets a chance to leave
        parent.X--;

        //make the "no other targets" button disappear
        ServerGame.serverNotifier.DisableDecliningTarget(parent.EffectController);

        //then skip to after the loop
        parent.ResolveSubeffect(IndexToJumpTo);
    }
}
