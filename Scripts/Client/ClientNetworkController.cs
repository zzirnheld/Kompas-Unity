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

    void Start()
    {
        mDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        mConnection = default(NetworkConnection);

        //this code is the connection code. dont do until client hits connect button?
        var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
        mConnection = mDriver.Connect(endpoint);
    }

    private void OnDestroy()
    {
        mDriver.Dispose();
    }

    void Update()
    {
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
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                mConnection = default(NetworkConnection); //default gets the default value of whatever type
            }
        }
    }

    /*private int connectionID;
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

    #endregion*/
}
