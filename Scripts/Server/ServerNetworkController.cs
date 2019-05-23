//using System.Collections;
//using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Unity.Networking.Transport.LowLevel.Unsafe;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
//using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ServerNetworkController : NetworkController {

    public UdpNetworkDriver mDriver;
    public NetworkPipeline mPipeline;
    public NativeList<NetworkConnection> mConnections;
    private bool Hosting = false;

    public void Start()
    {
        
    }

    public void Host(ushort port)
    {
        mDriver = new UdpNetworkDriver(new ReliableUtility.Parameters { WindowSize = 32 });
        mPipeline = mDriver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        var endpoint = new NetworkEndPoint();
        endpoint = NetworkEndPoint.Parse("0.0.0.0", port);
        if (mDriver.Bind(endpoint) != 0) Debug.Log("Failed to bind to port 8888");
        else { mDriver.Listen(); Debug.Log("listening"); }

        mConnections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Hosting = true;
        ServerGame.mainServerGame.uiCtrl.CurrentStateString = "Hosting";
    }

    public void HostIntPort(int port)
    {
        Host((ushort) port);
    }

    void OnDestroy()
    {
        mDriver.Dispose();
        mConnections.Dispose();
    }

    void Update()
    {
        if (!Hosting) return;
        mDriver.ScheduleUpdate().Complete();

        //remove dead connections
        for(int i = 0; i < mConnections.Length; i++)
        {
            if (!mConnections[i].IsCreated)
            {
                mConnections.RemoveAtSwapBack(i);
                i--;
            }
        }

        //add any and all new connections to accept
        NetworkConnection c;
        while((c = mDriver.Accept()) != default(NetworkConnection))
        {
            mConnections.Add(c);
            Debug.Log("Accepted connection");
            //add the player. if it's the second player, do i need to tell each player the other is here?
            if (ServerGame.mainServerGame.AddPlayer(c) == 2)
            {
                ServerGame.mainServerGame.uiCtrl.CurrentStateString = "Two Players Connected";
                SendPackets(new Packet(Packet.Command.YoureFirst), new Packet(Packet.Command.YoureSecond), ServerGame.mainServerGame, mConnections[mConnections.Length - 1]);
            }
            else
            {
                ServerGame.mainServerGame.uiCtrl.CurrentStateString = "One Player Connected";
            }
        }

        //query driver for events
        DataStreamReader reader;
        //loop through each connection
        for(int i = 0; i < mConnections.Length; i++)
        {
            if (i > 2) Debug.Log("WEE WOO " + mConnections.Length);
            //if the connection doesn't exist, skip to the next connection
            if (!mConnections[i].IsCreated) continue;

            NetworkEvent.Type cmd;
            //for each event for this connection (until you get an empty one), do stuff
            while((cmd = mDriver.PopEventForConnection(mConnections[i], out reader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data) {
                    //Debug.Log("Recieved Data Event");
                    var readerCtxt = default(DataStreamReader.Context);
                    byte[] packetBuffer = reader.ReadBytesAsArray(ref readerCtxt, BUFFER_SIZE);
                    ParseRequest(packetBuffer, mConnections[i]);
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    ServerGame.mainServerGame.uiCtrl.CurrentStateString = "Player " + i + " disconnected";
                    mConnections[i] = default(NetworkConnection); //default gets the default value of whatever type
                }
            }
        }
    }
    
    public void SendPackets(Packet outPacket, Packet outPacketInverted, ServerGame serverGame, NetworkConnection connectionID)
    {
        //if it's not null, send the normal packet to the player that queried you
        if (outPacket != null)
            Send(outPacket, mDriver, connectionID, mPipeline);


        //if the inverted packet isn't null, send the packet to the correct player
        if (outPacketInverted == null || !ServerGame.mainServerGame.HasPlayer2()) return;
        //if the one that queried you is player 0,
        if (connectionID == serverGame.Players[0].ConnectionID)
            //send the inverted one to player 1.
            Send(outPacketInverted, mDriver, serverGame.Players[1].ConnectionID, mPipeline);
        //if the one that queried you is player 1,
        else
            //send the inverted one to player 0.
            Send(outPacketInverted, mDriver, serverGame.Players[0].ConnectionID, mPipeline);

    }

    //TODO: move all these to separate methods
    private void ParseRequest(byte[] buffer, NetworkConnection connectionID)
    {
        //Debug.Log("recieved packet");
        Packet packet = Deserialize(buffer);
        if (packet == null) return;

        Packet outPacket = null;
        Packet outPacketInverted = null;
        int playerIndex = ServerGame.mainServerGame.GetPlayerIndexFromID(connectionID);
        packet.InvertForController(playerIndex);
        //Debug.Log("packet command is " + packet.command + " for player index " + playerIndex);

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            case Packet.Command.AddToDeck:
                //figure out who's getting the card to their deck
                Player owner = ServerGame.mainServerGame.Players[playerIndex];
                Debug.Log("owner is " + owner + ", server game is " + ServerGame.mainServerGame + ", packet is " + packet + ", owner index is " + playerIndex);
                //.Log("deck ctrl is " + (owner.deckCtrl == null));
                //add the card in, with the cardCount being the card id, then increment the card count
                Card added = owner.deckCtrl.AddCard(packet.CardName, ServerGame.mainServerGame.cardCount, playerIndex);
                Debug.Log("added info is " + added.Owner + " and id: " + added.ID);
                Debug.Log("new get card id owner is " + ServerGame.mainServerGame.GetCardFromID(ServerGame.mainServerGame.cardCount).Owner + " and id is " + ServerGame.mainServerGame.cardCount);
                ServerGame.mainServerGame.cardCount++;
                //let everyone know
                outPacket = new Packet(Packet.Command.AddAsFriendly, packet.CardName, (int) Card.CardLocation.Deck, added.ID);
                outPacketInverted = new Packet(Packet.Command.IncrementEnemyDeck);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Augment:
                Card toAugment = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //if it's not a valid place to do, return
                if (ServerGame.mainServerGame.uiCtrl.DebugMode || ServerGame.mainServerGame.ValidAugment(toAugment, packet.X, packet.Y))
                {
                    packet.InvertForController(playerIndex);
                    //tell everyone to do it
                    outPacket = new Packet(Packet.Command.Augment, toAugment, packet.X, packet.Y);
                    if (toAugment.Location == Card.CardLocation.Discard || toAugment.Location == Card.CardLocation.Field)
                        outPacketInverted = new Packet(Packet.Command.Augment, toAugment, packet.X, packet.Y, true);
                    else outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toAugment.CardName, (int) Card.CardLocation.Field, toAugment.ID, packet.
                        X, packet.Y, true);
                    //play the card here
                    packet.InvertForController(playerIndex);
                    ServerGame.mainServerGame.Play(toAugment, packet.X, packet.Y);
                }
                else
                {
                    outPacket = new Packet(Packet.Command.PutBack);
                    outPacketInverted = null;
                }
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Play:
                //get the card to play
                Card toPlay = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //if it's not a valid place to do, return
                if (ServerGame.mainServerGame.uiCtrl.DebugMode || ServerGame.mainServerGame.ValidBoardPlay(toPlay, packet.X, packet.Y))
                {
                    packet.InvertForController(playerIndex);
                    //tell everyone to do it
                    outPacket = new Packet(Packet.Command.Play, toPlay, packet.X, packet.Y);
                    if (toPlay.Location == Card.CardLocation.Discard || toPlay.Location == Card.CardLocation.Field)
                        outPacketInverted = new Packet(Packet.Command.Play, toPlay, packet.X, packet.Y, true);
                    else outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toPlay.CardName, (int) Card.CardLocation.Field, toPlay.ID, packet.X, packet.Y, true);
                    //play the card here
                    packet.InvertForController(playerIndex);
                    ServerGame.mainServerGame.Play(toPlay, packet.X, packet.Y);
                }
                else
                {
                    outPacket = new Packet(Packet.Command.PutBack);
                    outPacketInverted = null;
                }
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Move:
                //get the card to move
                Card toMove = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                Debug.Log("packet card id is " + packet.cardID + "; its owner is " + toMove.Owner);
                //if it's not a valid place to do, return
                //NOTE: there is no debug to override moves because of how checking if attack works
                if (ServerGame.mainServerGame.ValidMove(toMove, packet.X, packet.Y))
                {
                    Debug.Log("move");
                    //move the card there
                    ServerGame.mainServerGame.Move(toMove, packet.X, packet.Y);
                    //re/de-invert the packet so it gets sent back correctly
                    packet.InvertForController(playerIndex);
                    //tell everyone to do it
                    outPacket = new Packet(Packet.Command.Move, toMove, packet.X, packet.Y);
                    outPacketInverted = new Packet(Packet.Command.Move, toMove, packet.X, packet.Y, true);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                //try to see if it's a valid attack, if it's not a valid move
                else if(ServerGame.mainServerGame.ValidAttack(toMove, packet.X, packet.Y))
                {
                    Debug.Log("attack");
                    //tell the players to put cards down where they were
                    outPacket = new Packet(Packet.Command.PutBack);
                    outPacketInverted = new Packet(Packet.Command.PutBack);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                    //then resolve the attack
                    //TODO allow for activation of abilities, fast cards
                    CharacterCard attacker = toMove as CharacterCard;
                    CharacterCard defender = ServerGame.mainServerGame.boardCtrl.GetCharAt(packet.X, packet.Y);
                    attacker.Attack(defender);

                    outPacket = new Packet(Packet.Command.SetNESW, attacker, attacker.N, attacker.E, attacker.S, attacker.W);
                    outPacketInverted = new Packet(Packet.Command.SetNESW, attacker, attacker.N, attacker.E, attacker.S, attacker.W);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                    outPacket = new Packet(Packet.Command.SetNESW, defender, defender.N, defender.E, defender.S, defender.W);
                    outPacketInverted = new Packet(Packet.Command.SetNESW, defender, defender.N, defender.E, defender.S, defender.W);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                else
                {
                    Debug.Log("putback");
                    outPacket = new Packet(Packet.Command.PutBack);
                    outPacketInverted = null;
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            case Packet.Command.Topdeck:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                Card toTopdeck = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Topdeck, toTopdeck);
                if (toTopdeck.Location == Card.CardLocation.Hand || toTopdeck.Location == Card.CardLocation.Deck)
                    outPacketInverted = new Packet(Packet.Command.Delete, toTopdeck);
                else outPacketInverted = null;
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Topdeck(toTopdeck);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Discard:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                Card toDiscard = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Discard, toDiscard);
                if (toDiscard.Location == Card.CardLocation.Discard || toDiscard.Location == Card.CardLocation.Field)
                    outPacketInverted = new Packet(Packet.Command.Discard, toDiscard);
                else outPacketInverted = new Packet(Packet.Command.AddAsEnemy, toDiscard.CardName, (int)Card.CardLocation.Discard, toDiscard.ID);
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Discard(toDiscard);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Rehand:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                Card toRehand = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Rehand, toRehand);
                if (toRehand.Location == Card.CardLocation.Hand || toRehand.Location == Card.CardLocation.Deck)
                    outPacketInverted = new Packet(Packet.Command.Delete, toRehand);
                else outPacketInverted = null; //TODO make this add a blank card
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Rehand(toRehand);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Draw:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                AttemptToDraw(playerIndex, connectionID);
                break;
            case Packet.Command.SetNESW:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                Card toSetNESW = ServerGame.mainServerGame.SetNESW(packet.cardID, packet.N, packet.E, packet.S, packet.W);
                //let everyone know to set NESW
                outPacket = new Packet(Packet.Command.SetNESW, toSetNESW, packet.N, packet.E, packet.S, packet.W);
                outPacketInverted = new Packet(Packet.Command.SetNESW, toSetNESW, packet.N, packet.E, packet.S, packet.W);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.SetPips:
                if (!ServerGame.mainServerGame.uiCtrl.DebugMode) break;

                SetPips(playerIndex, connectionID, packet.Pips);
                break;
            case Packet.Command.EndTurn:
                ServerGame.mainServerGame.SwitchTurn();
                break;
            case Packet.Command.Target:
                Card potentialTarget = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                Debug.Log("Client sent target " + potentialTarget.CardName);
                if(ServerGame.mainServerGame.CurrentlyResolvingEffect.CurrentlyResolvingSubeffect is TargetCardOnBoardSubeffect tcob)
                {
                    tcob.cardRestriction.Evaluate(potentialTarget, true);
                }
                break;
            case Packet.Command.TestTargetEffect:
                Card whoseEffToTest = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                Debug.Log("Running eff of " + whoseEffToTest.CardName);
                whoseEffToTest.Effects[0].StartResolution();
                break;
            default:
                Debug.Log("Invalid command " + packet.command + " to server from " + connectionID);
                break;
        }
    }

    public void AttemptToDraw(int playerIndex, NetworkConnection connectionID)
    {
        //draw and store what was drawn
        Card toDraw = ServerGame.mainServerGame.Draw(playerIndex);
        if (toDraw == null) return; //deck was empty
        //let everyone know to add that character to the correct hand
        Packet outPacket = new Packet(Packet.Command.Rehand, toDraw);
        //Packet outPacketInverted = new Packet(Packet.Command.Rehand, toDraw);
        Packet outPacketInverted = null;
        SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
    }

    public void SetPips(int playerIndex, NetworkConnection connectionID, int pipsToSet)
    {
        ServerGame.mainServerGame.Players[playerIndex].pips = pipsToSet;
        if (playerIndex == 0) ServerGame.mainServerGame.uiCtrl.UpdateFriendlyPips(pipsToSet);
        else ServerGame.mainServerGame.uiCtrl.UpdateEnemyPips(pipsToSet);
        //let everyone know
        Packet outPacket = new Packet(Packet.Command.SetPips, pipsToSet);
        Packet outPacketInverted = new Packet(Packet.Command.SetEnemyPips, pipsToSet);
        SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
    }

    public void SetTurn(NetworkConnection connectionID, int indexToSet)
    {
        Packet outPacket = new Packet(Packet.Command.EndTurn, indexToSet);
        Packet outPacketInverted = new Packet(Packet.Command.EndTurn, indexToSet);
        SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
    }

    public void AskClientForTarget(int playerIndex, Card card, int effectIndex, int subeffectIndex)
    {
        Packet outPacket = new Packet(Packet.Command.RequestBoardTarget, card, effectIndex, subeffectIndex);
        SendPackets(outPacket, null, ServerGame.mainServerGame, ServerGame.mainServerGame.Players[playerIndex].ConnectionID);
        Debug.Log("Asking for target");
    }

    public void SetNESW(CharacterCard card)
    {
        //let everyone know to set NESW
        Packet outPacket = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        Packet outPacketInverted = new Packet(Packet.Command.SetNESW, card, card.N, card.E, card.S, card.W);
        SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, ServerGame.mainServerGame.Players[0].ConnectionID);
    }
}
