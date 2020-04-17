using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarCard : CharacterCard
{
    public override int E
    {
        get
        {
            if (e < 0) return 0;
            return e;
        }
        protected set
        {
            e = value > 0 ? value : 0;
            if (e < 0) serverGame?.Lose(ControllerIndex);
        }
    }
}
