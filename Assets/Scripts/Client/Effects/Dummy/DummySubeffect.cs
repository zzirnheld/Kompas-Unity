using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySubeffect : Subeffect
{
    public override Effect Effect => ClientEffect;
    public override Player Controller => ClientController;
    public override Game Game => ClientEffect.Game;

    public ClientEffect ClientEffect { get; private set; }
    public ClientPlayer ClientController => ClientEffect.ClientController;

    public static DummySubeffect FromJson(string json, ClientEffect parent, int subeffIndex)
    {
        var subeff = JsonUtility.FromJson<Subeffect>(json);

        Debug.Log($"Creating subeffect from json {json}");
        DummySubeffect toReturn;

        switch (subeff.subeffType)
        {
            case BoardTarget:
                toReturn = JsonUtility.FromJson<DummyBoardTargetSubeffect>(json);
                break;
            case DeckTarget:
            case DiscardTarget:
            case HandTarget:
                toReturn = JsonUtility.FromJson<DummyCardTargetSubeffect>(json);
                break;
            case ChooseFromList:
            case ChooseFromListSaveRest:
                toReturn = JsonUtility.FromJson<DummyListTargetSubeffect>(json);
                break;
            case SpaceTarget:
                toReturn = JsonUtility.FromJson<DummySpaceTargetSubeffect>(json);
                break;
            case ChooseEffectOption:
                toReturn = JsonUtility.FromJson<DummyChooseOptionSubeffect>(json);
                break;
            case PlayerChooseX:
                toReturn = JsonUtility.FromJson<DummyPlayerChooseXSubeffect>(json);
                break;
            default:
                Debug.Log("Creating client subeffect of a type other than one that has a specific dummy.");
                toReturn = new DummySubeffect();
                break;
        }

        if (toReturn != null)
        {
            Debug.Log($"Finishing setup for new effect of type {subeff.subeffType}");
            toReturn.Initialize(parent, subeffIndex);
        }

        return toReturn;
    }

    public virtual void Initialize(ClientEffect eff, int subeffIndex)
    {
        this.ClientEffect = eff;
        this.SubeffIndex = subeffIndex;
    }
}
