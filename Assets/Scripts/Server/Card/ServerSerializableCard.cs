using KompasCore.Cards;
using KompasServer.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerSerializableCard : SerializableCard
{
    public ServerEffect[] effects;
}
