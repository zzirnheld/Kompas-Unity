using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet {

    public enum Command { Play, Move, Topdeck, Discard, Rehand, AddToDeck, AddToEnemyDeck, Draw, SetNESW, SetPips, SetEnemyPips, PutBack}

    /// <summary>
    /// Contains the command that is sent.
    /// <para>Possible commands include: Summon, Cast, Augment, SetNESW, SetMyPips, SetYourPips, MoveChar, MoveSpell</para>
    /// </summary>
    public Command command;

    /*public SerializableCharCard serializedChar;
    public SerializableSpellCard serializedSpell;
    public SerializableAugCard serializedAug;*/
    public int cardID;

    public string args;
    public int x;
    public int y;
    public int n;
    public int e;
    public int s;
    public int w;
    public int num;
    public char cardType;

    public Packet(Command command)
    {
        this.command = command;
    }

    //used only for adding cards to deck
    public Packet(Command command, string cardName) : this(command)
    {
        args = cardName;
    }

    public Packet(Command command, int num) : this(command)
    {
        this.num = num;
    }

    public Packet(Command command, string cardName, int num) : this(command, cardName)
    {
        this.num = num;
    }

    public Packet(Command command, Card card) : this(command)
    {
        cardID = card.ID;
    }

    public Packet(Command command, Card card, int num) : this(command, card)
    {
        this.num = num;
    }

    public Packet(Command command, Card card, int x, int y, bool invert = false) : this(command, card)
    {
        if (invert)
        {
            this.x = 6 - x;
            this.y = 6 - y;
        }
        else
        {
            this.x = x;
            this.y = y;
        }
    }

    public Packet(Command command, Card card, int n, int e, int s, int w) : this(command, card)
    {
        this.n = n;
        this.e = e;
        this.s = s;
        this.w = w;
    }

    public void Invert()
    {
        x = 6 - x;
        y = 6 - y;
    }

    public void InvertForController(int playerFrom)
    {
        if (playerFrom == 1) Invert();
    }

}
