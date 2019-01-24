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
        cardType = 'C';
        this.command = command;
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, bool invert = false)
    {
        cardType = 'S';
        this.command = command;
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, bool invert = false)
    {
        cardType = 'A';
        this.command = command;
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(CharacterCard charCard, string command, string args, bool invert = false)
    {
        cardType = 'C';
        this.command = command;
        this.args = args;
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, string args, bool invert = false)
    {
        cardType = 'S';
        this.command = command;
        this.args = args;
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, string args, bool invert = false)
    {
        cardType = 'A';
        this.command = command;
        this.args = args;
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(Card card, string command, bool invert = false)
    {
        if (card is CharacterCard)
        {
            cardType = 'C';
            serializedChar = (card as CharacterCard).GetSerializableVersion();
            if (invert) serializedChar.Invert();
        }
        else if (card is SpellCard)
        {
            cardType = 'S';
            serializedSpell = (card as SpellCard).GetSerializableVersion();
            if (invert) serializedSpell.Invert();
        }
        else if (card is AugmentCard)
        {
            cardType = 'A';
            serializedAug = (card as AugmentCard).GetSerializableVersion();
            if (invert) serializedAug.Invert();
        }


        this.command = command;
    }

    public Packet(Card card, string command, string args, bool invert = false) : this(card, command, invert)
    {
        this.args = args;
    }

    public Packet(CharacterCard charCard, string command, int x, int y, bool invert = false)
    {
        cardType = 'C';
        this.command = command;
        this.x = x;
        this.y = y;
        if (invert) { this.x = 7 - x; this.y = 7 - y; }
        serializedChar = charCard.GetSerializableVersion();
        if (invert) serializedChar.Invert();
    }

    public Packet(SpellCard spellCard, string command, int x, int y, bool invert = false)
    {
        cardType = 'S';
        this.command = command;
        this.x = x;
        this.y = y;
        if (invert) { this.x = 7 - x; this.y = 7 - y; }
        serializedSpell = spellCard.GetSerializableVersion();
        if (invert) serializedSpell.Invert();
    }

    public Packet(AugmentCard augCard, string command, int x, int y, bool invert = false)
    {
        cardType = 'A';
        this.command = command;
        this.x = x;
        this.y = y;
        if (invert) { this.x = 7 - x; this.y = 7 - y; }
        serializedAug = augCard.GetSerializableVersion();
        if (invert) serializedAug.Invert();
    }

    public Packet(string command, int num)
    {
        this.command = command;
        this.num = num;
    }

    public Packet(string command)
    {
        this.command = command;
    }

    public Packet(string command, int x, int y, bool invert = false)
    {
        this.command = command;
        if (invert) { this.x = 7 - x; this.y = 7 - y; }
        else { this.x = x; this.y = y; }

    }

    public Packet(string command, string args)
    {
        this.command = command;
        this.args = args;
    }

    public void RepairOwner(int playerIndex)
    {
        //explanation: if the player index (our assignment for that connection id) is 0, then we take their owner number at face value
        //  if we treat them as player 1, then we invert their assignment of player index
        //         owner
        // sender  0  1
        //      0  0  1
        //      1  1  0
        switch (cardType)
        {
            case 'C':
                serializedChar.owner = serializedChar.owner ^ playerIndex;
                break;
            case 'S':
                serializedSpell.owner = serializedSpell.owner ^ playerIndex;
                break;
            case 'A':
                serializedAug.owner = serializedAug.owner ^ playerIndex;
                break;
        }
    }

}
