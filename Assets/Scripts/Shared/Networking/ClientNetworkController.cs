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

        public int X { get; private set; }

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

            /*
                case Packet.Command.Negate:
                    card?.SetNegated(packet.Answer);
                    break;
                case Packet.Command.Activate:
                    card?.SetActivated(packet.Answer);
                    break;
                case Packet.Command.ChangeControl:
                    if (card != null) card.Controller = ClientGame.Players[packet.ControllerIndex];
                    break;
                case Packet.Command.SetPips:
                    ClientGame.SetFriendlyPips(packet.Pips);
                    break;
                case Packet.Command.SetEnemyPips:
                    ClientGame.SetEnemyPips(packet.Pips);
                    break;
                case Packet.Command.PutBack:
                    ClientGame.PutCardsBack();
                    break;
                case Packet.Command.EndTurn:
                    ClientGame.EndTurn();
                    break;
                case Packet.Command.RequestBoardTarget:
                    ClientGame.targetMode = Game.TargetMode.BoardTarget;
                    ClientGame.CurrCardRestriction = packet.GetBoardRestriction(ClientGame);
                    ClientGame.clientUICtrl.SetCurrState("Choose Board Target", ClientGame.CurrCardRestriction.blurb);
                    break;
                case Packet.Command.RequestHandTarget:
                    ClientGame.targetMode = Game.TargetMode.HandTarget;
                    ClientGame.CurrCardRestriction = packet.GetCardRestriction(ClientGame);
                    ClientGame.clientUICtrl.SetCurrState("Choose Hand Target", ClientGame.CurrCardRestriction.blurb);
                    break;
                case Packet.Command.RequestDeckTarget:
                    ClientGame.targetMode = Game.TargetMode.OnHold;
                    Debug.Log($"Deck target for Eff index: {packet.EffIndex} subeff index {packet.SubeffIndex}");
                    ClientGame.CurrCardRestriction = packet.GetCardRestriction(ClientGame);
                    List<GameCard> toSearch = ClientGame.friendlyDeckCtrl.CardsThatFitRestriction(ClientGame.CurrCardRestriction);
                    ClientGame.clientUICtrl.StartSearch(toSearch);
                    ClientGame.clientUICtrl.SetCurrState("Choose Deck Target", ClientGame?.CurrCardRestriction?.blurb);
                    break;
                case Packet.Command.RequestDiscardTarget:
                    ClientGame.targetMode = Game.TargetMode.OnHold;
                    ClientGame.CurrCardRestriction = packet.GetCardRestriction(ClientGame);
                    List<GameCard> discardToSearch = ClientGame.friendlyDiscardCtrl.CardsThatFitRestriction(ClientGame.CurrCardRestriction);
                    ClientGame.clientUICtrl.StartSearch(discardToSearch);
                    ClientGame.clientUICtrl.SetCurrState("Choose Discard Target", ClientGame.CurrCardRestriction.blurb);
                    break;
                case Packet.Command.GetChoicesFromList:
                    ClientGame.targetMode = Game.TargetMode.OnHold;
                    int[] cardIDs = packet.specialArgs;
                    List<GameCard> choicesToPick = new List<GameCard>();
                    foreach (int id in cardIDs)
                    {
                        GameCard c = ClientGame.GetCardWithID(id);
                        if (c == null) Debug.LogError($"Tried to start a list search including card with invalid id {id}");
                        else choicesToPick.Add(c);
                    }
                    var listRestriction = packet.GetListRestriction(ClientGame);
                    ClientGame.clientUICtrl.StartSearch(choicesToPick, packet.MaxNum);
                    ClientGame.clientUICtrl.SetCurrState($"Choose Target for Effect of {listRestriction?.Subeffect?.Source?.CardName}",
                        ClientGame.CurrCardRestriction?.blurb);
                    break;
                case Packet.Command.ChooseEffectOption:
                    //TODO catch out of bounds errors, in case of malicious packets?
                    var subeff = card.Effects.ElementAt(packet.normalArgs[0]).Subeffects[packet.normalArgs[1]]
                        as DummyChooseOptionSubeffect;
                    if (subeff == null)
                    {
                        Debug.LogError($"Subeffect for card id {packet.cardID}, effect index {packet.normalArgs[0]}, subeffect index {packet.normalArgs[1]} " +
                            $"is null or not dummy choose option subeffect");
                    }
                    ClientGame.clientUICtrl.ShowEffectOptions(subeff);
                    break;
                case Packet.Command.SpaceTarget:
                    ClientGame.targetMode = Game.TargetMode.SpaceTarget;
                    ClientGame.CurrSpaceRestriction = packet.GetSpaceRestriction(ClientGame);
                    //TODO display based on that space
                    ClientGame.clientUICtrl.SetCurrState("Choose Space Target", ClientGame.CurrSpaceRestriction.blurb);
                    break;
                case Packet.Command.SetEffectsX:
                    Debug.Log("Setting X to " + packet.EffectX);
                    if (card != null) card.Effects.ElementAt(packet.EffIndex).X = packet.EffectX;
                    X = packet.EffectX;
                    break;
                case Packet.Command.PlayerSetX:
                    ClientGame.clientUICtrl.GetXForEffect();
                    break;
                case Packet.Command.TargetAccepted:
                    ClientGame.targetMode = Game.TargetMode.Free;
                    ClientGame.CurrCardRestriction = null;
                    ClientGame.CurrSpaceRestriction = null;
                    ClientGame.clientUICtrl.SetCurrState("Target Accepted");
                    break;
                case Packet.Command.Target:
                    var target = ClientGame.GetCardWithID(packet.normalArgs[1]);
                    if (target != null) card.Effects.ElementAt(packet.EffIndex).AddTarget(target);
                    break;
                case Packet.Command.EnableDecliningTarget:
                    ClientGame.clientUICtrl.EnableDecliningTarget();
                    break;
                case Packet.Command.DisableDecliningTarget:
                    ClientGame.clientUICtrl.DisableDecliningTarget();
                    break;
                case Packet.Command.DiscardSimples:
                    ClientGame.boardCtrl.DiscardSimples();
                    break;
                case Packet.Command.EffectResolving:
                    var eff = card.Effects.ElementAt(packet.EffIndex);
                    eff.Controller = ClientGame.Players[packet.normalArgs[1]];
                    ClientGame.clientUICtrl.SetCurrState($"Resolving Effect of {card?.CardName}", $"{eff.Blurb}");
                    break;
                case Packet.Command.StackEmpty:
                    ClientGame.clientUICtrl.SetCurrState(string.Empty);
                    break;
                case Packet.Command.EffectImpossible:
                    ClientGame.clientUICtrl.SetCurrState("Effect Impossible");
                    break;
                case Packet.Command.OptionalTrigger:
                    ClientTrigger t = card.Effects.ElementAt(packet.EffIndex).Trigger as ClientTrigger;
                    t.ClientEffect.ClientController = Friendly;
                    ClientGame.clientUICtrl.ShowOptionalTrigger(t, packet.EffIndex);
                    break;
                case Packet.Command.Response:
                    ClientGame.clientUICtrl.GetResponse();
                    break;
                case Packet.Command.NoMoreResponse:
                    ClientGame.clientUICtrl.UngetResponse();
                    break;
                default:
                    Debug.LogError($"Unrecognized command {packet.command} sent to client");
                    break;
            }*/
        }
    }
}