using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeNESWSubeffect : Subeffect
{
    public int nChange = 0;
    public int eChange = 0;
    public int sChange = 0;
    public int wChange = 0;

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
