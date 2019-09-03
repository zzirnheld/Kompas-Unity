using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    [System.NonSerialized] public Effect parent;

    public ServerGame ServerGame { get { return parent.serverGame; } }

    /// <summary>
    /// parent resolve method. at the end, needs to call resolve subeffect in parent
    /// if it's an if, it does a specific index
    /// otherwise, it does currentIndex + 1
    /// </summary>
    public abstract void Resolve();

    /// <summary>
    /// Called by restrictions that have found a valid target to add to the list
    /// </summary>
    /// <param name="card"></param>
    public void ContinueResolutionWith(Card card)
    {
        parent.targets.Add(card);
        parent.ResolveSubeffect(parent.effectIndex + 1);
    }

    public virtual void Initialize()
    {

    }

    public static Subeffect FromJson(SerializableEffect.SubeffectType seType, string subeffJson, Effect parent)
    {
        Subeffect toReturn = null;

        switch (seType)
        {
            case SerializableEffect.SubeffectType.TargetCardOnBoard:
                toReturn = JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.ChangeNESW:
                toReturn = JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.DeckTarget:
                toReturn = JsonUtility.FromJson<DeckTargetSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.DiscardTarget:
                toReturn = JsonUtility.FromJson<DiscardTargetSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.HandTarget:
                toReturn = JsonUtility.FromJson<HandTargetSubeffect>(subeffJson);
                break;
            default:
                Debug.Log("Unrecognized effect type enum for loading effect in effect constructor");
                break;
        }

        if(toReturn != null)
        {
            toReturn.parent = parent;
            toReturn.Initialize();
        }

        return toReturn;
    }
}