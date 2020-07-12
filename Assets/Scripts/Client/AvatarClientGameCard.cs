using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarClientGameCard : ClientGameCard
{
    public override bool IsAvatar => true;

    public override bool Remove(IStackable stackSrc = null)
    {
        Debug.LogWarning("Remove called for Avatar - doing nothing");
        if (Summoned) return base.Remove(stackSrc);
        else return Location == CardLocation.Nowhere;
    }

    public override void SetInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, game, owner, effects, id);
        SetE(E * 2);
    }
}
