using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace KompasNetworking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public Player Player;
        public ServerGame sGame;
        public ServerNotifier ServerNotifier;

        public UIController uiCtrl;

        //TODO make code that checks if ready to resolve the stack (both players have no responses/have declined priority in a row)
        public override void ProcessPacket(Packet packet)
        {
            if (packet == null) return;
            if (packet.command != Packet.Command.Nothing) Debug.Log($"packet command is {packet.command} for player index {Player.index}");

            //switch between all the possible requests for the server to handle.
            switch (packet.command)
            {
                case Packet.Command.Nothing:
                    //do nothing, keeps the connection alive
                    break;
                case Packet.Command.AddToDeck:
                    AddCardToDeck(packet.CardName);
                    break;
                case Packet.Command.Augment:
                    Augment(packet.cardID, packet.X, packet.Y);
                    break;
                case Packet.Command.Play:
                    Play(packet.cardID, packet.X, packet.Y);
                    break;
                case Packet.Command.Move:
                    Move(packet.cardID, packet.X, packet.Y);
                    break;
                case Packet.Command.Attack:
                    Attack(packet.cardID, packet.X, packet.Y);
                    break;
                case Packet.Command.EndTurn:
                    //TODO check to see if it was their turn bewfore swapping turns
                    sGame.SwitchTurn();
                    break;
                #region effect commands
                case Packet.Command.Target:
                    if (sGame.CurrEffect != null && sGame.CurrEffect.CurrSubeffect is CardTargetSubeffect targetEff)
                    {
                        targetEff.AddTargetIfLegal(sGame.GetCardFromID(packet.cardID));
                    }
                    break;
                case Packet.Command.SpaceTarget:
                    Debug.Log("Receieved space target " + packet.X + packet.Y);
                    if (sGame.CurrEffect != null && sGame.CurrEffect.CurrSubeffect is SpaceTargetSubeffect spaceEff)
                    {
                        packet.InvertForController(Player.index);
                        spaceEff.SetTargetIfValid(packet.X, packet.Y);
                    }
                    else Debug.Log("curr effect null? " + (sGame.CurrEffect == null) + " or not spacetgtsubeff? " + (sGame.CurrEffect.CurrSubeffect is SpaceTargetSubeffect));
                    break;
                case Packet.Command.PlayerSetX:
                    sGame.CurrEffect.X = packet.EffectX;
                    sGame.CurrEffect.ResolveNextSubeffect();
                    break;
                case Packet.Command.DeclineAnotherTarget:
                    if (sGame.CurrEffect != null)
                    {
                        sGame.CurrEffect.DeclineAnotherTarget();
                    }
                    break;
                #endregion
                #region debug commands
                case Packet.Command.Topdeck:
                    if (!uiCtrl.DebugMode) break;
                    DebugTopdeck(packet.cardID);
                    break;
                case Packet.Command.Discard:
                    if (!uiCtrl.DebugMode) break;
                    DebugDiscard(packet.cardID);
                    break;
                case Packet.Command.Rehand:
                    if (!uiCtrl.DebugMode) break;
                    DebugRehand(packet.cardID);
                    break;
                case Packet.Command.Draw:
                    if (!uiCtrl.DebugMode) break;
                    DebugDraw();
                    break;
                case Packet.Command.SetNESW:
                    if (!uiCtrl.DebugMode) break;
                    DebugSetNESW(packet.cardID, packet.N, packet.E, packet.S, packet.W);
                    break;
                case Packet.Command.SetPips:
                    if (!uiCtrl.DebugMode) break;
                    DebugSetPips(packet.Pips);
                    break;
                case Packet.Command.TestTargetEffect:
                    Card whoseEffToTest = sGame.GetCardFromID(packet.cardID);
                    Debug.Log("Running eff of " + whoseEffToTest.CardName);
                    sGame.PushToStack(whoseEffToTest.Effects[0], Player.index);
                    sGame.CheckForResponse();
                    break;
                #endregion
                default:
                    Debug.Log($"Invalid command {packet.command} to server from {Player.index}");
                    break;
            }
        }

        public void AddCardToDeck(string cardName)
        {
            //add the card in, with the cardCount being the card id, then increment the card count
            Card added = Player.deckCtrl.AddCard(cardName, sGame.cardCount, Player.index);
            sGame.cardCount++;
            ServerNotifier.NotifyAddToDeck(added);
        }

        public static int InvertIndexForController(int index, int controller)
        {
            if (controller == 0) return index;
            else return 6 - index;
        }

        /// <summary>
        /// x and y here are from playerIndex's perspective
        /// </summary>
        /// <param name="sGame"></param>
        /// <param name="playerIndex"></param>
        /// <param name="cardID"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sourceID"></param>
        public void Augment(int cardID, int x, int y)
        {
            Card toAugment = sGame.GetCardFromID(cardID);
            int invertedX = InvertIndexForController(x, Player.index);
            int invertedY = InvertIndexForController(y, Player.index);
            //if it's not a valid place to do, put the cards back
            if (sGame.ValidAugment(toAugment, invertedX, invertedY))
            {
                //tell everyone to 
                ServerNotifier.NotifyPlay(toAugment, invertedX, invertedY);

                //play the card here
                toAugment.Play(invertedX, invertedY, Player.index);
            }
            else
            {
                ServerNotifier.NotifyPutBack();
            }
        }

        public void Play(int cardID, int x, int y)
        {
            //get the card to play
            Card toPlay = sGame.GetCardFromID(cardID);
            int invertedX = InvertIndexForController(x, Player.index);
            int invertedY = InvertIndexForController(y, Player.index);
            //if it's not a valid place to do, return
            if (sGame.ValidBoardPlay(toPlay, invertedX, invertedY))
            {
                ServerNotifier.NotifyPlay(toPlay, invertedX, invertedY);
                //play the card here
                toPlay.Play(invertedX, invertedY, Player.index);
                //trigger effects
                sGame.Trigger(TriggerCondition.Play, toPlay, null, null);
                sGame.CheckForResponse();
            }
            else
            {
                ServerNotifier.NotifyPutBack();
            }
        }

        public void Move(int cardID, int x, int y)
        {
            //get the card to move
            Card toMove = sGame.GetCardFromID(cardID);
            int invertedX = InvertIndexForController(x, Player.index);
            int invertedY = InvertIndexForController(y, Player.index);
            //if it's not a valid place to do, put the cards back
            if (sGame.ValidMove(toMove, invertedX, invertedY))
            {
                Debug.Log($"ServerNetworkController moving {toMove.CardName} from {invertedX} to {invertedY}");
                //move the card there
                toMove.MoveOnBoard(invertedX, invertedY);
                ServerNotifier.NotifyMove(toMove, x, y);
            }
            else
            {
                Debug.Log($"ServerNetworkController putting back {toMove.CardName}");
                ServerNotifier.NotifyPutBack();
            }
        }

        public void Attack(int cardID, int x, int y)
        {
            //get the card to move
            Card toMove = sGame.GetCardFromID(cardID);
            int invertedX = InvertIndexForController(x, Player.index);
            int invertedY = InvertIndexForController(y, Player.index);
            if (sGame.ValidAttack(toMove, invertedX, invertedY))
            {
                Debug.Log($"ServerNetworkController {toMove.CardName} attacking {invertedX}, {invertedY}");
                //tell the players to put cards down where they were
                ServerNotifier.NotifyBothPutBack();
                //then push the attack tothe stack
                CharacterCard attacker = toMove as CharacterCard;
                CharacterCard defender = sGame.boardCtrl.GetCharAt(invertedX, invertedY);
                //push the attack to the stack, then check if any player wants to respond before resolving it
                sGame.PushToStack(new Attack(sGame, attacker, defender), Player.index);
                sGame.CheckForResponse();
            }
            else
            {
                Debug.Log($"ServerNetworkController putting back {toMove.CardName}");
                ServerNotifier.NotifyPutBack();
            }
        }

        public void DebugTopdeck(int cardID)
        {
            Card toTopdeck = sGame.GetCardFromID(cardID);
            ServerNotifier.NotifyTopdeck(toTopdeck);
            toTopdeck.Topdeck();
        }

        public void DebugDiscard(int cardID)
        {
            Card toDiscard = sGame.GetCardFromID(cardID);
            ServerNotifier.NotifyDiscard(toDiscard);
            toDiscard.Discard();
            sGame.CheckForResponse();
        }

        public void DebugRehand(int cardID)
        {
            Card toRehand = sGame.GetCardFromID(cardID);
            ServerNotifier.NotifyRehand(toRehand);
            toRehand.Rehand();
        }

        public void DebugDraw()
        {
            //draw and store what was drawn
            Card toDraw = sGame.Draw(Player.index);
            if (toDraw == null) return; //deck was empty
            ServerNotifier.NotifyDraw(toDraw);
        }

        public void DebugSetPips(int pipsToSet)
        {
            sGame.GivePlayerPips(Player, pipsToSet);
        }

        public void DebugSetNESW(int cardID, int n, int e, int s, int w)
        {
            Card toSet = sGame.GetCardFromID(cardID);
            if (!(toSet is CharacterCard charToSet)) return;
            charToSet.SetNESW(n, e, s, w);
            ServerNotifier.NotifySetNESW(charToSet);
        }
    }
}
