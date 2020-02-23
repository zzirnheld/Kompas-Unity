using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCard : CharacterCard
{
    public override int E
    {
        get
        {
            if (e < 0) return 0;
            return e;
        }
        set
        {
            e = value;
            if (e < 0) serverGame?.Lose(ControllerIndex);
        }
    }
}
