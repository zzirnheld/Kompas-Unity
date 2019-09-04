using System;
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
        parent.ResolveSubeffect(parent.subeffectIndex + 1);
    }

    public virtual void Initialize()
    {

    }

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex;

    public Card Target
    {
        get
        {
            return TargetIndex < 0 ?
                parent.targets[parent.targets.Count + TargetIndex] :
                parent.targets[TargetIndex];
        }
    }

    public static Subeffect FromJson(SerializableEffect.SubeffectType seType, string subeffJson, Effect parent)
    {
        Debug.Log("Creating subeffect from json of type " + seType + " json " + subeffJson);

        Subeffect toReturn = null;

        switch (seType)
        {
            case SerializableEffect.SubeffectType.TargetCardOnBoard:
                toReturn = JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
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
            case SerializableEffect.SubeffectType.ChangeNESW:
                toReturn = JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.AddPips:
                toReturn = JsonUtility.FromJson<AddPipsSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.SetXByBoardCount:
                toReturn = JsonUtility.FromJson<SetXBoardRestrictionSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.SpaceTarget:
                toReturn = JsonUtility.FromJson<SpaceTargetSubeffect>(subeffJson);
                Debug.Log("Toreturn is null? " + (toReturn == null) + " here");
                Debug.Log("Is the restriction null tho " + ((toReturn as SpaceTargetSubeffect).spaceRestriction == null));
                break;
            case SerializableEffect.SubeffectType.PayPips:
                toReturn = JsonUtility.FromJson<PayPipsSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.SetXByTargetS:
                toReturn = JsonUtility.FromJson<SetXTargetSSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.SetXByTargetCost:
                throw new NotImplementedException();
            case SerializableEffect.SubeffectType.PlayCard:
                toReturn = JsonUtility.FromJson<PlaySubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.PayPipsByTargetCost:
                toReturn = JsonUtility.FromJson<PayPipsTargetCostSubeffect>(subeffJson);
                break;
            case SerializableEffect.SubeffectType.DiscardCard:
                toReturn = JsonUtility.FromJson<DiscardSubeffect>(subeffJson);
                break;
            default:
                Debug.Log("Unrecognized effect type enum for loading effect in effect constructor");
                break;
        }

        if (toReturn != null)
        {
            Debug.Log("Toreturn was not null, but was " + toReturn);
            toReturn.parent = parent;
            toReturn.Initialize();
        }

        return toReturn;
    }
}