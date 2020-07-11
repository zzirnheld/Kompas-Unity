using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarClientGameCard : ClientGameCard
{
    public override bool CanRemove => false;
    public override bool IsAvatar => true;

    public override void SetInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, game, owner, effects, id);
        SetE(E * 2);
    }
}
