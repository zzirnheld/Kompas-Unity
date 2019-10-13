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

    private long timeTargetAccepted;
    private bool changeTargetMode = false;

    public Packet lastPacket;

    public Restriction lastRestriction;

    public void Start()
    {
        timeTargetAccepted = DateTime.Now.Ticks;
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
        
        if (changeTargetMode && DateTime.Now.Ticks - timeTargetAccepted >= 5000000)
        {
            ClientGame.mainClientGame.targetMode = Game.TargetMode.Free;
            changeTargetMode = false;
        }

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

        if (!Connected) return;
        timeChange += Time.deltaTime;
        if(timeChange >= 1f)
        {
            Send(new Packet(Packet.Command.Nothing), mDriver, mConnection, mPipeline);
            timeChange = 0f;
        }
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
                ClientGame.mainClientGame.friendlyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe);
                break;
            case Packet.Command.AddAsEnemy:
                Card added = ClientGame.mainClientGame.enemyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe, 1); //TODO make it always ask for cards from enemy deck
                switch (packet.Location)
                {
                    case CardLocation.Field:
                        added.Play(packet.X, packet.Y, 1);
                        break;
                    case CardLocation.Discard:
                        added.Discard();
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
                Card emptyHandAdd = ClientGame.mainClientGame.enemyDeckCtrl.AddBlankCard();
                emptyHandAdd.Rehand();
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
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).Play(packet.X, packet.Y);
                break;
            case Packet.Command.Move:
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).MoveOnBoard(packet.X, packet.Y);
                //make the ui show the updated n (and other values)
                ClientGame.mainClientGame.uiCtrl.SelectCard(ClientGame.mainClientGame.uiCtrl.SelectedCard, false);
                break;
            case Packet.Command.Topdeck:
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).Topdeck();
                break;
            case Packet.Command.Discard:
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).Discard();
                break;
            case Packet.Command.Rehand:
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).Rehand();
                break;
            case Packet.Command.Reshuffle:
                ClientGame.mainClientGame.GetCardFromID(packet.cardID).Reshuffle();
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

    public void Send(Packet packet)
    {
        Send(packet, mDriver, mConnection, mPipeline);
    }
}
