using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountXLoopSubeffect : LoopSubeffect
{
    protected override bool ShouldContinueLoop
    {
        get
        {
            //count the number of times this happens
            ServerEffect.X++;

            //let the effect know that if there are no more targets, then call this for loop exit
            ServerEffect.OnImpossible = this;

            //always return true, if another iteration is chosen not to happen exit loop will be called
            return true;
        }
    }

    //Specifically for this type of loop, if another target isn't found, that's when we exit the loop
    public override bool OnImpossible() => ExitLoop();
}
