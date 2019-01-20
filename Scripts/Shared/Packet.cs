using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet {

    /// <summary>
    /// Contains the command that is sent.
    /// <para>Possible commands include: Summon, Cast, Augment, SetNESW, SetMyPips, SetYourPips, MoveChar, MoveSpell</para>
    /// </summary>
    public string command;

    public SerializableCharCard serializedChar;
    public SerializableSpellCard serializedSpell;
    public SerializableAugCard serializedAug;

    public string args;
    public int x;
    public int y;
    public int num;
    public char cardType;

    /// <summary>
    /// Creates a packet. make invert true to 
    /// </summary>
    /// <param name="charCard">test584269713</param>
    /// <param name="command"></param>
    public Packet(CharacterCard charCard, string command, bool invert = false)
    {
        this.command = command;
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, bool invert = false)
    {
        this.command = command;
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, bool invert = false)
    {
        this.command = command;
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(CharacterCard charCard, string command, string args, bool invert = false)
    {
        this.command = command;
        this.args = args;
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, string args, bool invert = false)
    {
        this.command = command;
        this.args = args;
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, string args, bool invert = false)
    {
        this.command = command;
        this.args = args;
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(CharacterCard charCard, string command, int x, int y, bool invert = false)
    {
        this.command = command;
        this.x = x;
        this.y = y;
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, int x, int y, bool invert = false)
    {
        this.command = command;
        this.x = x;
        this.y = y;
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, int x, int y, bool invert = false)
    {
        this.command = command;
        this.x = x;
        this.y = y;
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(string command, int num)
    {
        this.command = command;
        this.num = num;
    }

}
