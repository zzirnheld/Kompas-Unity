using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Networking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public static readonly string[] DontLogThesePackets =
        {
            Packet.PassPriority
        };

        public ServerPlayer Player;
        public ServerGame sGame;
        public ServerNotifier ServerNotifier;
        public ServerAwaiter serverAwaiter;

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
                case Packet.HandSizeChoices:      return JsonUtility.FromJson<SendHandSizeChoicesServerPacket>(json);

                //effects
                case Packet.CardTargetChosen:        return JsonUtility.FromJson<CardTargetServerPacket>(json);
                case Packet.SpaceTargetChosen:       return JsonUtility.FromJson<SpaceTargetServerPacket>(json);
                case Packet.XSelectionChosen:        return JsonUtility.FromJson<SelectXServerPacket>(json);
                case Packet.DeclineAnotherTarget:    return JsonUtility.FromJson<DeclineAnotherTargetServerPacket>(json);
                case Packet.ListChoicesChosen:       return JsonUtility.FromJson<ListChoicesServerPacket>(json);
                case Packet.OptionalTriggerResponse: return JsonUtility.FromJson<OptionalTriggerAnswerServerPacket>(json);
                case Packet.ChooseEffectOption:      return JsonUtility.FromJson<EffectOptionResponseServerPacket>(json);
                case Packet.PassPriority:            return JsonUtility.FromJson<PassPriorityServerPacket>(json);
                case Packet.ChooseTriggerOrder:      return JsonUtility.FromJson<TriggerOrderResponseServerPacket>(json);

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

        protected override async void Update()
        {
            //Debug.Log("SERVER NET CTRL UPDATE");
            base.Update();
            if (packets.Count != 0) await ProcessPacket(packets.Dequeue());
            if (sGame.Players.Any(p => p.TcpClient != null && !p.TcpClient.Connected)) Destroy(sGame.gameObject); //TODO notify player that no
        }

        public override async Task ProcessPacket((string command, string json) packetInfo)
        {
            if(packetInfo.command == Packet.Invalid)
            {
                Debug.LogError("Invalid packet");
                return;
            }

            if(!DontLogThesePackets.Contains(packetInfo.command)) Debug.Log($"Processing {packetInfo.json} from {Player.Index}");

            var packet = FromJson(packetInfo.command, packetInfo.json);
            await packet.Execute(sGame, Player, serverAwaiter);
        }
    }
}
