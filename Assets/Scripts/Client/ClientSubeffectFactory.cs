using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSubeffectFactory : ISubeffectFactory
{
    public Subeffect FromJson(SubeffectType subeffectType, string json, Effect parent)
    {
        switch (subeffectType)
        {
            case SubeffectType.TargetCardOnBoard:
                return JsonUtility.FromJson<DummyBoardTargetSubeffect>(json);
            case SubeffectType.DeckTarget:
            case SubeffectType.DiscardTarget:
            case SubeffectType.HandTarget:
                return JsonUtility.FromJson<DummyCardTargetSubeffect>(json);
            case SubeffectType.SpaceTarget:
                return JsonUtility.FromJson<DummySpaceTargetSubeffect>(json);
            default:
                Debug.Log("Creating client subeffect of a type other than one that has a specific dummy.");
                return new DummySubeffect();
        }
    }
}
