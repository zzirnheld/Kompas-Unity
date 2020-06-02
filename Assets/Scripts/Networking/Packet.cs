using System;
using System.Collections.Generic;
using UnityEngine;

namespace KompasNetworking
{
    public class Packet
    {
        public static int id = 0;

        public enum Command
        {
            //game start procedures
            GetDeck, SetDeck, SetFriendlyAvatar, SetEnemyAvatar,
            //basic player commands
            Play, Augment, Move, EndTurn, Attack,
            //move cards around
            Topdeck, Discard, Rehand, Reshuffle, AddAsFriendly, AddAsEnemy, Bottomdeck,
            Draw, Delete,
            //card properties
            SetNESW, Negate, SetSpellStats, Activate, Deactivate, ChangeControl,
            //change numbers of cards that you see of your opponent
            IncrementEnemyDeck, IncrementEnemyHand, DecrementEnemyDeck, DecrementEnemyHand,
            //server requesting a target of a client
            GetAttackTarget, RequestBoardTarget, RequestDeckTarget, RequestDiscardTarget, RequestHandTarget, GetChoicesFromList,
            //client responding
            Target, SpaceTarget, Response, DeclineAnotherTarget, CancelSearch,
            //server notifying if anything else is necessary
            TargetAccepted, SpaceTargetAccepted,
            //other effect technicalities
            PlayerSetX, EnableDecliningTarget, DisableDecliningTarget, SetPips, SetEnemyPips, SetEffectsX,
            OptionalTrigger, ChooseEffectOption, EffectResolving,
            //miscellaneous
            DiscardSimples, PutBack, YoureFirst, YoureSecond,
            //debug
            ActivateEffect
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
        public int packetID;

        public int[] normalArgs;
        public string stringArg;
        public int[] specialArgs;

        #region abstraction of args
        public string CardName => Game.CardNames[normalArgs[1]];
        public int CardIDToBe => cardID;

        public int Pips => normalArgs[0];
        public int EffectX => normalArgs[2];
        public bool Answer => normalArgs[0] == 1;
        public int ControllerIndex => normalArgs[0];
        public int EffectOption => normalArgs[0];

        public CardLocation Location => (CardLocation)normalArgs[0];

        public int X => normalArgs[2];
        public int Y => normalArgs[3];

        public int N => normalArgs[0];
        public int E => normalArgs[1];
        public int S => normalArgs[2];
        public int W => normalArgs[3];

        public int C => normalArgs[0];

        public int EffIndex => normalArgs[0];
        public int SubeffIndex => normalArgs[1];
        #endregion abstraction of args

        #region constuctors 
        public Packet(Command command)
        {
            this.command = command;
            id = (id + 1) % 1000;

            normalArgs = new int[4];
            specialArgs = new int[0];
        }

        public Packet(Command command, Card source, BoardTargetSubeffect boardTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = boardTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = boardTargetSubeffect.SubeffIndex;
        }

        public Packet(Command command, Card source, CardTargetSubeffect cardTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = cardTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = cardTargetSubeffect.SubeffIndex;
        }

        public Packet(Command command, Card source, SpaceTargetSubeffect spaceTargetSubeffect) : this(command, source)
        {
            normalArgs[0] = spaceTargetSubeffect.ServerEffect.EffectIndex;
            normalArgs[1] = spaceTargetSubeffect.SubeffIndex;
        }

        //used only for adding cards to deck
        public Packet(Command command, string cardName) : this(command)
        {
            try
            {
                normalArgs[1] = Game.CardNameIndices[cardName];
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

        public Packet(Command command, Card card) : this(command)
        {
            if (card != null) cardID = card.ID;
        }

        public Packet(Command command, Card card, int num) : this(command, card)
        {
            normalArgs[0] = num;
        }

        public Packet(Command command, Card card, bool boolean) : this(command, card)
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

        public Packet(Command command, Card card, int x, int y) : this(command, card)
        {
            //this is used for the target packet
            normalArgs[0] = x;
            normalArgs[1] = y;
            normalArgs[2] = x;
            normalArgs[3] = y;
        }

        public Packet(Command command, Card card, int x, int y, bool boolean) : this(command, card, x, y)
        {
            normalArgs[0] = boolean ? 1 : 0;
        }

        public Packet(Command command, Card card, int n, int e, int s, int w) : this(command, card)
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

        public Packet(Command command, Card card, int[] specialArgs, int arg0, int arg1, int arg2) : this(command, card)
        {
            this.specialArgs = specialArgs;
            normalArgs[0] = arg0;
            normalArgs[1] = arg1;
            normalArgs[2] = arg2;
        }
        #endregion
        
        public CardRestriction GetCardRestriction(ClientGame clientGame)
        {
            Card thatHasEffect = clientGame.GetCardFromID(cardID);
            Effect eff = thatHasEffect.Effects[normalArgs[0]];
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
            Card thatHasEffect = clientGame.GetCardFromID(cardID);
            Effect eff = thatHasEffect.Effects[normalArgs[0]];
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
            Card thatHasEffect = clientGame.GetCardFromID(cardID);
            Effect eff = thatHasEffect.Effects[normalArgs[0]];
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
            Card thatHasEffect = clientGame.GetCardFromID(cardID);
            Effect eff = thatHasEffect.Effects[normalArgs[1]];
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