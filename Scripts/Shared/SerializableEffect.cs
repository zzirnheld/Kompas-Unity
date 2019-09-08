using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEffect
{
    public enum SubeffectType {
        TargetCardOnBoard = 0,
        DeckTarget = 1,
        DiscardTarget = 2,
        HandTarget = 3,
        SpaceTarget = 4,
        ChangeNESW = 100,
        AddPips = 101,
        PayPips = 102,
        PayPipsByTargetCost = 150,
        SetXByBoardCount = 200,
        //SetXByTargetN = 250,
        //SetXByTargetE = 251,
        SetXByTargetS = 252,
        //SetXByTargetW = 253,
        SetXByTargetCost = 254,
        PlayCard = 300,
        DiscardCard = 301
    }

    //used for knowing what effect type to deserialize as
    public SubeffectType[] subeffectTypes;

    //array of strings that will get deserialized
    public string[] subeffects;

    //used for knowing what trigger to deserialize as
    public TriggerCondition triggerCondition;

    //string to be deserialized
    public string trigger;

    public int maxTimesCanUsePerTurn;
}
