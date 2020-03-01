using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using KompasNetworking;


public class ClientNetworkController : NetworkController {

    private bool changeTargetMode = false;
    private long timeTargetAccepted;

    public ClientGame ClientGame;

    public Packet lastPacket;

    public Restriction Restriction;
    public int X;

    public override void Awake()
    {
        base.Awake();
        timeTargetAccepted = DateTime.Now.Ticks;
    }

    public void Connect(string ip)
    {
        Debug.Log($"Connecting to {ip} on port {port}");
        ClientGame.uiCtrl.CurrentStateString = $"Connecting to {ip}";
        tcpClient = new System.Net.Sockets.TcpClient(ip, port);
        ClientGame.uiCtrl.CurrentStateString = $"Connected to {ip}";
    }

    public override void Update()
    {
        if (changeTargetMode && DateTime.Now.Ticks - timeTargetAccepted >= 5000000)
        {
            ClientGame.targetMode = Game.TargetMode.Free;
            changeTargetMode = false;
        }

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
        ClientGame.uiCtrl.CurrentStateString = $"Parsing command {packet.command} for card id {packet.cardID}";
        
        lastPacket = packet;

        switch (packet.command)
        {
            #region game start
            case Packet.Command.GetDeck:
                //tell ui to get deck
                break;
            #endregion game start
            case Packet.Command.Delete:
                ClientGame.Delete(ClientGame.GetCardFromID(packet.cardID));
                break;
            case Packet.Command.AddAsFriendly:
                ClientGame.friendlyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe, ClientGame.Players[0]);
                break;
            case Packet.Command.AddAsEnemy:
                Card added = ClientGame.enemyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe, ClientGame.Players[1]);
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
                ClientGame.enemyDeckCtrl.AddBlankCard();
                break;
            case Packet.Command.IncrementEnemyHand:
                Card emptyHandAdd = ClientGame.enemyDeckCtrl.AddBlankCard();
                ClientGame.Rehand(emptyHandAdd);
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
                ClientGame.MoveOnBoard(ClientGame.GetCardFromID(packet.cardID), packet.X, packet.Y);
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
            case Packet.Command.SetNESW:
                Card toSet = ClientGame.GetCardFromID(packet.cardID);
                (toSet as CharacterCard)?.SetNESW(packet.N, packet.E, packet.S, packet.W);
                break;
            case Packet.Command.SetPips:
                ClientGame.SetFriendlyPips(packet.Pips);
                break;
            case Packet.Command.SetEnemyPips:
                ClientGame.SetEnemyPips(packet.Pips);
                break;
            case Packet.Command.PutBack:
                ClientGame.boardCtrl.PutCardsBack();
                break;
            case Packet.Command.EndTurn:
                ClientGame.turnPlayer = 1 - ClientGame.turnPlayer;
                ClientGame.boardCtrl.ResetCardsForTurn();
                if(ClientGame.turnPlayer == 0) ClientGame.uiCtrl.CurrentStateString = "Your Turn";
                else ClientGame.uiCtrl.CurrentStateString = "Enemy Turn";
                break;
            case Packet.Command.YoureFirst:
                ClientGame.turnPlayer = 0;
                ClientGame.uiCtrl.CurrentStateString = "Your Turn";
                break;
            case Packet.Command.YoureSecond:
                ClientGame.turnPlayer = 1;
                ClientGame.uiCtrl.CurrentStateString = "Enemy Turn";
                break;
            case Packet.Command.RequestBoardTarget:
                ClientGame.targetMode = Game.TargetMode.BoardTarget;
                Restriction = packet.BoardRestriction;
                new Effect(ClientGame.GetCardFromID(packet.cardID), Restriction, packet.X);
                break;
            case Packet.Command.RequestHandTarget:
                ClientGame.targetMode = Game.TargetMode.HandTarget;
                Restriction = packet.CardRestriction;
                new Effect(ClientGame.GetCardFromID(packet.cardID), Restriction, packet.X);
                break;
            case Packet.Command.RequestDeckTarget:
                Debug.Log("Eff index: " + packet.EffIndex + " subeff index " + packet.SubeffIndex);
                CardRestriction deckRestriction = packet.CardRestriction;
                new Effect(ClientGame.GetCardFromID(packet.cardID), Restriction, packet.X);
                List<Card> toSearch = ClientGame.friendlyDeckCtrl.CardsThatFitRestriction(deckRestriction);
                ClientGame.clientUICtrl.StartSearch(toSearch, true);
                break;
            case Packet.Command.RequestDiscardTarget:
                CardRestriction discardRestriction = packet.CardRestriction;
                new Effect(ClientGame.GetCardFromID(packet.cardID), Restriction, packet.X);
                List<Card> discardToSearch = ClientGame.friendlyDiscardCtrl.CardsThatFitRestriction(discardRestriction);
                ClientGame.clientUICtrl.StartSearch(discardToSearch, true);
                break;
            case Packet.Command.SpaceTarget:
                ClientGame.targetMode = Game.TargetMode.SpaceTarget;
                Restriction = packet.SpaceRestriction;
                new Effect(ClientGame.GetCardFromID(packet.cardID), Restriction, packet.X);
                //TODO display based on that space
                break;
            case Packet.Command.SetEffectsX:
                Debug.Log("Setting X to " + packet.X);
                Game.mainGame.GetCardFromID(packet.cardID).Effects[packet.EffIndex].X = packet.X;
                break;
            case Packet.Command.PlayerSetX:
                ClientGame.clientUICtrl.GetXForEffect();
                break;
            case Packet.Command.TargetAccepted:
                timeTargetAccepted = DateTime.Now.Ticks;
                changeTargetMode = true;
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
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }
}
