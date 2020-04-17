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

    public override void Resolve()
    {
        if(Target is CharacterCard charCard)
        {
            ServerGame.SetStats(charCard,
                charCard.N + nChange,
                charCard.E + eChange,
                charCard.S + sChange,
                charCard.W + wChange);
        }

        Effect.ResolveNextSubeffect();
    }
}
