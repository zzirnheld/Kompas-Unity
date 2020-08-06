using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.GameCore;
using KompasCore.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace KompasClient.Networking
{
    public class ClientNetworkController : NetworkController
    {
        public ClientGame ClientGame;

        public int X { get; set; }

        public void Connect(string ip)
        {
            Debug.Log($"Connecting to {ip} on a random port");
            var address = IPAddress.Parse(ip);
            tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.Connect(address, port);
            Debug.Log("Connected");
        }

        public override void Update()
        {
            base.Update();
        }

        private IClientOrderPacket FromJson(string command, string json)
        {
            switch (command)
            {
                //game start
                case Packet.GetDeck: return JsonUtility.FromJson<GetDeckClientPacket>(json);
                case Packet.DeckAccepted: return JsonUtility.FromJson<DeckAcceptedClientPacket>(json);
                case Packet.SetAvatar: return JsonUtility.FromJson<SetAvatarClientPacket>(json);
                case Packet.SetFirstTurnPlayer: return JsonUtility.FromJson<SetFirstPlayerClientPacket>(json);

                //gamestate
                case Packet.SetLeyload: return JsonUtility.FromJson<SetLeyloadClientPacket>(json);

                //card addition/deletion
                case Packet.AddCard: return JsonUtility.FromJson<AddCardClientPacket>(json);
                case Packet.DeleteCard: return JsonUtility.FromJson<DeleteCardClientPacket>(json);
                case Packet.ChangeEnemyHandCount: return JsonUtility.FromJson<ChangeEnemyHandCountClientPacket>(json);

                //card movement
                    //public areas
                case Packet.PlayCard: return JsonUtility.FromJson<PlayCardClientPacket>(json);
                case Packet.AttachCard: return JsonUtility.FromJson<AttachCardClientPacket>(json);
                case Packet.MoveCard: return JsonUtility.FromJson<MoveCardClientPacket>(json);
                case Packet.DiscardCard: return JsonUtility.FromJson<DiscardCardClientPacket>(json);
                case Packet.AnnihilateCard: return JsonUtility.FromJson<AnnihilateCardClientPacket>(json);
                    //private areas
                case Packet.RehandCard: return JsonUtility.FromJson<RehandCardClientPacket>(json);
                case Packet.TopdeckCard: return JsonUtility.FromJson<TopdeckCardClientPacket>(json);
                case Packet.ReshuffleCard: return JsonUtility.FromJson<ReshuffleCardClientPacket>(json);
                case Packet.BottomdeckCard: return JsonUtility.FromJson<BottomdeckCardClientPacket>(json);

                //stats
                case Packet.UpdateCardNumericStats: return JsonUtility.FromJson<ChangeCardNumericStatsClientPacket>(json);
                case Packet.NegateCard: return JsonUtility.FromJson<NegateCardClientPacket>(json);
                case Packet.ActivateCard: return JsonUtility.FromJson<ActivateCardClientPacket>(json);
                case Packet.ChangeCardController: return JsonUtility.FromJson<ChangeCardControllerClientPacket>(json);
                case Packet.SetPips: return JsonUtility.FromJson<SetPipsClientPacket>(json);

                //effects
                    //targeting
                case Packet.GetBoardTarget: return JsonUtility.FromJson<GetBoardTargetClientPacket>(json);
                case Packet.GetHandTarget: return JsonUtility.FromJson<GetHandTargetClientPacket>(json);
                case Packet.GetDeckTarget: return JsonUtility.FromJson<GetDeckTargetClientPacket>(json);
                case Packet.GetDiscardTarget: return JsonUtility.FromJson<DiscardCardClientPacket>(json);
                case Packet.GetListChoices: return JsonUtility.FromJson<GetListChoicesClientPacket>(json);
                case Packet.GetSpaceTarget: return JsonUtility.FromJson<GetSpaceTargetClientPacket>(json);
                    //other
                case Packet.GetEffectOption: return JsonUtility.FromJson<GetEffectOptionClientPacket>(json);
                case Packet.EffectResolving: return JsonUtility.FromJson<EffectResolvingClientPacket>(json);
                case Packet.SetEffectsX: return JsonUtility.FromJson<SetEffectsXClientPacket>(json);
                case Packet.PlayerChooseX: return JsonUtility.FromJson<GetPlayerChooseXClientPacket>(json);
                case Packet.TargetAccepted: return JsonUtility.FromJson<TargetAcceptedClientPacket>(json);
                case Packet.AddTarget: return JsonUtility.FromJson<AddTargetClientPacket>(json);
                case Packet.ToggleDecliningTarget: return JsonUtility.FromJson<ToggleDecliningTargetClientPacket>(json);
                case Packet.DiscardSimples: return JsonUtility.FromJson<DiscardSimplesClientPacket>(json);
                case Packet.StackEmpty: return JsonUtility.FromJson<StackEmptyClientPacket>(json);
                case Packet.EffectImpossible: return JsonUtility.FromJson<EffectImpossibleClientPacket>(json);
                case Packet.OptionalTrigger: return JsonUtility.FromJson<OptionalTriggerClientPacket>(json);
                case Packet.ToggleAllowResponses: return JsonUtility.FromJson<ToggleAllowResponsesClientPacket>(json);

                //misc
                default: throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client");
            }
        }

        public override void ProcessPacket((string command, string json) packetInfo)
        {
            if (packetInfo.command == Packet.Invalid)
            {
                Debug.LogError("Invalid packet");
                return;
            }

            var p = FromJson(packetInfo.command, packetInfo.json);
            p.Execute(ClientGame);
        }
    }
}