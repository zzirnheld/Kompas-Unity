using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseOptionSubeffect : Subeffect
{
    public string ChoiceBlurb;
    public string[] OptionBlurbs;
    public int[] OptionJumpIndices;

    private void AskForOptionChoice()
    {
        EffectController.ServerNotifier.ChooseEffectOption(this);
    }

    public override void Resolve()
    {
        AskForOptionChoice();
    }

    public void ChooseOption(int optionIndex)
    {
        if (optionIndex >= OptionJumpIndices.Length)
        {
            //ask again
            AskForOptionChoice();
            return;
        }

        Effect.ResolveSubeffect(OptionJumpIndices[optionIndex]);
    }
}
