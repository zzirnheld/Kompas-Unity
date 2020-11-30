using KompasClient.Effects;
using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClientSerializableCard : SerializableCard
{
    public ClientEffect[] effects;
}
