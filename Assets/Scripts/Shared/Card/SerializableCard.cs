using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableCard
{
    //card type
    public char cardType;

    //perma-values
    public string cardName;
    public string effText;
    public string subtypeText;
    public string[] subtypes;
    public SerializableEffect[] effects;

    public MovementRestriction MovementRestriction;
    public AttackRestriction AttackRestriction;
    public PlayRestriction PlayRestriction;

    public int n;
    public int e;
    public int s;
    public int w;
    public int c;
    public string spellType;
    public int arg;
    public int a;
    public bool fast;
    public string subtext;
    public string[] augSubtypes;
}
