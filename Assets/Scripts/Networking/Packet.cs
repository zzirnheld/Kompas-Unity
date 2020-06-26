using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KompasNetworking
{
    public class Packet
    {
        public enum Command
        {
            //game start procedures
            GetDeck = 0,
            SetDeck = 1,
            DeckAccepted = 2,
            SetFriendlyAvatar = 10,
            SetEnemyAvatar = 11,
            //basic player commands
            Play = 100,
            Augment = 101,
            Move = 102,
            Attack = 103,
            Attach = 104,
            EndTurn = 150,
            Leyload = 151,
            //move cards around
            Topdeck = 200,
            Discard = 201,
            Rehand = 202,
            Reshuffle = 203,
            AddAsFriendly = 204,
            AddAsEnemy = 205,
            Bottomdeck = 206,
            Draw = 207,
            Delete = 208,
            //card properties
            SetNESW = 301,
            Negate = 302,
            SetSpellStats = 303,
            Activate = 304,
            Deactivate = 305,
            ChangeControl = 306,
            SetN = 350,
            SetE = 351,
            SetS = 352,
            SetW = 353,
            SetC = 354,
            SetA = 355,
            //change numbers of cards that you see of your opponent
            IncrementEnemyDeck = 400,
            IncrementEnemyHand = 401,
            DecrementEnemyDeck = 402,
            DecrementEnemyHand = 403,
            //server requesting a target of a client
            GetAttackTarget = 500,
            RequestBoardTarget = 501,
            RequestDeckTarget = 502,
            RequestDiscardTarget = 503,
            RequestHandTarget = 504,
            GetChoicesFromList = 505,
            //client responding
            Target = 550,
            SpaceTarget = 551,
            Response = 552,
            DeclineAnotherTarget = 553,
            CancelSearch = 554,
            //server notifying if anything else is necessary
            TargetAccepted = 575,
            SpaceTargetAccepted = 576,
            //other effect technicalities
            PlayerSetX = 600,
            EnableDecliningTarget = 601,
            DisableDecliningTarget = 602,
            SetPips = 603,
            SetEnemyPips = 604,
            SetEffectsX = 605,
            OptionalTrigger = 606,
            ChooseEffectOption = 607,
            EffectResolving = 608,
            EffectImpossible = 609,
            //miscellaneous
            DiscardSimples = 700,
            PutBack = 701,
            YoureFirst = 702,
            YoureSecond = 703,
            //debug
            ActivateEffect = 800
        }

        public Packet Copy()
        {
            Packet p = new Packet(command);
            Array.Copy(normalArgs, p.normalArgs, normalArgs.Length);
            Array.Copy(specialArgs, p.specialArgs, specialArgs.Length);
            p.cardID = cardID;
            p.stringArg = stringArg;
            return p;
        }

        /// <summary>
        /// Contains the command that is sent.
        /// </summary>
        public Command command;

        public int cardID;

        public int[] normalArgs;
        public string stringArg;
        public int[] specialArgs;

        #region abstraction of args
        public string CardName => CardRepository.CardName(normalArgs[1]);
        public int CardIDToBe => cardID;

        public int Pips => normalArgs[0];
        public int EffectX => normalArgs[2];
        public bool Answer => normalArgs[0] == 1;
        public int ControllerIndex => normalArgs[0];
        public int EffectOption => normalArgs[0];
        public int Leyload => normalArgs[0];

        public CardLocation Location => (CardLocation)normalArgs[0];

        public int X => normalArgs[2];
        public int Y => normalArgs[3];

        public (int N, int E, int S, int W) Stats => (normalArgs[0], normalArgs[1], normalArgs[2], normalArgs[3]);

        public int C => normalArgs[0];

        public int EffIndex => normalArgs[0];
        public int SubeffIndex => normalArgs[1];
        public int Stat => normalArgs[0];
        #endregion abstraction of args

        #region constuctors 
        public Packet(Command command)
        {
            this.command = command;

            normalArgs = new int[4];
            specialArgs = new int[0];
        }

        public Packet(Command command, GameCard source, BoardTargetSubeffect boardTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = boardTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = boardTargetSubeffect.SubeffIndex;
        }

        public Packet(Command command, GameCard source, CardTargetSubeffect cardTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = cardTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = cardTargetSubeffect.SubeffIndex;
        }

        public Packet(Command command, GameCard source, SpaceTargetSubeffect spaceTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = spaceTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = spaceTargetSubeffect.SubeffIndex;
        }

        //used only for adding cards to deck
        public Packet(Command command, string cardName) : this(command)
        {
            try
            {
                normalArgs[1] = CardRepository.CardNameID(cardName);
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("Could not find card with name \"" + cardName + 
                    "\", length " + cardName.Length + "\n" + e.StackTrace);
            }
            catch(ArgumentNullException e)
            {
                Debug.LogError($"{cardName} was null, {e.StackTrace}");
            }
        }

        public Packet(Command command, int num) : this(command)
        {
            normalArgs[0] = num;
        }

        public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe) : this(command, cardName)
        {
            normalArgs[0] = cardLocation;
            cardID = cardIDtoBe;
        }

        public Packet(Command command, string cardName, int cardLocation, int cardIDtoBe, int x, int y) : this(command, cardName, cardLocation, cardIDtoBe)
        {
            normalArgs[2] = x;
            normalArgs[3] = y;
        }

        public Packet(Command command, GameCard card) : this(command)
        {
            if (card != null) cardID = card.ID;
        }

        public Packet(Command command, GameCard card, int num) : this(command, card)
        {
            normalArgs[0] = num;
        }

        public Packet(Command command, GameCard card, bool boolean) : this(command, card)
        {
            normalArgs[0] = boolean ? 1 : 0;
        }

        /// <summary>
        /// Used for choosing coordinates/spaces
        /// </summary>
        public Packet(Command command, int x, int y, bool invert = false) : this(command)
        {
            //this is used for the target packet
            normalArgs[0] = x;
            normalArgs[1] = y;

            if (invert)
            {
                normalArgs[2] = 6 - x;
                normalArgs[3] = 6 - y;
            }
            else
            {
                normalArgs[2] = x;
                normalArgs[3] = y;
            }
        }

        public Packet(Command command, GameCard card, int x, int y) : this(command, card)
        {
            //this is used for the target packet
            normalArgs[0] = x;
            normalArgs[1] = y;
            normalArgs[2] = x;
            normalArgs[3] = y;
        }

        public Packet(Command command, GameCard card, int x, int y, bool boolean) : this(command, card, x, y)
        {
            normalArgs[0] = boolean ? 1 : 0;
        }

        public Packet(Command command, GameCard card, int n, int e, int s, int w) : this(command, card)
        {
            normalArgs[0] = n;
            normalArgs[1] = e;
            normalArgs[2] = s;
            normalArgs[3] = w;
        }

        public Packet(Command command, int[] specialArgs) : this(command)
        {
            this.specialArgs = specialArgs;
        }

        public Packet(Command command, int[] specialArgs, int arg) : this(command, specialArgs)
        {
            normalArgs[0] = arg;
        }

        public Packet(Command command, GameCard card, int[] specialArgs, int arg0, int arg1, int arg2) : this(command, card)
        {
            this.specialArgs = specialArgs;
            normalArgs[0] = arg0;
            normalArgs[1] = arg1;
            normalArgs[2] = arg2;
        }
        #endregion
        
        public CardRestriction GetCardRestriction(ClientGame clientGame)
        {
            GameCard thatHasEffect = clientGame.GetCardWithID(cardID);
            Effect eff = thatHasEffect.Effects.ElementAt(normalArgs[0]);
            DummyCardTargetSubeffect subeff = eff.Subeffects[normalArgs[1]] as DummyCardTargetSubeffect;

            if(subeff == null)
            {
                Debug.LogError($"Tried to get effect from card {cardID}, eff index {normalArgs[0]}," +
                    $"subeff index {normalArgs[1]} but it was null or not a card target subeff");
                return null;
            }

            return subeff.cardRestriction;
        }
        
        public BoardRestriction GetBoardRestriction(ClientGame clientGame)
        {
            GameCard thatHasEffect = clientGame.GetCardWithID(cardID);
            Effect eff = thatHasEffect.Effects.ElementAt(normalArgs[0]);
            DummyBoardTargetSubeffect subeff = eff.Subeffects[normalArgs[1]] as DummyBoardTargetSubeffect;

            if (subeff == null)
            {
                Debug.LogError($"Tried to get effect from card {cardID}, eff index {normalArgs[0]}," +
                    $"subeff index {normalArgs[1]} but it was null or not a card target subeff");
                return null;
            }

            return subeff.boardRestriction;
        }
        
        public SpaceRestriction GetSpaceRestriction(ClientGame clientGame)
        {
            GameCard thatHasEffect = clientGame.GetCardWithID(cardID);
            Effect eff = thatHasEffect.Effects.ElementAt(normalArgs[0]);
            DummySpaceTargetSubeffect subeff = eff.Subeffects[normalArgs[1]] as DummySpaceTargetSubeffect;

            if (subeff == null)
            {
                Debug.LogError($"Tried to get effect from card {cardID}, eff index {normalArgs[0]}," +
                    $"subeff index {normalArgs[1]} but it was null or not a card target subeff");
                return null;
            }

            return subeff.spaceRestriction;
        }

        public ListRestriction GetListRestriction(ClientGame clientGame)
        {
            Debug.Log($"Getting list restriction for {string.Join(", ", normalArgs)}");
            GameCard thatHasEffect = clientGame.GetCardWithID(cardID);
            Effect eff = thatHasEffect.Effects.ElementAt(EffIndex);
            DummyListTargetSubeffect subeff = eff.Subeffects[normalArgs[2]] as DummyListTargetSubeffect;

            if (subeff == null)
            {
                Debug.LogError($"Tried to get effect from card {cardID}, eff index {normalArgs[0]}," +
                    $"subeff index {normalArgs[1]} but it was null or not a card target subeff");
                return null;
            }

            return subeff.ListRestriction;
        }

        private void Invert()
        {
            if (normalArgs == null)
            {
                Debug.Log("args is null");
                return;
            }
            normalArgs[2] = 6 - normalArgs[2];
            normalArgs[3] = 6 - normalArgs[3];
        }

        private bool IsInvertibleCommand(Command command)
        {
            switch (command)
            {
                case Command.SetNESW:
                case Command.GetChoicesFromList:
                case Command.ActivateEffect:
                case Command.PlayerSetX:
                case Command.SetEffectsX:
                case Command.EffectResolving:
                case Command.SetEnemyPips:
                case Command.SetPips:
                case Command.Leyload:
                case Command.SetN:
                case Command.SetE:
                case Command.SetS:
                case Command.SetW:
                case Command.SetC:
                case Command.SetA:
                case Command.GetAttackTarget:
                case Command.RequestBoardTarget:
                case Command.RequestDeckTarget:
                case Command.RequestDiscardTarget:
                case Command.RequestHandTarget:
                    return false;
                default:
                    return true;
            }
        }

        public void InvertForController(int playerFrom)
        {
            if (playerFrom == 1 && IsInvertibleCommand(command)) Invert();
        }

    }

}