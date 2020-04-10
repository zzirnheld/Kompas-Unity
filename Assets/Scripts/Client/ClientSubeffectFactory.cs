using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSubeffectFactory : ISubeffectFactory
{
    public Subeffect FromJson(SubeffectType subeffectType, string json, Effect parent, int subeffIndex)
    {
        Subeffect toReturn;

        switch (subeffectType)
        {
            case SubeffectType.TargetCardOnBoard:
                toReturn = JsonUtility.FromJson<DummyBoardTargetSubeffect>(json);
                break;
            case SubeffectType.DeckTarget:
            case SubeffectType.DiscardTarget:
            case SubeffectType.HandTarget:
            case SubeffectType.TargetAll:
                toReturn = JsonUtility.FromJson<DummyCardTargetSubeffect>(json);
                break;
            case SubeffectType.SpaceTarget:
                toReturn = JsonUtility.FromJson<DummySpaceTargetSubeffect>(json);
                break;
            default:
                Debug.Log("Creating client subeffect of a type other than one that has a specific dummy.");
                toReturn = new DummySubeffect();
                break;
        }

        if (toReturn != null)
        {
            Debug.Log($"Finishing setup for new effect of type {subeffectType}");
            toReturn.Effect = parent;
            toReturn.Initialize();
            toReturn.SubeffIndex = subeffIndex;
        }

        return toReturn;
    }
}
