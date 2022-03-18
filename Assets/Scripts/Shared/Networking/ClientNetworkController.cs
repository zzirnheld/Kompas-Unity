using KompasClient.GameCore;
using KompasCore.Networking;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasClient.Networking
{
    public class ClientNetworkController : NetworkController
    {
        public ClientGame ClientGame;
        private bool connecting = false;

        public async void Connect(string ip)
        {
            if (connecting) return;

            connecting = true;
            var address = IPAddress.Parse(ip);
            tcpClient = new System.Net.Sockets.TcpClient();
            try
            {
                await tcpClient.ConnectAsync(address, port);
            }
            catch (SocketException e)
            {
                Debug.LogError($"Failed to connect to {ip}. Stack trace:\n{e.StackTrace}");
                ClientGame.clientUICtrl.ShowConnectUI();
            }
            Debug.Log("Connected");
            if (tcpClient.Connected) ClientGame.clientUICtrl.ShowConnectedWaitingUI();
            connecting = false;
        }

        protected override void Update()
        {
            base.Update();
            if (packets.Count != 0) ProcessPacket(packets.Dequeue());
        }

        private IClientOrderPacket FromJson(string command, string json)
        {
            return command switch
            {
                //game start
                Packet.GetDeck => JsonConvert.DeserializeObject<GetDeckClientPacket>(json),
                Packet.DeckAccepted => JsonConvert.DeserializeObject<DeckAcceptedClientPacket>(json),
                Packet.SetAvatar => JsonConvert.DeserializeObject<SetAvatarClientPacket>(json),
                Packet.SetFirstTurnPlayer => JsonConvert.DeserializeObject<SetFirstPlayerClientPacket>(json),
                //gamestate
                Packet.SetLeyload => JsonConvert.DeserializeObject<SetLeyloadClientPacket>(json),
                Packet.SetTurnPlayer => JsonConvert.DeserializeObject<SetTurnPlayerClientPacket>(json),
                Packet.PutCardsBack => JsonConvert.DeserializeObject<PutCardsBackClientPacket>(json),
                Packet.AttackStarted => JsonConvert.DeserializeObject<AttackStartedClientPacket>(json),
                Packet.HandSizeToStack => JsonConvert.DeserializeObject<HandSizeToStackClientPacket>(json),
                Packet.ChooseHandSize => JsonConvert.DeserializeObject<GetHandSizeChoicesClientPacket>(json),
                Packet.SetDeckCount => JsonConvert.DeserializeObject<SetDeckCountClientPacket>(json),
                //card addition/deletion
                Packet.AddCard => JsonConvert.DeserializeObject<AddCardClientPacket>(json),
                Packet.DeleteCard => JsonConvert.DeserializeObject<DeleteCardClientPacket>(json),
                Packet.ChangeEnemyHandCount => JsonConvert.DeserializeObject<ChangeEnemyHandCountClientPacket>(json),
                //card movement
                Packet.KnownToEnemy => JsonConvert.DeserializeObject<UpdateKnownToEnemyClientPacket>(json),
                //public areas
                Packet.PlayCard => JsonConvert.DeserializeObject<PlayCardClientPacket>(json),
                Packet.AttachCard => JsonConvert.DeserializeObject<AttachCardClientPacket>(json),
                Packet.MoveCard => JsonConvert.DeserializeObject<MoveCardClientPacket>(json),
                Packet.DiscardCard => JsonConvert.DeserializeObject<DiscardCardClientPacket>(json),
                Packet.AnnihilateCard => JsonConvert.DeserializeObject<AnnihilateCardClientPacket>(json),
                //private areas
                Packet.RehandCard => JsonConvert.DeserializeObject<RehandCardClientPacket>(json),
                Packet.TopdeckCard => JsonConvert.DeserializeObject<TopdeckCardClientPacket>(json),
                Packet.ReshuffleCard => JsonConvert.DeserializeObject<ReshuffleCardClientPacket>(json),
                Packet.BottomdeckCard => JsonConvert.DeserializeObject<BottomdeckCardClientPacket>(json),
                //stats
                Packet.UpdateCardNumericStats => JsonConvert.DeserializeObject<ChangeCardNumericStatsClientPacket>(json),
                Packet.NegateCard => JsonConvert.DeserializeObject<NegateCardClientPacket>(json),
                Packet.ActivateCard => JsonConvert.DeserializeObject<ActivateCardClientPacket>(json),
                Packet.ChangeCardController => JsonConvert.DeserializeObject<ChangeCardControllerClientPacket>(json),
                Packet.SetPips => JsonConvert.DeserializeObject<SetPipsClientPacket>(json),
                Packet.AttacksThisTurn => JsonConvert.DeserializeObject<AttacksThisTurnClientPacket>(json),
                Packet.SpacesMoved => JsonConvert.DeserializeObject<SpacesMovedClientPacket>(json),
                //effects
                //targeting
                Packet.GetCardTarget => JsonConvert.DeserializeObject<GetCardTargetClientPacket>(json),
                Packet.GetSpaceTarget => JsonConvert.DeserializeObject<GetSpaceTargetClientPacket>(json),
                //other
                Packet.GetEffectOption => JsonConvert.DeserializeObject<GetEffectOptionClientPacket>(json),
                Packet.EffectResolving => JsonConvert.DeserializeObject<EffectResolvingClientPacket>(json),
                Packet.EffectActivated => JsonConvert.DeserializeObject<EffectActivatedClientPacket>(json),
                Packet.RemoveStackEntry => JsonConvert.DeserializeObject<RemoveStackEntryClientPacket>(json),
                Packet.SetEffectsX => JsonConvert.DeserializeObject<SetEffectsXClientPacket>(json),
                Packet.PlayerChooseX => JsonConvert.DeserializeObject<GetPlayerChooseXClientPacket>(json),
                Packet.TargetAccepted => JsonConvert.DeserializeObject<TargetAcceptedClientPacket>(json),
                Packet.AddTarget => JsonConvert.DeserializeObject<AddTargetClientPacket>(json),
                Packet.RemoveTarget => JsonConvert.DeserializeObject<RemoveTargetClientPacket>(json),
                Packet.ToggleDecliningTarget => JsonConvert.DeserializeObject<ToggleDecliningTargetClientPacket>(json),
                Packet.DiscardSimples => JsonConvert.DeserializeObject<DiscardSimplesClientPacket>(json),
                Packet.StackEmpty => JsonConvert.DeserializeObject<StackEmptyClientPacket>(json),
                Packet.EffectImpossible => JsonConvert.DeserializeObject<EffectImpossibleClientPacket>(json),
                Packet.OptionalTrigger => JsonConvert.DeserializeObject<OptionalTriggerClientPacket>(json),
                Packet.ToggleAllowResponses => JsonConvert.DeserializeObject<ToggleAllowResponsesClientPacket>(json),
                Packet.GetTriggerOrder => JsonConvert.DeserializeObject<GetTriggerOrderClientPacket>(json),
                //misc
                _ => throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client"),
            };
        }

        public override Task ProcessPacket((string command, string json) packetInfo)
        {
            if (packetInfo.command == Packet.Invalid)
            {
                Debug.LogError("Invalid packet");
                return Task.CompletedTask;
            }

            var p = FromJson(packetInfo.command, packetInfo.json);
            // Debug.Log($"Parsing packet {p}");
            p.Execute(ClientGame);

            //clean up any visual differences after the latest packet.
            //TODO make this more efficient, probably with dirty lists
            ClientGame.Refresh();

            return Task.CompletedTask;
        }
    }
}