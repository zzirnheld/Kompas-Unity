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
        ChangeNESW = 100,
        AddPips = 101,
        SetXByBoardCount = 200
    }

    //used for knowing what effect type to deserialize as
    public SubeffectType[] subeffectTypes;

    //array of strings that will get deserialized
    public string[] subeffects;

    public int maxTimesCanUsePerTurn;
}
