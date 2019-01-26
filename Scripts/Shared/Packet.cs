﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet {

    public enum Command { Play, Move, Topdeck, Discard, Rehand, AddToDeck, ImportDecklist}

    /// <summary>
    /// Contains the command that is sent.
    /// <para>Possible commands include: Summon, Cast, Augment, SetNESW, SetMyPips, SetYourPips, MoveChar, MoveSpell</para>
    /// </summary>
    public Command command;

    public SerializableCharCard serializedChar;
    public SerializableSpellCard serializedSpell;
    public SerializableAugCard serializedAug;
    public int cardID;

    public string args;
    public int x;
    public int y;
    public int num;
    public char cardType;

    //used only for adding cards to deck
    public Packet(Command command, string cardName)
    {
        this.command = command;
        args = cardName;
    }

    public Packet(Command command, Card card)
    {
        cardID = card.ID;
        this.command = command;
    }

    public Packet(Command command, Card card, int num) : this(command, card)
    {
        this.num = num;
    }

    public Packet(Command command, Card card, int x, int y) : this(command, card)
    {
        this.x = x;
        this.y = y;
    }

    public void Invert()
    {
        x = 7 - x;
        y = 7 - y;
    }

    public void InvertForController(int playerFrom)
    {
        if (playerFrom == 1) Invert();
    }

}
