using KompasClient.GameCore;
using KompasCore.Networking;
using System;
using System.Net;
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
            catch(Exception e)
            {
                Debug.LogError($"Failed to connect to {ip}. Stack trace:\n{e.StackTrace}");
                ClientGame.clientUICtrl.ShowConnectUI();
            }
            Debug.Log("Connected");
            if(tcpClient.Connected) ClientGame.clientUICtrl.ShowConnectedWaitingUI();
            connecting = false;
        }

        protected override void Update()
        {
            base.Update();
            if (packets.Count != 0) ProcessPacket(packets.Dequeue());
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
                case Packet.SetTurnPlayer: return JsonUtility.FromJson<SetTurnPlayerClientPacket>(json);
                case Packet.PutCardsBack: return JsonUtility.FromJson<PutCardsBackClientPacket>(json);
                case Packet.AttackStarted: return JsonUtility.FromJson<AttackStartedClientPacket>(json);
                case Packet.HandSizeToStack: return JsonUtility.FromJson<HandSizeToStackClientPacket>(json);
                case Packet.ChooseHandSize: return JsonUtility.FromJson<GetHandSizeChoicesClientPacket>(json);
                case Packet.SetDeckCount: return JsonUtility.FromJson<SetDeckCountClientPacket>(json);

                //card addition/deletion
                case Packet.AddCard: return JsonUtility.FromJson<AddCardClientPacket>(json);
                case Packet.DeleteCard: return JsonUtility.FromJson<DeleteCardClientPacket>(json);
                case Packet.ChangeEnemyHandCount: return JsonUtility.FromJson<ChangeEnemyHandCountClientPacket>(json);

                //card movement
                case Packet.KnownToEnemy: return JsonUtility.FromJson<UpdateKnownToEnemyClientPacket>(json);
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
                case Packet.ResetCard: return JsonUtility.FromJson<ResetCardClientPacket>(json);
                case Packet.ChangeCardController: return JsonUtility.FromJson<ChangeCardControllerClientPacket>(json);
                case Packet.SetPips: return JsonUtility.FromJson<SetPipsClientPacket>(json);
                case Packet.AttacksThisTurn: return JsonUtility.FromJson<AttacksThisTurnClientPacket>(json);
                case Packet.SpacesMoved: return JsonUtility.FromJson<SpacesMovedClientPacket>(json);

                //effects
                    //targeting
                case Packet.GetCardTarget: return JsonUtility.FromJson<GetCardTargetClientPacket>(json);
                case Packet.GetSpaceTarget: return JsonUtility.FromJson<GetSpaceTargetClientPacket>(json);
                    //other
                case Packet.GetEffectOption: return JsonUtility.FromJson<GetEffectOptionClientPacket>(json);
                case Packet.EffectResolving: return JsonUtility.FromJson<EffectResolvingClientPacket>(json);
                case Packet.EffectActivated: return JsonUtility.FromJson<EffectActivatedClientPacket>(json);
                case Packet.RemoveStackEntry: return JsonUtility.FromJson<RemoveStackEntryClientPacket>(json);
                case Packet.SetEffectsX: return JsonUtility.FromJson<SetEffectsXClientPacket>(json);
                case Packet.PlayerChooseX: return JsonUtility.FromJson<GetPlayerChooseXClientPacket>(json);
                case Packet.TargetAccepted: return JsonUtility.FromJson<TargetAcceptedClientPacket>(json);
                case Packet.AddTarget: return JsonUtility.FromJson<AddTargetClientPacket>(json);
                case Packet.RemoveTarget: return JsonUtility.FromJson<RemoveTargetClientPacket>(json);
                case Packet.ToggleDecliningTarget: return JsonUtility.FromJson<ToggleDecliningTargetClientPacket>(json);
                case Packet.DiscardSimples: return JsonUtility.FromJson<DiscardSimplesClientPacket>(json);
                case Packet.StackEmpty: return JsonUtility.FromJson<StackEmptyClientPacket>(json);
                case Packet.EffectImpossible: return JsonUtility.FromJson<EffectImpossibleClientPacket>(json);
                case Packet.OptionalTrigger: return JsonUtility.FromJson<OptionalTriggerClientPacket>(json);
                case Packet.ToggleAllowResponses: return JsonUtility.FromJson<ToggleAllowResponsesClientPacket>(json);
                case Packet.GetTriggerOrder: return JsonUtility.FromJson<GetTriggerOrderClientPacket>(json);

                //misc
                default: throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client");
            }
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
            ClientGame.RefreshShownCards();
            ClientGame.clientUICtrl.RefreshShownCardInfo();
            ClientGame.clientUICtrl.cardInfoViewUICtrl.searchUICtrl.ReshowSearchShown();

            return Task.CompletedTask;
        }
    }
}