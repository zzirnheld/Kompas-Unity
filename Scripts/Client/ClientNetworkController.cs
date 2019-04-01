using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;


public class ClientNetworkController : NetworkController {

    public UdpCNetworkDriver mDriver;
    public NetworkConnection mConnection;
    public bool Done;
    private bool Hosting = false;

    public void Start()
    {
        
    }

    public void Connect()
    {
        mDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        mConnection = default(NetworkConnection);

        //this code is the connection code. dont do until client hits connect button?
        var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
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
            if (!Done) Debug.Log("Something went wrong in connection");
            else return;
        }

        DataStreamReader reader;
        NetworkEvent.Type cmd;
        while((cmd = mConnection.PopEvent(mDriver, out reader)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                //connected to server
                Debug.Log("connected to server");
                //how to send stuff:
                /*using (var writer = new DataStreamWriter(4, Unity.Collections.Allocator.Temp))
                {
                    writer.Write(byte array, length);
                    mConnection.Send(mDriver, writer);
                }*/
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
                mConnection = default(NetworkConnection); //default gets the default value of whatever type
            }
        }
    }

    public void ParseCommand(byte[] buffer)
    {
        Packet packet = Deserialize(buffer);
        if (packet == null) return;
        Debug.Log("Parsing command " + packet.command + " for " + packet.cardID);

        switch (packet.command)
        {
            case Packet.Command.AddToDeck:
                ClientGame.mainClientGame.friendlyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe);
                break;
            case Packet.Command.AddToEnemyDeck:
                ClientGame.mainClientGame.enemyDeckCtrl.AddCard(packet.CardName, packet.CardIDToBe, 1); //TODO make it always ask for cards from enemy deck
                break;
            case Packet.Command.Augment: //the play method calls augment if the card is an augment
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.X + ", " + packet.Y);
                ClientGame.mainClientGame.Play(packet.cardID, packet.X, packet.Y);
                break;
            case Packet.Command.Move:
                ClientGame.mainClientGame.Move(packet.cardID, packet.X, packet.Y);
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
                break;
            case Packet.Command.YoureSecond:
                ClientGame.mainClientGame.turnPlayer = 1;
                break;
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }

    #region Request Actions
    public void RequestPlay(Card card, int toX, int toY)
    {
        //card.PutBack();
        Packet packet;
        if (card is AugmentCard) packet = new Packet(Packet.Command.Augment, card, toX, toY);
        else packet = new Packet(Packet.Command.Play, card, toX, toY);
        Send(packet, mDriver, mConnection);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
        Send(packet, mDriver, mConnection);
    }

    public void RequestTopdeck(Card card)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Topdeck, card);
        Send(packet, mDriver, mConnection);
    }

    public void RequestDiscard(Card card)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Discard, card);
        Send(packet, mDriver, mConnection);
    }

    public void RequestRehand(Card card)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Rehand, card);
        Send(packet, mDriver, mConnection);
    }

    public void RequestAddToDeck(string cardName)
    {
        Debug.Log("Requesting add \"" + cardName + "\" to deck, length " + cardName.Length);
        Packet packet = new Packet(Packet.Command.AddToDeck, cardName);
        Send(packet, mDriver, mConnection);
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
        Send(packet, mDriver, mConnection);
    }

    public void RequestSetNESW(Card card, int n, int e, int s, int w)
    {
        Packet packet = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        Send(packet, mDriver, mConnection);
    }

    public void RequestUpdatePips(int num)
    {
        Packet packet = new Packet(Packet.Command.SetPips, num);
        Send(packet, mDriver, mConnection);
        Debug.Log("requesting updating pips to " + num);
    }

    public void RequestEndTurn()
    {
        Packet packet = new Packet(Packet.Command.EndTurn);
        Send(packet, mDriver, mConnection);
    }

    #endregion
}
