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

    public string args;
    public int x;
    public int y;
    public int num;

    /// <summary>
    /// Creates a packet. make invert true to 
    /// </summary>
    /// <param name="charCard">test584269713</param>
    /// <param name="command"></param>
    public Packet(CharacterCard charCard, string command, bool invert = false)
    {
        this.command = command;
        serializedChar = charCard.GetSerializableVersion();
        if(invert)
    }

    public Packet(SpellCard spellCard, string command)
    {
        this.command = command;
        serializedSpell = spellCard.GetSerializableVersion();
    }

    public Packet(CharacterCard charCard, string command, string args)
    {
        this.command = command;
        this.args = args;
        serializedChar = charCard.GetSerializableVersion();
    }

    public Packet(SpellCard spellCard, string command, string args)
    {
        this.command = command;
        this.args = args;
        serializedSpell = spellCard.GetSerializableVersion();
    }

    public Packet(CharacterCard charCard, string command, int x, int y)
    {
        this.command = command;
        this.x = x;
        this.y = y;
        serializedChar = charCard.GetSerializableVersion();
    }

    public Packet(SpellCard spellCard, string command, int x, int y)
    {
        this.command = command;
        this.x = x;
        this.y = y;
        serializedSpell = spellCard.GetSerializableVersion();
    }

    public Packet(string command, int num)
    {
        this.command = command;
        this.num = num;
    }

}
