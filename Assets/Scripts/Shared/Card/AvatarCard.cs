using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarCard : CharacterCard
{
    public override int E
    {
        get => e < 0 ? 0 : e;
        protected set
        {
            e = value > 0 ? value : 0;
            if (e < 0) game.Lose(ControllerIndex);
        }
    }

    //TODO make this return whether the Avatar is summoned yet
    public override bool Summoned => false;

    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects, int id)
    {
        base.SetInfo(serializedCard, game, owner, effects, id);
        E *= 2;
    }
}
