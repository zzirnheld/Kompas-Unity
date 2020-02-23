using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasNetworking
{
    public class Packet
    {
        public static int id = 0;

        public enum Command
        {
            //no-op packets
            Nothing, Confirm,
            //basic player commands
            Play, Augment, Move, EndTurn, Attack,
            //move cards around
            Topdeck, Discard, Rehand, Reshuffle, AddAsFriendly, AddAsEnemy, AddToDeck,
            Draw, Delete,
            //change numbers of cards that you see of your opponent
            IncrementEnemyDeck, IncrementEnemyHand, DecrementEnemyDeck, DecrementEnemyHand,
            //server requesting a target of a client
            GetAttackTarget, RequestBoardTarget, RequestDeckTarget, RequestDiscardTarget, RequestHandTarget,
            //client responding
            Target, SpaceTarget, Response, DeclineAnotherTarget,
            //server notifying if anything else is necessary
            TargetAccepted, SpaceTargetAccepted,
            //other effect technicalities
            PlayerSetX, EnableDecliningTarget, DisableDecliningTarget, SetNESW, SetPips, SetEnemyPips, SetEffectsX,
            //miscellaneous
            DiscardSimples, PutBack, YoureFirst, YoureSecond,
            //debug
            TestTargetEffect
        }

        public Packet Copy()
        {
            Packet p = new Packet(command);
            Array.Copy(args, p.args, args.Length);
            p.cardID = cardID;
            p.stringArg = stringArg;
            return p;
        }

        /// <summary>
        /// Contains the command that is sent.
        /// </summary>
        public Command command;

        public int cardID;
        public int packetID;

        public int[] args;
        public string stringArg;

        #region abstraction of args
        public CardRestriction CardRestriction 
        {
            get 
            {
                try
                {
                    return JsonUtility.FromJson<CardRestriction>(stringArg);
                }
                catch (ArgumentException)
                {
                    Debug.LogError($"Could not deserialize {stringArg} to card restriction");
                    return null;
                }
            } 
        }
        public BoardRestriction BoardRestriction
        {
            get
            {
                try
                {
                    return JsonUtility.FromJson<BoardRestriction>(stringArg);
                }
                catch (ArgumentException)
                {
                    Debug.LogError($"Could not deserialize {stringArg} to card restriction");
                    return null;
                }
            }
        }
        public SpaceRestriction SpaceRestriction
        {
            get
            {
                try
                {
                    return JsonUtility.FromJson<SpaceRestriction>(stringArg);
                }
                catch (ArgumentException)
                {
                    Debug.LogError($"Could not deserialize {stringArg} to card restriction");
                    return null;
                }
            }
        }
        public string CardName { get { return Game.CardNames[args[1]]; } }
        public int CardIDToBe { get { return cardID; } }

        public int Pips { get { return args[0]; } }
        public int EffectX { get => args[0]; }

        public CardLocation Location { get { return (CardLocation)args[0]; } }

        public int X { get { return args[2]; } }
        public int Y { get { return args[3]; } }

        public int N { get { return args[0]; } }
        public int E { get { return args[1]; } }
        public int S { get { return args[2]; } }
        public int W { get { return args[3]; } }

        public int EffIndex { get => args[0]; }
        public int SubeffIndex { get => args[1]; }
        #endregion abstraction of args

        #region constuctors 
        /// <summary>
        /// Use only for confirm packet
        /// </summary>
        /// <param name="packetID"></param>
        public Packet(int packetID)
        {
            command = Command.Confirm;
            this.packetID = packetID;
            args = new int[4];
        }

        public Packet(Command command)
        {
            this.command = command;
            if (command != Command.Nothing)
            {
                packetID = id;
                id = (id + 1) % 1000;
            }

            args = new int[4];
        }

        public Packet(Command command, Card source, BoardRestriction boardRestriction, int X) : this(command, source, X)
        {
            stringArg = JsonUtility.ToJson(boardRestriction);
        }

        public Packet(Command command, Card source, CardRestriction cardRestriction, int X) : this(command, source, X)
        {
            stringArg = JsonUtility.ToJson(cardRestriction);
        }

        public Packet(Command command, Card source, SpaceRestriction spaceRestriction, int X) : this(command, source, X)
        {
            stringArg = JsonUtility.ToJson(spaceRestriction);
        }

        //used only for adding cards to deck
        public Packet(Command command, string cardName) : this(command)
        {
            try
            {
                args[1] = Game.CardNameIndices[cardName];
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("Could not find card with name \"" + cardName + 
                    "\", length " + cardName.Length + "\n" + e.StackTrace);
            }
        }

        public Packet(Command command, int num) : this(command)
        {
            args[0] = num;
        }

        public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe) : this(command, cardName)
        {
            args[0] = cardLocation;
            cardID = cardIDtoBe;
        }

        public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe, int x, int y, bool invert) : this(command, cardName, cardLocation, cardIDtoBe)
        {
            if (invert)
            {
                args[2] = 6 - x;
                args[3] = 6 - y;
            }
            else
            {
                args[2] = x;
                args[3] = y;
            }
        }

        public Packet(Command command, Card card) : this(command)
        {
            if (card != null) cardID = card.ID;
        }

        public Packet(Command command, Card card, int num) : this(command, card)
        {
            args[0] = num;
        }

        /// <summary>
        /// Used for choosing coordinates/spaces
        /// </summary>
        public Packet(Command command, int x, int y, bool invert = false) : this(command)
        {
            //this is used for the target packet
            args[0] = x;
            args[1] = y;

            if (invert)
            {
                args[2] = 6 - x;
                args[3] = 6 - y;
            }
            else
            {
                args[2] = x;
                args[3] = y;
            }
        }

        public Packet(Command command, Card card, int x, int y, bool invert = false) : this(command, card)
        {
            //this is used for the target packet
            args[0] = x;
            args[1] = y;

            if (invert)
            {
                args[2] = 6 - x;
                args[3] = 6 - y;
            }
            else
            {
                args[2] = x;
                args[3] = y;
            }
        }

        public Packet(Command command, Card card, int n, int e, int s, int w) : this(command, card)
        {
            args[0] = n;
            args[1] = e;
            args[2] = s;
            args[3] = w;
        }
        #endregion


        public void Invert()
        {
            if (args == null)
            {
                Debug.Log("args is null");
                return;
            }
            args[2] = 6 - args[2];
            args[3] = 6 - args[3];
        }

        public void InvertForController(int playerFrom)
        {
            if (playerFrom == 1 && command != Command.SetNESW) Invert();
        }

    }

}