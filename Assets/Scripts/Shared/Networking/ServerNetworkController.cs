using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasServer.Networking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public ServerPlayer Player;
        public ServerGame sGame;
        public ServerNotifier ServerNotifier;

        private IServerOrderPacket FromJson(string command, string json)
        {
            switch (command)
            {
                //game start
                case Packet.SetDeck: return JsonUtility.FromJson<SetDeckServerPacket>(json);

                //player actions
                case Packet.PlayAction: return JsonUtility.FromJson<PlayActionServerPacket>(json);
                case Packet.AugmentAction: return JsonUtility.FromJson<AugmentActionServerPacket>(json);
                case Packet.MoveAction: return JsonUtility.FromJson<MoveActionServerPacket>(json);
                case Packet.AttackAction: return JsonUtility.FromJson<AttackActionServerPacket>(json);
                case Packet.EndTurnAction: return JsonUtility.FromJson<EndTurnActionServerPacket>(json);

                //effects
                case Packet.CardTargetChosen: return JsonUtility.FromJson<CardTargetServerPacket>(json);
                case Packet.SpaceTargetChosen: return JsonUtility.FromJson<SpaceTargetServerPacket>(json);
                case Packet.XSelectionChosen: return JsonUtility.FromJson<SelectXServerPacket>(json);
                case Packet.DeclineAnotherTarget: return JsonUtility.FromJson<DeclineAnotherTargetServerPacket>(json);
                case Packet.ListChoicesChosen: return JsonUtility.FromJson<ListChoicesServerPacket>(json);
                case Packet.OptionalTriggerResponse: return JsonUtility.FromJson<OptionalTriggerAnswerServerPacket>(json);
                case Packet.ChooseEffectOption: return JsonUtility.FromJson<EffectOptionResponseServerPacket>(json);

                //misc
                default: throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client");
            }
        }

        public override void ProcessPacket((string command, string json) packetInfo)
        {
            if(packetInfo.command == Packet.Invalid)
            {
                Debug.LogError("Invalid packet");
                return;
            }

            var packet = FromJson(packetInfo.command, packetInfo.json);
            packet.Execute(sGame, Player);
        }
        /*
        //TODO make code that checks if ready to resolve the stack (both players have no responses/have declined priority in a row)
        public override void ProcessPacket()
                #region effect commands
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

    }*/

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
