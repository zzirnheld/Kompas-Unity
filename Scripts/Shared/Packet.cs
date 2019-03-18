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

    public int[] args;

    //public string args;
    //public int x;
    //public int y;
    //public int n;
    //public int e;
    //public int s;
    //public int w;
    //public int num;
    //public char cardType;

    public Packet(Command command)
    {
        this.command = command;
        args = new int[4];
    }

    //used only for adding cards to deck
    public Packet(Command command, string cardName) : this(command)
    {
        args[0] = Game.CardNameIndices[cardName];
    }

    public Packet(Command command, int num) : this(command)
    {
        args[0] = num;
    }

    public Packet(Command command, string cardName, int num) : this(command, cardName)
    {
        args[0] = num;
    }

    public Packet(Command command, Card card) : this(command)
    {
        cardID = card.ID;
    }

    public Packet(Command command, Card card, int num) : this(command, card)
    {
        args[0] = num;
    }

    public Packet(Command command, Card card, int x, int y, bool invert = false) : this(command, card)
    {
        if (invert)
        {
            args[0] = 6 - x;
            args[1] = 6 - y;
        }
        else
        {
            args[0] = x;
            args[1] = y;
        }
    }

    public Packet(Command command, Card card, int n, int e, int s, int w) : this(command, card)
    {
        args[0] = n;
        args[1] = e;
        args[2] = s;
        args[3] = w;
    }

    public void Invert()
    {
        args[0] = 6 - args[0];
        args[1] = 6 - args[1];
    }

    public void InvertForController(int playerFrom)
    {
        if (playerFrom == 1) Invert();
    }

}
