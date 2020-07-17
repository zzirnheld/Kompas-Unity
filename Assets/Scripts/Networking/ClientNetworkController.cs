using System;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using KompasNetworking;


public class ClientNetworkController : NetworkController {
    public ClientGame ClientGame;
    private ClientPlayer Friendly => ClientGame.ClientPlayers[0];
    private ClientPlayer Enemy => ClientGame.ClientPlayers[1];
    
    public int X { get; private set; }

    public override void Awake()
    {
        base.Awake();
    }

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

    public override void ProcessPacket(Packet packet)
    {
        if (packet == null)
        {
            Debug.Log("Null packet");
            return;
        }
        var card = ClientGame.GetCardWithID(packet.cardID);
        Debug.Log($"Parsing command {packet.command} for {packet.cardID}. That's the card {card?.CardName}");

        switch (packet.command)
        {
            #region game start
            case Packet.Command.GetDeck:
                //tell ui to get deck
                ClientGame.clientUICtrl.ShowGetDecklistUI();
                break;
            case Packet.Command.DeckAccepted:
                ClientGame.clientUICtrl.ShowDeckAcceptedUI();
                break;
            case Packet.Command.YoureFirst:
                ClientGame.SetFirstTurnPlayer(0);
                break;
            case Packet.Command.YoureSecond:
                ClientGame.SetFirstTurnPlayer(1);
                break;
            case Packet.Command.SetFriendlyAvatar:
                ClientGame.SetAvatar(0, packet.CardName, packet.CardIDToBe);
                break;
            case Packet.Command.SetEnemyAvatar:
                ClientGame.SetAvatar(1, packet.CardName, packet.CardIDToBe);
                break;
            #endregion game start
            case Packet.Command.Delete:
                ClientGame.Delete(card);
                break;
            case Packet.Command.AddAsFriendly:
                var friendlyCard = ClientGame.CardRepo.InstantiateClientNonAvatar(packet.CardName, ClientGame, Friendly, packet.CardIDToBe);
                ClientGame.cardsByID.Add(packet.CardIDToBe, friendlyCard);
                ClientGame.friendlyDeckCtrl.PushTopdeck(friendlyCard);
                break;
            case Packet.Command.AddAsEnemy:
                var added = ClientGame.CardRepo.InstantiateClientNonAvatar(packet.CardName, ClientGame, Enemy, packet.CardIDToBe);
                ClientGame.cardsByID.Add(packet.CardIDToBe, added);
                //TODO make it always ask for cards from enemy deck
                switch (packet.Location)
                {
                    case CardLocation.Field:
                        added.Play(packet.X, packet.Y, added.Owner);
                        break;
                    case CardLocation.Discard:
                        added.Discard();
                        break;
                    case CardLocation.Annihilation:
                        added.Game.AnnihilationCtrl.Annihilate(added);
                        break;
                    default:
                        Debug.Log("Tried to add an enemy card to " + packet.Location);
                        break;
                }
                break;
            case Packet.Command.AddAsEnemyAndAttach:
                var addAndAttach = ClientGame.CardRepo.InstantiateClientNonAvatar(packet.CardName, ClientGame, Enemy, packet.CardIDToBe);
                ClientGame.cardsByID.Add(packet.CardIDToBe, addAndAttach);
                ClientGame.boardCtrl.GetCardAt(packet.X, packet.Y).AddAugment(addAndAttach);
                break;
            case Packet.Command.IncrementEnemyDeck:
                //TODO
                break;
            case Packet.Command.IncrementEnemyHand:
                ClientGame.enemyHandCtrl.IncrementHand();
                break;
            case Packet.Command.DecrementEnemyDeck:
                //TODO make sure for both this and decrement hand that you're not deleting a revealedcard
                if(ClientGame.enemyDeckCtrl.DeckSize > 0)
                {
                    ClientGame.enemyDeckCtrl.PopBottomdeck();
                }
                break;
            case Packet.Command.DecrementEnemyHand:
                ClientGame.enemyHandCtrl.DecrementHand();
                break;
            case Packet.Command.Augment: //the play method calls augment if the card is an augment
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.X + ", " + packet.Y);
                card?.Play(packet.X, packet.Y, card.Owner);
                break;
            case Packet.Command.Attach:
                ClientGame.boardCtrl.GetCardAt(packet.X, packet.Y)?.AddAugment(card);
                break;
            case Packet.Command.Move:
                card.Move(packet.X, packet.Y, packet.Answer);
                //make the ui show the updated n (and other values)
                ClientGame.uiCtrl.SelectCard(ClientGame.uiCtrl.SelectedCard, false);
                break;
            case Packet.Command.Topdeck:
                card.Topdeck();
                break;
            case Packet.Command.Discard:
                card.Discard();
                break;
            case Packet.Command.Rehand:
                card?.Rehand();
                break;
            case Packet.Command.Reshuffle:
                card?.Reshuffle();
                break;
            case Packet.Command.Bottomdeck:
                card?.Bottomdeck();
                break;
            case Packet.Command.Annihilate:
                ClientGame.AnnihilationCtrl.Annihilate(card);
                break;
            case Packet.Command.SetN:
                card?.SetN(packet.Stat);
                break;
            case Packet.Command.SetE:
                card?.SetE(packet.Stat);
                break;
            case Packet.Command.SetS:
                card?.SetS(packet.Stat);
                break;
            case Packet.Command.SetW:
                card?.SetW(packet.Stat);
                break;
            case Packet.Command.SetC:
                card?.SetC(packet.Stat);
                break;
            case Packet.Command.SetA:
                card?.SetA(packet.Stat);
                break;
            case Packet.Command.Negate:
                card?.SetNegated(packet.Answer);
                break;
            case Packet.Command.Activate:
                card?.SetActivated(packet.Answer);
                break;
            case Packet.Command.ChangeControl:
                if(card != null) card.Controller = ClientGame.Players[packet.ControllerIndex];
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
                foreach(int id in cardIDs)
                {
                    GameCard c = ClientGame.GetCardWithID(id);
                    if (c == null) Debug.LogError($"Tried to start a list search including card with invalid id {id}");
                    else choicesToPick.Add(c);
                }
                var listRestriction = packet.GetListRestriction(ClientGame);
                ClientGame.clientUICtrl.StartSearch(choicesToPick, listRestriction, packet.MaxNum);
                ClientGame.clientUICtrl.SetCurrState($"Choose Target for Effect of {listRestriction.Subeffect.Source.CardName}", 
                    ClientGame.CurrCardRestriction.blurb);
                break;
            case Packet.Command.ChooseEffectOption:
                //TODO catch out of bounds errors, in case of malicious packets?
                var subeff = card.Effects.ElementAt(packet.normalArgs[0]).Subeffects[packet.normalArgs[1]]
                    as DummyChooseOptionSubeffect;
                if(subeff == null)
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
                if(card != null) card.Effects.ElementAt(packet.EffIndex).X = packet.EffectX;
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
                if(target != null) card.Effects.ElementAt(packet.EffIndex).AddTarget(target);
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
                card.Effects.ElementAt(packet.EffIndex).Controller = ClientGame.Players[packet.normalArgs[1]];
                break;
            /*case Packet.Command.EffectImpossible:
                ClientGame.clientUICtrl.SetCurrState("Effect Impossible");
                break;*/
            case Packet.Command.OptionalTrigger:
                ClientTrigger t = card.Effects.ElementAt(packet.EffIndex).Trigger as ClientTrigger;
                t.ClientEffect.ClientController = Friendly;
                ClientGame.clientUICtrl.ShowOptionalTrigger(t, packet.EffIndex);
                break;
            default:
                Debug.LogError($"Unrecognized command {packet.command} sent to client");
                break;
        }
    }
}
