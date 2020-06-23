using System;
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
        Debug.Log($"Parsing command {packet.command} for {packet.cardID}");

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
                ClientGame.Delete(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.AddAsFriendly:
                Card friendlyCard = ClientGame.CardRepo.InstantiateClientNonAvatar(packet.CardName, ClientGame, Friendly, packet.CardIDToBe);
                ClientGame.cardsByID.Add(packet.CardIDToBe, friendlyCard);
                ClientGame.friendlyDeckCtrl.AddCard(friendlyCard);
                break;
            case Packet.Command.AddAsEnemy:
                Card added = ClientGame.CardRepo.InstantiateClientNonAvatar(packet.CardName, ClientGame, Enemy, packet.CardIDToBe);
                ClientGame.cardsByID.Add(packet.CardIDToBe, added);
                ClientGame.enemyDeckCtrl.AddCard(added);
                //TODO make it always ask for cards from enemy deck
                switch (packet.Location)
                {
                    case CardLocation.Field:
                        ClientGame.Play(added, packet.X, packet.Y, added.Owner);
                        break;
                    case CardLocation.Discard:
                        ClientGame.Discard(added);
                        break;
                    default:
                        Debug.Log("Tried to add an enemy card to " + packet.Location);
                        break;
                }
                break;
            case Packet.Command.IncrementEnemyDeck:
                //TODO
                break;
            case Packet.Command.IncrementEnemyHand:
                //TODO
                break;
            case Packet.Command.DecrementEnemyDeck:
                //TODO make sure for both this and decrement hand that you're not deleting a revealedcard
                if(ClientGame.enemyDeckCtrl.DeckSize() > 0)
                {
                    ClientGame.enemyDeckCtrl.PopBottomdeck();
                }
                break;
            case Packet.Command.DecrementEnemyHand:
                if(ClientGame.enemyHandCtrl.HandSize > 0)
                {
                    ClientGame.enemyHandCtrl.RemoveFromHandAt(0);
                }
                break;
            case Packet.Command.Augment: //the play method calls augment if the card is an augment
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.X + ", " + packet.Y);
                Card toPlay = ClientGame.GetCardFromID(packet.cardID);
                ClientGame.Play(toPlay, packet.X, packet.Y, toPlay.Owner);
                break;
            case Packet.Command.Move:
                ClientGame.MoveOnBoard(ClientGame.GetCardFromID(packet.cardID), packet.X, packet.Y, packet.Answer);
                //make the ui show the updated n (and other values)
                ClientGame.uiCtrl.SelectCard(ClientGame.uiCtrl.SelectedCard, false);
                break;
            case Packet.Command.Topdeck:
                ClientGame.Topdeck(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.Discard:
                ClientGame.Discard(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.Rehand:
                ClientGame.Rehand(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.Reshuffle:
                ClientGame.Reshuffle(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.Bottomdeck:
                ClientGame.Bottomdeck(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.SetNESW:
                var charToSet = ClientGame.GetCardFromID(packet.cardID) as CharacterCard;
                charToSet?.SetNESW(packet.N, packet.E, packet.S, packet.W);
                ClientGame.clientUICtrl.ShowInfoFor(ClientGame.clientUICtrl.SelectedCard, true);
                break;
            case Packet.Command.SetSpellStats:
                var spellToSet = ClientGame.GetCardFromID(packet.cardID) as SpellCard;
                if (spellToSet != null) spellToSet.C = packet.C;
                break;
            case Packet.Command.Negate:
                Card toNegate = ClientGame.GetCardFromID(packet.cardID);
                ClientGame.SetNegated(toNegate, packet.Answer);
                break;
            case Packet.Command.Activate:
                var toActivate = ClientGame.GetCardFromID(packet.cardID);
                ClientGame.SetActivated(toActivate, packet.Answer);
                break;
            case Packet.Command.ChangeControl:
                var toChangeCtrl = ClientGame.GetCardFromID(packet.cardID);
                var player = ClientGame.Players[packet.ControllerIndex];
                ClientGame.ChangeControl(toChangeCtrl, player);
                break;
            case Packet.Command.SetPips:
                ClientGame.SetFriendlyPips(packet.Pips);
                break;
            case Packet.Command.SetEnemyPips:
                ClientGame.SetEnemyPips(packet.Pips);
                break;
            case Packet.Command.Leyload:
                ClientGame.Leyload = packet.Leyload;
                break;
            case Packet.Command.PutBack:
                ClientGame.boardCtrl.PutCardsBack();
                break;
            case Packet.Command.EndTurn:
                ClientGame.EndTurn();
                break;
            case Packet.Command.RequestBoardTarget:
                ClientGame.targetMode = Game.TargetMode.BoardTarget;
                ClientGame.CurrCardRestriction = packet.GetBoardRestriction(ClientGame);
                ClientGame.clientUICtrl.SetCurrState("Choose Board Target", ClientGame.CurrCardRestriction.Blurb);
                break;
            case Packet.Command.RequestHandTarget:
                ClientGame.targetMode = Game.TargetMode.HandTarget;
                ClientGame.CurrCardRestriction = packet.GetCardRestriction(ClientGame);
                ClientGame.clientUICtrl.SetCurrState("Choose Hand Target", ClientGame.CurrCardRestriction.Blurb);
                break;
            case Packet.Command.RequestDeckTarget:
                Debug.Log($"Deck target for Eff index: {packet.EffIndex} subeff index {packet.SubeffIndex}");
                CardRestriction deckRestriction = packet.GetCardRestriction(ClientGame);
                List<Card> toSearch = ClientGame.friendlyDeckCtrl.CardsThatFitRestriction(deckRestriction);
                ClientGame.clientUICtrl.StartSearch(toSearch);
                ClientGame.clientUICtrl.SetCurrState("Choose Deck Target", ClientGame.CurrCardRestriction.Blurb);
                break;
            case Packet.Command.RequestDiscardTarget:
                CardRestriction discardRestriction = packet.GetCardRestriction(ClientGame);
                List<Card> discardToSearch = ClientGame.friendlyDiscardCtrl.CardsThatFitRestriction(discardRestriction);
                ClientGame.clientUICtrl.StartSearch(discardToSearch);
                ClientGame.clientUICtrl.SetCurrState("Choose Discard Target", ClientGame.CurrCardRestriction.Blurb);
                break;
            case Packet.Command.GetChoicesFromList:
                int[] cardIDs = packet.specialArgs;
                List<Card> choicesToPick = new List<Card>();
                foreach(int id in cardIDs)
                {
                    Card c = ClientGame.GetCardFromID(id);
                    if (c == null) Debug.LogError($"Tried to start a list search including card with invalid id {id}");
                    else choicesToPick.Add(c);
                }
                var listRestriction = packet.GetListRestriction(ClientGame);
                ClientGame.clientUICtrl.StartSearch(choicesToPick, listRestriction, packet.normalArgs[0]);
                ClientGame.clientUICtrl.SetCurrState($"Choose Target for Effect of {listRestriction.Subeffect.Source.CardName}", 
                    ClientGame.CurrCardRestriction.Blurb);
                break;
            case Packet.Command.ChooseEffectOption:
                //TODO catch out of bounds errors, in case of malicious packets?
                var subeff = ClientGame.GetCardFromID(packet.cardID).Effects[packet.normalArgs[0]].Subeffects[packet.normalArgs[1]]
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
                ClientGame.clientUICtrl.SetCurrState("Choose Space Target", ClientGame.CurrSpaceRestriction.Blurb);
                break;
            case Packet.Command.SetEffectsX:
                Debug.Log("Setting X to " + packet.X);
                ClientGame.GetCardFromID(packet.cardID).Effects[packet.EffIndex].X = packet.EffectX;
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
                var eff = ClientGame.GetCardFromID(packet.cardID).Effects[packet.EffIndex];
                eff.Controller = ClientGame.Players[packet.normalArgs[1]];
                break;
            case Packet.Command.EffectImpossible:
                ClientGame.clientUICtrl.SetCurrState("Effect Impossible");
                break;
            case Packet.Command.OptionalTrigger:
                ClientTrigger t = ClientGame.GetCardFromID(packet.cardID).Effects[packet.EffIndex].Trigger as ClientTrigger;
                t.ClientEffect.ClientController = Friendly;
                ClientGame.clientUICtrl.ShowOptionalTrigger(t, packet.EffectX);
                break;
            default:
                Debug.LogError($"Unrecognized command {packet.command} sent to client");
                break;
        }
    }
}
