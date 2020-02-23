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

    public Restriction lastRestriction;

    public override void Awake()
    {
        base.Awake();
        timeTargetAccepted = DateTime.Now.Ticks;
    }

    public void Connect(string ip)
    {
        Debug.Log($"Connecting to {ip} on port {port}");
        tcpClient = new System.Net.Sockets.TcpClient(ip, port);
    }

    public override void Update()
    {
        if (changeTargetMode && DateTime.Now.Ticks - timeTargetAccepted >= 5000000)
        {
            ClientGame.mainClientGame.targetMode = Game.TargetMode.Free;
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
        if (packet.command != Packet.Command.Nothing)
        {
            Debug.Log("Parsing command " + packet.command + " for " + packet.cardID);
            ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Parsing command " + packet.command + " for card id" + packet.cardID;
        }

        lastPacket = packet;

        switch (packet.command)
        {
            case Packet.Command.Nothing:
                //discard
                return;
            case Packet.Command.Delete:
                ClientGame.mainClientGame.Delete(ClientGame.mainClientGame.GetCardFromID(packet.cardID));
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
                ClientGame.mainClientGame.enemyDeckCtrl.AddBlankCard();
                break;
            case Packet.Command.IncrementEnemyHand:
                Card emptyHandAdd = ClientGame.enemyDeckCtrl.AddBlankCard();
                ClientGame.Rehand(emptyHandAdd);
                break;
            case Packet.Command.DecrementEnemyDeck:
                //TODO make sure for both this and decrement hand that you're not deleting a revealedcard
                if(ClientGame.mainClientGame.enemyDeckCtrl.DeckSize() > 0)
                {
                    ClientGame.mainClientGame.enemyDeckCtrl.PopBottomdeck();
                }
                break;
            case Packet.Command.DecrementEnemyHand:
                if(ClientGame.mainClientGame.enemyHandCtrl.HandSize > 0)
                {
                    ClientGame.mainClientGame.enemyHandCtrl.RemoveFromHandAt(0);
                }
                break;
            case Packet.Command.Augment: //the play method calls augment if the card is an augment
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.X + ", " + packet.Y);
                Card toPlay = ClientGame.GetCardFromID(packet.cardID);
                ClientGame.Play(toPlay, packet.X, packet.X, toPlay.Owner);
                break;
            case Packet.Command.Move:
                ClientGame.MoveOnBoard(ClientGame.GetCardFromID(packet.cardID), packet.X, packet.Y);
                //make the ui show the updated n (and other values)
                ClientGame.mainClientGame.uiCtrl.SelectCard(ClientGame.mainClientGame.uiCtrl.SelectedCard, false);
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
                Card toSet = ClientGame.mainClientGame.GetCardFromID(packet.cardID);
                (toSet as CharacterCard)?.SetNESW(packet.N, packet.E, packet.S, packet.W);
                break;
            case Packet.Command.SetPips:
                ClientGame.mainClientGame.SetFriendlyPips(packet.Pips);
                break;
            case Packet.Command.SetEnemyPips:
                ClientGame.mainClientGame.SetEnemyPips(packet.Pips);
                break;
            case Packet.Command.PutBack:
                ClientGame.mainClientGame.boardCtrl.PutCardsBack();
                break;
            case Packet.Command.EndTurn:
                ClientGame.mainClientGame.turnPlayer = 1 - ClientGame.mainClientGame.turnPlayer;
                ClientGame.mainClientGame.boardCtrl.ResetCardsForTurn();
                if(ClientGame.mainClientGame.turnPlayer == 0) ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Your Turn";
                else ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Enemy Turn";
                break;
            case Packet.Command.YoureFirst:
                ClientGame.mainClientGame.turnPlayer = 0;
                ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Your Turn";
                break;
            case Packet.Command.YoureSecond:
                ClientGame.mainClientGame.turnPlayer = 1;
                ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Enemy Turn";
                break;
            case Packet.Command.RequestBoardTarget:
                ClientGame.mainClientGame.targetMode = Game.TargetMode.BoardTarget;
                lastRestriction = (Game.mainGame.GetCardFromID(packet.cardID)
                                    .Effects[packet.EffIndex].Subeffects[packet.SubeffIndex] as BoardTargetSubeffect)
                                    .boardRestriction;
                break;
            case Packet.Command.RequestHandTarget:
                ClientGame.mainClientGame.targetMode = Game.TargetMode.HandTarget;
                lastRestriction = (Game.mainGame.GetCardFromID(packet.cardID)
                                    .Effects[packet.EffIndex].Subeffects[packet.SubeffIndex] as HandTargetSubeffect)
                                    .cardRestriction;
                break;
            case Packet.Command.RequestDeckTarget:
                Debug.Log("Eff index: " + packet.EffIndex + " subeff index " + packet.SubeffIndex);
                CardRestriction deckRestriction = (Game.mainGame.GetCardFromID(packet.cardID)
                                    .Effects[packet.EffIndex].Subeffects[packet.SubeffIndex] as DeckTargetSubeffect)
                                    .cardRestriction;
                List<Card> toSearch = ClientGame.mainClientGame.friendlyDeckCtrl.CardsThatFitRestriction(deckRestriction);
                ClientGame.mainClientGame.clientUICtrl.StartSearch(toSearch, true);
                break;
            case Packet.Command.RequestDiscardTarget:
                CardRestriction discardRestriction = (Game.mainGame.GetCardFromID(packet.cardID)
                                    .Effects[packet.EffIndex].Subeffects[packet.SubeffIndex] as DiscardTargetSubeffect)
                                    .cardRestriction;
                List<Card> discardToSearch = ClientGame.mainClientGame.friendlyDiscardCtrl.CardsThatFitRestriction(discardRestriction);
                ClientGame.mainClientGame.clientUICtrl.StartSearch(discardToSearch, true);
                break;
            case Packet.Command.SpaceTarget:
                ClientGame.mainClientGame.targetMode = Game.TargetMode.SpaceTarget;
                lastRestriction = (Game.mainGame.GetCardFromID(packet.cardID)
                                    .Effects[packet.EffIndex].Subeffects[packet.SubeffIndex] as SpaceTargetSubeffect)
                                    .spaceRestriction;
                //TODO display based on that space
                break;
            case Packet.Command.SetEffectsX:
                Debug.Log("Setting X to " + packet.X);
                Game.mainGame.GetCardFromID(packet.cardID).Effects[packet.EffIndex].X = packet.X;
                break;
            case Packet.Command.PlayerSetX:
                ClientGame.mainClientGame.clientUICtrl.GetXForEffect();
                break;
            case Packet.Command.TargetAccepted:
                timeTargetAccepted = DateTime.Now.Ticks;
                changeTargetMode = true;
                break;
            case Packet.Command.EnableDecliningTarget:
                ClientGame.mainClientGame.clientUICtrl.EnableDecliningTarget();
                break;
            case Packet.Command.DisableDecliningTarget:
                ClientGame.mainClientGame.clientUICtrl.DisableDecliningTarget();
                break;
            case Packet.Command.DiscardSimples:
                ClientGame.mainClientGame.boardCtrl.DiscardSimples();
                break;
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }
}
