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
                case Packet.PlayAction:           return JsonUtility.FromJson<PlayActionServerPacket>(json);
                case Packet.AugmentAction:        return JsonUtility.FromJson<AugmentActionServerPacket>(json);
                case Packet.MoveAction:           return JsonUtility.FromJson<MoveActionServerPacket>(json);
                case Packet.AttackAction:         return JsonUtility.FromJson<AttackActionServerPacket>(json);
                case Packet.EndTurnAction:        return JsonUtility.FromJson<EndTurnActionServerPacket>(json);
                case Packet.ActivateEffectAction: return JsonUtility.FromJson<ActivateEffectActionServerPacket>(json);

                //effects
                case Packet.CardTargetChosen:        return JsonUtility.FromJson<CardTargetServerPacket>(json);
                case Packet.SpaceTargetChosen:       return JsonUtility.FromJson<SpaceTargetServerPacket>(json);
                case Packet.XSelectionChosen:        return JsonUtility.FromJson<SelectXServerPacket>(json);
                case Packet.DeclineAnotherTarget:    return JsonUtility.FromJson<DeclineAnotherTargetServerPacket>(json);
                case Packet.ListChoicesChosen:       return JsonUtility.FromJson<ListChoicesServerPacket>(json);
                case Packet.OptionalTriggerResponse: return JsonUtility.FromJson<OptionalTriggerAnswerServerPacket>(json);
                case Packet.ChooseEffectOption:      return JsonUtility.FromJson<EffectOptionResponseServerPacket>(json);
                case Packet.PassPriority:      return JsonUtility.FromJson<PassPriorityServerPacket>(json);

                //debug
                case Packet.DebugTopdeck: return JsonUtility.FromJson<DebugTopdeckServerPacket>(json);
                case Packet.DebugDiscard: return JsonUtility.FromJson<DebugDiscardServerPacket>(json);
                case Packet.DebugRehand:  return JsonUtility.FromJson<DebugRehandServerPacket>(json);
                case Packet.DebugDraw:    return JsonUtility.FromJson<DebugDrawServerPacket>(json);
                case Packet.DebugSetNESW: return JsonUtility.FromJson<DebugSetNESWServerPacket>(json);
                case Packet.DebugSetPips: return JsonUtility.FromJson<DebugSetPipsServerPacket>(json);

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
    }
}
