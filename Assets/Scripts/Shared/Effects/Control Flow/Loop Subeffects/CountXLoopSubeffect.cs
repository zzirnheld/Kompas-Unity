using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountXLoopSubeffect : LoopSubeffect
{
    public bool CanDecline = true;

    protected override bool ShouldContinueLoop()
    {
        //count the number of times this happens
        Effect.X++;

        //tell the client to enable the button to exit the loop
        if(CanDecline) EffectController.ServerNotifier.EnableDecliningTarget();

        //let the effect know that if there are no more targets, then call this for loop exit
        Effect.OnImpossible = this;

        //always return true, if another iteration is chosen not to happen exit loop will be called
        return true;
    }

    protected override void OnLoopExit()
    {
        //we'll otherwise be off by one, 
        // since it increments every time the loop is entered, before the client gets a chance to leave
        Effect.X--;

        //make the "no other targets" button disappear
        EffectController.ServerNotifier.DisableDecliningTarget();
    }

    public override void OnImpossible()
    {
        ExitLoop();
    }
}
