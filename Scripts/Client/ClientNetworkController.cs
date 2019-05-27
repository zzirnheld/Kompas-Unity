using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
//using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;


public class ClientNetworkController : NetworkController {

    public UdpNetworkDriver mDriver;
    public NetworkPipeline mPipeline;
    public NetworkConnection mConnection;
    private bool Hosting = false;
    private bool Connected = false;

    public Packet lastPacket;

    public Restriction lastRestriction;

    public void Start()
    {
        
    }

    public void Connect()
    {
        mDriver = new UdpNetworkDriver(new ReliableUtility.Parameters { WindowSize = 32 });
        mPipeline = mDriver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        mConnection = default;

        //this code is the connection code. dont do until client hits connect button?
        var endpoint = new NetworkEndPoint();
        endpoint = NetworkEndPoint.Parse("127.0.0.1", 8888); //TODO get ip
        mConnection = mDriver.Connect(endpoint);
        Hosting = true;
    }

    private void OnDestroy()
    {
        mDriver.Dispose();
    }

    void Update()
    {
        if (!Hosting) return;
        mDriver.ScheduleUpdate().Complete();
        if (!mConnection.IsCreated)
        {
            Debug.Log("Something went wrong in connection");
            return;
        }

        //keeps the connection alive
        //

        DataStreamReader reader;
        NetworkEvent.Type cmd;
        while((cmd = mConnection.PopEvent(mDriver, out reader)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                //connected to server
                Debug.Log("connected to server");
                ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Connected to Server";
                ClientGame.mainClientGame.uiCtrl.HideNetworkingUI();
                Connected = true;
            }
            if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtxt = default(DataStreamReader.Context);
                byte[] packetBuffer = reader.ReadBytesAsArray(ref readerCtxt, BUFFER_SIZE);
                ParseCommand(packetBuffer);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Disconnected from server";
                mConnection = default(NetworkConnection); //default gets the default value of whatever type
                Hosting = false;
                Connected = false;
            }
        }
        
        if(Connected) Send(new Packet(Packet.Command.Nothing), mDriver, mConnection, mPipeline);
    }

    public void ParseCommand(byte[] buffer) //KEEP CONNECTION ALIVE
    {
        Packet packet = Deserialize(buffer);
        if (packet == null)
        {
            Debug.Log("Null packet");
            return;
        }
        if (packet.command != Packet.Command.Nothing)
        {
            Debug.Log("Parsing command " + packet.command + " for " + packet.cardID);
            ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Parsing command " + packet.command + " for " + packet.cardID;
        }

        lastPacket = packet;

        switch (packet.command)
        {
            case Packet.Command.Nothing:
            //case Packet.Command.Confirm:
                //return;
                return;
            case Packet.Command.AddAsFriendly:
                ClientGame.mainClientGame.friendlyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe);
                break;
            case Packet.Command.AddAsEnemy:
                Card added = ClientGame.mainClientGame.enemyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe, 1); //TODO make it always ask for cards from enemy deck
                switch (packet.Location)
                {
                    case Card.CardLocation.Field:
                        ClientGame.mainClientGame.Play(packet.CardIDToBe, packet.X, packet.Y);
                        break;
                    case Card.CardLocation.Discard:
                        ClientGame.mainClientGame.Discard(packet.CardIDToBe);
                        break;
                }
                break;
            case Packet.Command.Augment: //the play method calls augment if the card is an augment
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.X + ", " + packet.Y);
                ClientGame.mainClientGame.Play(packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.Move:
                ClientGame.mainClientGame.Move(packet.cardID, packet.X, packet.Y);
                //make the ui show the updated n (and other values)
                ClientGame.mainClientGame.uiCtrl.SelectCard(ClientGame.mainClientGame.uiCtrl.SelectedCard, false);
                break;
            case Packet.Command.Topdeck:
                ClientGame.mainClientGame.Topdeck(packet.cardID);
                break;
            case Packet.Command.Discard:
                ClientGame.mainClientGame.Discard(packet.cardID);
                break;
            case Packet.Command.Rehand:
                ClientGame.mainClientGame.Rehand(packet.cardID);
                break;
            case Packet.Command.SetNESW:
                ClientGame.mainClientGame.SetNESW(packet.cardID, packet.N, packet.E, packet.S, packet.W);
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
                ClientGame.mainClientGame.boardCtrl.ResetCardsM();
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
            case Packet.Command.X:
                ClientGame.mainClientGame.clientUICtrl.GetXForEffect();
                break;
            case Packet.Command.TargetAccepted:
                ClientGame.mainClientGame.targetMode = Game.TargetMode.NoTargeting;
                break;
            case Packet.Command.EnableDecliningTarget:
                ClientGame.mainClientGame.clientUICtrl.EnableDecliningTarget();
                break;
            case Packet.Command.DisableDecliningTarget:
                ClientGame.mainClientGame.clientUICtrl.DisableDecliningTarget();
                break;
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }

    #region Request Actions
    public void RequestPlay(Card card, int toX, int toY)
    {
        Packet packet;
        if (card is AugmentCard) packet = new Packet(Packet.Command.Augment, card, toX, toY);
        else packet = new Packet(Packet.Command.Play, card, toX, toY);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestAttack(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Attack, card, toX, toY);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestTopdeck(Card card)
    {
        Packet packet = new Packet(Packet.Command.Topdeck, card);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestDiscard(Card card)
    {
        Packet packet = new Packet(Packet.Command.Discard, card);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestRehand(Card card)
    {
        Packet packet = new Packet(Packet.Command.Rehand, card);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestAddToDeck(string cardName)
    {
        Debug.Log("Requesting add \"" + cardName + "\" to deck, length " + cardName.Length);
        Packet packet = new Packet(Packet.Command.AddToDeck, cardName);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestDecklistImport(string decklist)
    {
        Debug.Log("Requesting Deck import of \"" + decklist + "\"");
        string[] cardNames = decklist.Split('\n');
        foreach(string cardName in cardNames){
            RequestAddToDeck(cardName);
        }
    }

    public void RequestDraw()
    {
        Packet packet = new Packet(Packet.Command.Draw);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestSetNESW(Card card, int n, int e, int s, int w)
    {
        Packet packet = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestUpdatePips(int num)
    {
        Packet packet = new Packet(Packet.Command.SetPips, num);
        Send(packet, mDriver, mConnection, mPipeline);
        Debug.Log("requesting updating pips to " + num);
    }

    public void RequestEndTurn()
    {
        Packet packet = new Packet(Packet.Command.EndTurn);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestTarget(Card card)
    {
        Debug.Log("Requesting target " + card.CardName);
        Packet packet = new Packet(Packet.Command.Target, card);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestResolveEffect(Card card, int index)
    {
        Debug.Log("Requesting effect of " + card.CardName + " number" + index);
        Packet packet = new Packet(Packet.Command.TestTargetEffect, card, index);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void RequestSetX(int x)
    {
        Debug.Log("Requesting to set X to " + x);
        Packet packet = new Packet(Packet.Command.X, x);
        Send(packet, mDriver, mConnection, mPipeline);
    }

    public void DeclineAnotherTarget()
    {
        Debug.Log("Declining to select another target");
        Packet packet = new Packet(Packet.Command.DeclineAnotherTarget);
        Send(packet, mDriver, mConnection, mPipeline);
    }
    #endregion
}
