using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSubeffect : ServerSubeffect
{
    public int JumpTo;
    public bool CanDecline = false;

    protected virtual void OnLoopExit()
    {
        //make the "no other targets" button disappear
        if (CanDecline) EffectController.ServerNotifier.DisableDecliningTarget();
    }

    protected virtual bool ShouldContinueLoop => true;

    public override void Resolve()
    {
        //loop again if necessary
        Debug.Log($"im in ur loop of type {GetType()}, the one that jumps to {JumpTo}");
        if (ShouldContinueLoop)
        {
            //tell the client to enable the button to exit the loop
            if (CanDecline)
            {
                EffectController.ServerNotifier.EnableDecliningTarget();
                ServerEffect.OnImpossible = this;
            }
            ServerEffect.ResolveSubeffect(JumpTo);
        }
        else ExitLoop();
    }

    /// <summary>
    /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
    /// </summary>
    public void ExitLoop()
    {
        //let parent know the loop is over
        if(ServerEffect.OnImpossible == this) ServerEffect.OnImpossible = null;

        //do anything necessary to clean up the loop
        OnLoopExit();

        //then skip to after the loop
        ServerEffect.ResolveSubeffect(SubeffIndex + 1);
    }
}
