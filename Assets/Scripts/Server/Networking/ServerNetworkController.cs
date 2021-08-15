using KompasCore.Networking;
using KompasServer.GameCore;
using Newtonsoft.Json;
using System.Linq;
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
                case Packet.SetDeck: return JsonConvert.DeserializeObject<SetDeckServerPacket>(json);

                //player actions
                case Packet.PlayAction:           return JsonConvert.DeserializeObject<PlayActionServerPacket>(json);
                case Packet.AugmentAction:        return JsonConvert.DeserializeObject<AugmentActionServerPacket>(json);
                case Packet.MoveAction:           return JsonConvert.DeserializeObject<MoveActionServerPacket>(json);
                case Packet.AttackAction:         return JsonConvert.DeserializeObject<AttackActionServerPacket>(json);
                case Packet.EndTurnAction:        return JsonConvert.DeserializeObject<EndTurnActionServerPacket>(json);
                case Packet.ActivateEffectAction: return JsonConvert.DeserializeObject<ActivateEffectActionServerPacket>(json);
                case Packet.HandSizeChoices:      return JsonConvert.DeserializeObject<SendHandSizeChoicesServerPacket>(json);

                //effects
                case Packet.CardTargetChosen:        return JsonConvert.DeserializeObject<CardTargetServerPacket>(json);
                case Packet.SpaceTargetChosen:       return JsonConvert.DeserializeObject<SpaceTargetServerPacket>(json);
                case Packet.XSelectionChosen:        return JsonConvert.DeserializeObject<SelectXServerPacket>(json);
                case Packet.DeclineAnotherTarget:    return JsonConvert.DeserializeObject<DeclineAnotherTargetServerPacket>(json);
                case Packet.ListChoicesChosen:       return JsonConvert.DeserializeObject<ListChoicesServerPacket>(json);
                case Packet.OptionalTriggerResponse: return JsonConvert.DeserializeObject<OptionalTriggerAnswerServerPacket>(json);
                case Packet.ChooseEffectOption:      return JsonConvert.DeserializeObject<EffectOptionResponseServerPacket>(json);
                case Packet.PassPriority:            return JsonConvert.DeserializeObject<PassPriorityServerPacket>(json);
                case Packet.ChooseTriggerOrder:      return JsonConvert.DeserializeObject<TriggerOrderResponseServerPacket>(json);

                //debug
                case Packet.DebugTopdeck: return JsonConvert.DeserializeObject<DebugTopdeckServerPacket>(json);
                case Packet.DebugDiscard: return JsonConvert.DeserializeObject<DebugDiscardServerPacket>(json);
                case Packet.DebugRehand:  return JsonConvert.DeserializeObject<DebugRehandServerPacket>(json);
                case Packet.DebugDraw:    return JsonConvert.DeserializeObject<DebugDrawServerPacket>(json);
                case Packet.DebugSetNESW: return JsonConvert.DeserializeObject<DebugSetNESWServerPacket>(json);
                case Packet.DebugSetPips: return JsonConvert.DeserializeObject<DebugSetPipsServerPacket>(json);

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

            if(!DontLogThesePackets.Contains(packetInfo.command)) Debug.Log($"Processing {packetInfo.json} from {Player.index}");

            var packet = FromJson(packetInfo.command, packetInfo.json);
            await packet.Execute(sGame, Player, serverAwaiter);
        }
    }
}
