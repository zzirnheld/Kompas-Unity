using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyChooseOptionSubeffect : Subeffect
{
    public string ChoiceBlurb;
    public string[] OptionBlurbs;
    public int[] OptionJumpIndices;

    public override void Resolve()
    {
        throw new System.NotImplementedException("Dummy Choose Option Subeffect should never resolve.");
    }
}
