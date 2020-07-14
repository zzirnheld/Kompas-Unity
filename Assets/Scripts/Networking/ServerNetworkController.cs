using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace KompasNetworking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public ServerPlayer Player;
        public ServerGame sGame;
        public ServerNotifier ServerNotifier;

        //TODO make code that checks if ready to resolve the stack (both players have no responses/have declined priority in a row)
        public override void ProcessPacket(Packet packet)
        {
            if (packet == null) return;
            packet.InvertForController(Player.index);
            Debug.Log($"packet command is {packet.command} for player index {Player.index}," +
                $"inverted numbers {packet.normalArgs[0]}, {packet.normalArgs[1]}, {packet.normalArgs[2]}, {packet.normalArgs[3]}");

            //switch between all the possible requests for the server to handle.
            switch (packet.command)
            {
                case Packet.Command.SetDeck:
                    sGame.SetDeck(Player, packet.stringArg);
                    break;
                case Packet.Command.Augment:
                    Player.TryAugment(sGame.GetCardWithID(packet.cardID), packet.X, packet.Y);
                    break;
                case Packet.Command.Play:
                    Player.TryPlay(sGame.GetCardWithID(packet.cardID), packet.X, packet.Y);
                    break;
                case Packet.Command.Move:
                    Player.TryMove(sGame.GetCardWithID(packet.cardID), packet.X, packet.Y);
                    break;
                case Packet.Command.Attack:
                    var attacker = sGame.GetCardWithID(packet.cardID);
                    var defender = sGame.boardCtrl.GetCardAt(packet.X, packet.Y);
                    Player.TryAttack(attacker, defender);
                    break;
                case Packet.Command.EndTurn:
                    //TODO check to see if it was their turn bewfore swapping turns
                    sGame.SwitchTurn();
                    break;
                #region effect commands
                case Packet.Command.Target:
                    var currSubeff = sGame.CurrEffect?.CurrSubeffect;
                    if (currSubeff is CardTargetSubeffect targetEff)
                    {
                        targetEff.AddTargetIfLegal(sGame.GetCardWithID(packet.cardID));
                    }
                    else if(currSubeff is ChooseFromListSubeffect chooseListSubeff)
                    {
                        var choice = new List<GameCard>{ sGame.GetCardWithID(packet.cardID) };
                        chooseListSubeff.AddListIfLegal(choice);
                    }
                    break;
                case Packet.Command.SpaceTarget:
                    Debug.Log("Receieved space target " + packet.X + packet.Y);
                    if (sGame.CurrEffect?.CurrSubeffect is SpaceTargetSubeffect spaceEff)
                    {
                        spaceEff.SetTargetIfValid(packet.X, packet.Y);
                    }
                    else Debug.Log("curr effect null? " + (sGame.CurrEffect == null) + " or not spacetgtsubeff? " + (sGame.CurrEffect?.CurrSubeffect is SpaceTargetSubeffect));
                    break;
                case Packet.Command.PlayerSetX:
                    if (sGame.CurrEffect?.CurrSubeffect is PlayerChooseXSubeffect xEff)
                    {
                        xEff.SetXIfLegal(packet.EffectX);
                    }
                    else Debug.Log("curr effect null? " + (sGame.CurrEffect == null) + " or not player set x? " + (sGame.CurrEffect?.CurrSubeffect is SpaceTargetSubeffect));
                    break;
                case Packet.Command.DeclineAnotherTarget:
                    sGame.CurrEffect?.DeclineAnotherTarget();
                    break;
                case Packet.Command.GetChoicesFromList:
                    List<GameCard> choices = new List<GameCard>();
                    foreach (int id in packet.specialArgs)
                    {
                        GameCard c = sGame.GetCardWithID(id);
                        if (c == null) Debug.LogError($"Player tried to search card to list with invalid id {id}");
                        else choices.Add(c);
                    }

                    if (sGame.CurrEffect?.CurrSubeffect is ChooseFromListSubeffect listEff)
                    {
                        listEff.AddListIfLegal(choices);
                    }

                    if(sGame.CurrEffect?.CurrSubeffect is DeckTargetSubeffect deckTgtSubeff)
                    {
                        deckTgtSubeff.AddTargetIfLegal(choices.FirstOrDefault());
                    }
                    break;
                case Packet.Command.OptionalTrigger:
                    sGame.EffectsController.OptionalTriggerAnswered(packet.Answer);
                    break;
                case Packet.Command.ChooseEffectOption:
                    if(sGame.CurrEffect?.CurrSubeffect is ChooseOptionSubeffect optionSubeff)
                    {
                        optionSubeff.ChooseOption(packet.EffectOption);
                    }
                    break;
                #endregion
                #region debug commands
                case Packet.Command.Topdeck:
                    DebugTopdeck(packet.cardID);
                    break;
                case Packet.Command.Discard:
                    DebugDiscard(packet.cardID);
                    break;
                case Packet.Command.Rehand:
                    DebugRehand(packet.cardID);
                    break;
                case Packet.Command.Draw:
                    DebugDraw();
                    break;
                case Packet.Command.SetNESW:
                    var (n, e, s, w) = packet.Stats;
                    DebugSetNESW(packet.cardID, n, e, s, w);
                    break;
                case Packet.Command.SetPips:
                    DebugSetPips(packet.Pips);
                    break;
                case Packet.Command.ActivateEffect:
                    var eff = sGame.GetCardWithID(packet.cardID)?.Effects.ElementAt(packet.EffIndex) as ServerEffect;
                    Player.TryActivateEffect(eff);
                    break;
                #endregion
                default:
                    Debug.Log($"Invalid command {packet.command} to server from {Player.index}");
                    break;
            }
        }

        public static int InvertIndexForController(int index, int controller)
        {
            if (controller == 0) return index;
            else return 6 - index;
        }

        #region Debug Actions
        public void DebugTopdeck(int cardID)
        {
            if (!sGame.uiCtrl.DebugMode)
            {
                Debug.LogError($"Tried to debug topdeck card with id {cardID} while NOT in debug mode!");
                ServerNotifier.NotifyPutBack();
                return;
            }
            Debug.LogWarning($"Debug topdecking card with id {cardID}");
            sGame.GetCardWithID(cardID).Topdeck();
        }

        public void DebugDiscard(int cardID)
        {
            if (!sGame.uiCtrl.DebugMode)
            {
                Debug.LogError($"Tried to debug discard card with id {cardID} while NOT in debug mode!");
                ServerNotifier.NotifyPutBack();
                return;
            }
            Debug.LogWarning($"Debug discarding card with id {cardID}");
            sGame.GetCardWithID(cardID).Discard();
        }

        public void DebugRehand(int cardID)
        {
            if (!sGame.uiCtrl.DebugMode)
            {
                Debug.LogError($"Tried to debug rehand card with id {cardID} while NOT in debug mode!");
                ServerNotifier.NotifyPutBack();
                return;
            }
            Debug.LogWarning($"Debug rehanding card with id {cardID}");
            sGame.GetCardWithID(cardID).Rehand();
        }

        public void DebugDraw()
        {
            if (!sGame.uiCtrl.DebugMode)
            {
                Debug.LogError("Tried to debug draw card while NOT in debug mode!");
                ServerNotifier.NotifyPutBack();
                return;
            }
            Debug.LogWarning("Debug drawing");
            //draw and store what was drawn
            GameCard toDraw = sGame.Draw(Player.index);
            if (toDraw == null) return; //deck was empty
            ServerNotifier.NotifyDraw(toDraw);
            sGame.EffectsController.CheckForResponse();
        }

        public void DebugSetPips(int pipsToSet)
        {
            if (!sGame.uiCtrl.DebugMode)
            {
                Debug.LogError($"Tried to debug give {pipsToSet} pips while NOT in debug mode!");
                ServerNotifier.NotifyPutBack();
                return;
            }
            Debug.LogWarning($"Debug setting pips to {pipsToSet}");
            sGame.GivePlayerPips(Player, pipsToSet);
        }

        public void DebugSetNESW(int cardID, int n, int e, int s, int w)
        {
            Debug.LogWarning($"Debug setting NESW of card with id {cardID}");
            sGame.GetCardWithID(cardID).SetCharStats(n, e, s, w);
        }
        #endregion Debug Actions
    }
}
