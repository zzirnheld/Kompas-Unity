using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySimpleCommand : StackableCommand
{
    SpellCard simple;
    int x;
    int y;

    public PlaySimpleCommand(SpellCard simple, int x, int y)
    {
        this.simple = simple;
        this.x = x;
        this.y = y;
    }

    public override void StartResolution()
    {
        //put the simple onto the field, TODO unless it's put there as part of going on the stack
        serverGame.Play(simple, x, y);

        //TODO: get simple's effect, and push it onto the stack UNLESS i make it a triggered etb effect, and triggers happen as soon as the triggering event happens

        //after its effect is pushed to the stack, the simple is discarded
        simple.Discard();

        //and we move to the next resolution, which is probably this card's effect
        FinishResolution();
    }

    public override void CancelResolution()
    {
        simple.Discard();
        FinishResolution();
    }
}
