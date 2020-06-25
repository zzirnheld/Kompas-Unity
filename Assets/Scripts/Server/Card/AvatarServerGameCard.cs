using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarServerGameCard : ServerGameCard
{
    public override void SetE(int e, IStackable stackSrc = null)
    {
        base.SetE(e, stackSrc);
        if (E < 0) ServerGame.Lose(ControllerIndex);
    }

    //TODO make this return whether the Avatar is summoned yet
    public override bool Summoned => false;
    public override bool IsAvatar => true;

    public override void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, game, owner, effects, id);
        E *= 2;
    }
}
