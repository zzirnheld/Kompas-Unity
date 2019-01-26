using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkController : NetworkController {
    
    private int connectionID;
    public const int SOCKET = 8888;
    public string ip;

    // Update is called once per frame
    // reason that this code is in client/server is because client only needs the connection ID of server,
    // server needs to listen to both clients
    void Update()
    {
        if (!hosting) return;
        //the "out" arguments in the following method mean that the things are being passed by reference,
        //and will be changed by the function
        recData = NetworkTransport.Receive(out hostID, out connectionID, out channelID,
            recBuffer, BUFFER_SIZE, out dataSize, out error);
        switch (recData)
        {
            //nothing
            case NetworkEventType.Nothing:
                break;
            //someone has tried to connect to me
            case NetworkEventType.ConnectEvent:
                //yay someone connected to me with the connectionID that was just set
                Debug.Log("Connected!");
                connected = true;
                break;
            //they've actually sent something
            case NetworkEventType.DataEvent:
                ParseCommand(recBuffer);
                break;
            //the person has disconnected
            case NetworkEventType.DisconnectEvent:
                break;
            //i don't know what this does and i don't 
            case NetworkEventType.BroadcastEvent:
                break;
        }
    }

    public void Connect()
    {
        Host(8888);
        connectionID = NetworkTransport.Connect(hostID, ip, SOCKET, 0, out error);
        //TODO cast error to NetworkError and see if was NetworkError.OK
    }

    public void ParseCommand(byte[] buffer)
    {
        Packet packet = Deserialize(buffer);
        if (packet == null) return;
        switch (packet.command)
        {
            case Packet.Command.AddToDeck:
                ClientGame.mainClientGame.friendlyDeckCtrl.AddCard(packet.args);
                break;
            case Packet.Command.AddToEnemyDeck:
                ClientGame.mainClientGame.enemyDeckCtrl.AddCard(packet.args); //TODO make it always ask for cards from enemy deck
                break;
            case Packet.Command.Play:
                ClientGame.mainClientGame.Play(packet.cardID, packet.x, packet.y);
                break;
            case Packet.Command.Move:
                ClientGame.mainClientGame.Move(packet.cardID, packet.x, packet.y);
                break;
            case Packet.Command.Topdeck:
                ClientGame.mainClientGame.Topdeck(packet.cardID);
                break;
            case Packet.Command.Discard:
                break;
            case Packet.Command.Rehand:
                break;
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }

    #region Request Actions
    public void RequestPlay(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Play, card, toX, toY);
        Send(packet, connectionID);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
        Send(packet, connectionID);
    }

    public void RequestTopdeck(Card card)
    {
        Packet packet = new Packet(Packet.Command.Topdeck, card);
        Send(packet, connectionID);
    }

    public void RequestDiscard(Card card)
    {
        Packet packet = new Packet(Packet.Command.Discard, card);
        Send(packet, connectionID);
    }

    public void RequestRehand(Card card)
    {
        Packet packet = new Packet(Packet.Command.Rehand, card);
        Send(packet, connectionID);
    }

    public void RequestAddToDeck(string cardName)
    {
        Packet packet = new Packet(Packet.Command.AddToDeck, cardName);
        Send(packet, connectionID);
    }

    public void RequestDecklistImport(string decklist)
    {
        string[] cardNames = decklist.Split('\n');
        foreach(string cardName in cardNames){
            RequestAddToDeck(cardName);
        }
    }

    #endregion
}
