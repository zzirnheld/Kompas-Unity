using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarServerGameCard : ServerGameCard
{
    public override void SetE(int e)
    {
        base.SetE(e);
        if (E < 0) ServerGame.Lose(ControllerIndex);
    }

    //TODO make this return whether the Avatar is summoned yet
    public override bool Summoned => false;

    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects, int id)
    {
        base.SetInfo(serializedCard, game, owner, effects, id);
        SetE(E * 2);
    }
}
