using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountXLoopSubeffect : Subeffect
{
    public int IndexToJumpTo;

    public override void Resolve()
    {
        //count the number of times this happens
        parent.X++;

        //tell the client to enable the button to exit the loop
        parent.serverGame.serverNetworkCtrl.EnableDecliningTarget(parent.serverGame, parent.effectController);

        //and resolve the next effect
        parent.ResolveNextSubeffect();
    }

    /// <summary>
    /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
    /// </summary>
    public void ExitLoop()
    {
        //we'll otherwise be off by one, 
        // since it increments every time the loop is entered, before the client gets a chance to leave
        parent.X--;

        //make the "no other targets" button disappear
        parent.serverGame.serverNetworkCtrl.DisableDecliningTarget(parent.serverGame, parent.effectController);

        //then skip to after the loop
        parent.ResolveSubeffect(IndexToJumpTo);
    }
}
