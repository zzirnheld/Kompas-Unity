using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountXLoopSubeffect : LoopSubeffect
{
    protected override bool ShouldContinueLoop()
    {
        //count the number of times this happens
        parent.X++;

        //tell the client to enable the button to exit the loop
        parent.EffectController.ServerNotifier.EnableDecliningTarget();

        //always return true, if another iteration is chosen not to happen exit loop will be called
        return true;
    }

    protected override void OnLoopExit()
    {
        //we'll otherwise be off by one, 
        // since it increments every time the loop is entered, before the client gets a chance to leave
        parent.X--;

        //make the "no other targets" button disappear
        parent.EffectController.ServerNotifier.DisableDecliningTarget();
    }
}
