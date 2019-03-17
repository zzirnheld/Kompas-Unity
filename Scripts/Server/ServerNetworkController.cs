using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkController : NetworkController {

    private int recievedConnectionID; //the connection id that is recieved with the network transport layer packet

    void Update()
    {
        if (!hosting) return; //self-explanatory
        if (!(Game.mainGame is ServerGame)) return; //this controller should only deal with the networking if the current game is a server game.

        //the "out" arguments in the following method mean that the things are being passed by reference,
        //and will be changed by the function

        recData = NetworkTransport.Receive(out hostID, out recievedConnectionID, out channelID,
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
                //add the player. if it's the second player, do i need to tell each player the other is here?
                if (ServerGame.mainServerGame.AddPlayer(recievedConnectionID) == 2)
                {
                    ServerGame.mainServerGame.uiCtrl.CurrentStateString = "Two Players Connected";
                }
                else
                {
                    ServerGame.mainServerGame.uiCtrl.CurrentStateString = "One Player Connected";
                }

                break;
            //they've actually sent something
            case NetworkEventType.DataEvent:
                Debug.Log("Recieved Data Event");
                ParseRequest(recBuffer, recievedConnectionID);
                break;
            //the person has disconnected
            case NetworkEventType.DisconnectEvent:
                break;
            //i don't know what this does and i don't 
            case NetworkEventType.BroadcastEvent:
                break;
        }

    }


    private void SendPackets(Packet outPacket, Packet outPacketInverted, ServerGame serverGame, int connectionID)
    {
        //if it's not null, send the normal packet to the player that queried you
        if (outPacket != null)
            Send(outPacket, connectionID);


        //if the inverted packet isn't null, send the packet to the correct player
        if (outPacketInverted == null || !ServerGame.mainServerGame.HasPlayer2()) return;
        //if the one that queried you is player 0,
        if (connectionID == serverGame.Players[0].ConnectionID)
            //send the inverted one to player 1.
            Send(outPacketInverted, serverGame.Players[1].ConnectionID);
        //if the one that queried you is player 1,
        else
            //send the inverted one to player 0.
            Send(outPacketInverted, serverGame.Players[0].ConnectionID);

    }

    private void ParseRequest(byte[] buffer, int connectionID)
    {
        Debug.Log("recieved packet");
        Packet packet = Deserialize(buffer);
        if (packet == null) return;

        Debug.Log("packet command is " + packet.command);

        Packet outPacket = null;
        Packet outPacketInverted = null;
        int playerIndex = ServerGame.mainServerGame.GetPlayerIndexFromID(connectionID);
        packet.InvertForController(playerIndex);

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            case Packet.Command.AddToDeck:
                //figure out who's getting the card to their deck
                Player owner = ServerGame.mainServerGame.Players[playerIndex];
                Debug.Log("owner is " + owner + ", server game is " + ServerGame.mainServerGame + ", packet is " + packet + ", owner index is " + playerIndex);
                Debug.Log("deck ctrl is " + (owner.deckCtrl == null));
                //add the card in, with the cardCount being the card id, then increment the card count
                Card added = owner.deckCtrl.AddCard(packet.args, ServerGame.mainServerGame.cardCount++, playerIndex);
                //let everyone know
                outPacket = new Packet(Packet.Command.AddToDeck, packet.args, added.ID);
                outPacketInverted = new Packet(Packet.Command.AddToEnemyDeck, packet.args, added.ID);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Play:
                //get the card to play
                Card toPlay = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //if it's not a valid place to do, return
                if (ServerGame.mainServerGame.ValidBoardPlay(toPlay, packet.x, packet.y))
                {
                    //play the card here
                    ServerGame.mainServerGame.Play(toPlay, packet.x, packet.y);
                    //re/de-invert the packet so it gets sent back correctly
                    packet.InvertForController(playerIndex);
                    //tell everyone to do it
                    outPacket = new Packet(Packet.Command.Play, toPlay, packet.x, packet.y);
                    outPacketInverted = new Packet(Packet.Command.Play, toPlay, packet.x, packet.y, true);
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
                //if it's not a valid place to do, return
                if (ServerGame.mainServerGame.ValidMove(toMove, packet.x, packet.y))
                {
                    //play the card here
                    ServerGame.mainServerGame.Move(toMove, packet.x, packet.y);
                    //re/de-invert the packet so it gets sent back correctly
                    packet.InvertForController(playerIndex);
                    //tell everyone to do it
                    outPacket = new Packet(Packet.Command.Move, toMove, packet.x, packet.y);
                    outPacketInverted = new Packet(Packet.Command.Move, toMove, packet.x, packet.y, true);
                }
                else
                {
                    outPacket = new Packet(Packet.Command.PutBack);
                    outPacketInverted = null;
                }
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Topdeck:
                Card toTopdeck = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Topdeck(toTopdeck);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Topdeck, toTopdeck);
                outPacketInverted = outPacket;
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Discard:
                Card toDiscard = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Discard(toDiscard);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Discard, toDiscard);
                outPacketInverted = outPacket;
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Rehand:
                Card toRehand = ServerGame.mainServerGame.GetCardFromID(packet.cardID);
                //eventually, this won't be necessary, because the player won't initiate this action
                ServerGame.mainServerGame.Rehand(toRehand);
                //and let everyone know
                outPacket = new Packet(Packet.Command.Rehand, toRehand);
                outPacketInverted = outPacket;
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.Draw:
                //draw and store what was drawn
                Card toDraw = ServerGame.mainServerGame.Draw(playerIndex);
                //let everyone know to add that character to the correct hand
                outPacket = new Packet(Packet.Command.Rehand, toDraw);
                outPacketInverted = new Packet(Packet.Command.Rehand, toDraw);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.SetNESW:
                Card toSetNESW = ServerGame.mainServerGame.SetNESW(packet.cardID, packet.n, packet.e, packet.s, packet.w);
                //let everyone know to set NESW
                outPacket = new Packet(Packet.Command.SetNESW, toSetNESW, packet.n, packet.e, packet.s, packet.w);
                outPacketInverted = new Packet(Packet.Command.SetNESW, toSetNESW, packet.n, packet.e, packet.s, packet.w);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case Packet.Command.SetPips:
                ServerGame.mainServerGame.Players[playerIndex].pips = packet.num;
                if (playerIndex == 0) ServerGame.mainServerGame.uiCtrl.UpdateFriendlyPips(packet.num);
                else ServerGame.mainServerGame.uiCtrl.UpdateEnemyPips(packet.num);
                //let everyone know
                outPacket = new Packet(Packet.Command.SetPips, packet.num);
                outPacketInverted = new Packet(Packet.Command.SetEnemyPips, packet.num);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            default:
                Debug.Log("Invalid command " + packet.command + " to server from " + connectionID);
                break;
        }
    }
}
