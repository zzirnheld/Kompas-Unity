using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkController : NetworkController {
    
    private int connectionID;
    public int SOCKET = 0; //TODo make something else so can test 2 clients
    public const int SERVER_PORT = 8888;
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
                ClientGame.mainClientGame.uiCtrl.CurrentStateString = "Connected to Server!";
                break;
            //they've actually sent something
            case NetworkEventType.DataEvent:
                Debug.Log("Recieved data event");
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
        //TODO get ip from ui
        ip = ClientGame.mainClientGame.uiCtrl.ipInputField.text;
        Host(0);
        if (ip == "localhost" || ip == "")
        {
            Debug.Log("renaming " + ip);
            ip = "127.0.0.1";
        }
        connectionID = NetworkTransport.Connect(hostID, ip, SERVER_PORT, 0, out error);
        //TODO cast error to NetworkError and see if was NetworkError.OK
    }

    public void ParseCommand(byte[] buffer)
    {
        Packet packet = Deserialize(buffer);
        if (packet == null) return;
        Debug.Log("Parsing command " + packet.command + " for " + packet.cardID);

        switch (packet.command)
        {
            case Packet.Command.AddToDeck:
                ClientGame.mainClientGame.friendlyDeckCtrl.AddCard(packet.args, packet.num);
                break;
            case Packet.Command.AddToEnemyDeck:
                ClientGame.mainClientGame.enemyDeckCtrl.AddCard(packet.args, packet.num, 1); //TODO make it always ask for cards from enemy deck
                break;
            case Packet.Command.Play:
                Debug.Log("Client ordered to play to " + packet.x + ", " + packet.y);
                ClientGame.mainClientGame.Play(packet.cardID, packet.x, packet.y);
                break;
            case Packet.Command.Move:
                ClientGame.mainClientGame.Move(packet.cardID, packet.x, packet.y);
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
                ClientGame.mainClientGame.SetNESW(packet.cardID, packet.n, packet.e, packet.s, packet.w);
                break;
            case Packet.Command.SetPips:
                ClientGame.mainClientGame.SetFriendlyPips(packet.num);
                break;
            case Packet.Command.SetEnemyPips:
                ClientGame.mainClientGame.SetEnemyPips(packet.num);
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
        Packet packet = new Packet(Packet.Command.Play, card, toX, toY);
        Send(packet, connectionID);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
        Send(packet, connectionID);
    }

    public void RequestTopdeck(Card card)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Topdeck, card);
        Send(packet, connectionID);
    }

    public void RequestDiscard(Card card)
    {
        //card.PutBack();
        Packet packet = new Packet(Packet.Command.Discard, card);
        Send(packet, connectionID);
    }

    public void RequestRehand(Card card)
    {
        //card.PutBack();
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
        Debug.Log("Requesting Deck import of " + decklist);
        string[] cardNames = decklist.Split('\n');
        foreach(string cardName in cardNames){
            RequestAddToDeck(cardName);
        }
    }

    public void RequestDraw()
    {
        Packet packet = new Packet(Packet.Command.Draw);
        Send(packet, connectionID);
    }

    public void RequestSetNESW(Card card, int n, int e, int s, int w)
    {
        Packet packet = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        Send(packet, connectionID);
    }

    public void RequestUpdatePips(int num)
    {
        Packet packet = new Packet(Packet.Command.SetPips, num);
        Send(packet, connectionID);
        Debug.Log("requesting updating pips to " + num);
    }

    #endregion
}
