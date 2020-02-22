using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeNESWSubeffect : Subeffect
{
    public int nChange;
    public int eChange;
    public int sChange;
    public int wChange;
    public int targetIndex;

    public override void Resolve()
    {
        if(parent.targets[targetIndex] is CharacterCard charCard)
        {
            charCard.N += nChange;
            charCard.E += eChange;
            charCard.S += sChange;
            charCard.W += wChange;

            parent.EffectController.ServerNotifier.NotifySetNESW(charCard);
        }

        parent.ResolveNextSubeffect();
    }
}
