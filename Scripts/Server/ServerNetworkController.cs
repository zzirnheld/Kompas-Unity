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
                break;
            //they've actually sent something
            case NetworkEventType.DataEvent:
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

        //if the inverted packet isn't null, send the packet to the 
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
        Packet packet = Deserialize(buffer);
        if (packet == null) return;

        ServerGame serverGame = Game.mainGame as ServerGame;
        Packet outPacket = null;
        Packet outPacketInverted = null;

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            case "Request Summon":
                //first create a new character card from the prefab
                CharacterCard charCard = Instantiate(characterPrefab).GetComponent<CharacterCard>();
                //set the image and information of the new character from the packet's info
                charCard.SetInfo(packet.serializedChar);

                //check if it's a valid location
                if (serverGame.ValidBoardPlay(charCard, packet.x, packet.y))
                {
                    //if so, summon the character there
                    serverGame.Play(charCard, packet.x, packet.y);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the character and the inverted one
                    outPacket = new Packet(charCard, "Summon");
                    outPacketInverted = new Packet(charCard, "Summon", true);
                    SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                }
                break;
            case "Request Cast":
                //first create a new spell card from the prefab
                SpellCard spellCard = Instantiate(spellPrefab).GetComponent<SpellCard>();
                //set the image and information of the new spell from the packet's info
                spellCard.SetInfo(packet.serializedSpell);

                //check if it's a valid location
                if (serverGame.ValidBoardPlay(spellCard, packet.x, packet.y))
                {
                    //if so, cast the spell there
                    serverGame.Play(spellCard, packet.x, packet.y);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the spell and the inverted one
                    outPacket = new Packet(spellCard, "Cast");
                    outPacketInverted = new Packet(spellCard, "Cast", true);
                    SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                }
                break;
            case "Request Augment":
                //first create a new augment card from the prefab
                AugmentCard augmentCard = Instantiate(augmentPrefab).GetComponent<AugmentCard>();
                //set the image and information of the new spell from the packet's info
                augmentCard.SetInfo(packet.serializedAug);

                //check if it's a valid location
                if (serverGame.ValidBoardPlay(augmentCard, packet.x, packet.y))
                {
                    //if so, apply the augment there
                    serverGame.Play(augmentCard, packet.x, packet.y);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the augment and the inverted one
                    outPacket = new Packet(augmentCard, "Augment");
                    outPacketInverted = new Packet(augmentCard, "Augment", true);
                    SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                }
                break;
            case "Request Move Char":
                //first get who's moving
                CharacterCard toMove = serverGame.boardCtrl.GetCharAt(packet.serializedChar.BoardX, packet.serializedChar.BoardY);
                CharacterCard charAtDest = serverGame.boardCtrl.GetCharAt(packet.x, packet.y);
                //then check if legal move
                if (serverGame.ValidMove(toMove, packet.x, packet.y))
                    //if legal, do the move
                    serverGame.Move(toMove, packet.x, packet.y);

                //create and send notification packets
                //for first char
                outPacket = new Packet(toMove, "MoveChar");
                outPacketInverted = new Packet(toMove, "MoveChar", true); //true for inverted
                SendPackets(outPacket, outPacketInverted, serverGame, connectionID);

                //for second char, if was a swap
                if(charAtDest != null)
                {
                    outPacket = new Packet(charAtDest, "MoveChar");
                    outPacketInverted = new Packet(charAtDest, "MoveChar", true);
                    SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                }

                break;
            case "Request Move Spell":
                //first get who's moving
                SpellCard spellToMove = serverGame.boardCtrl.GetSpellAt(packet.serializedSpell.BoardX, packet.serializedSpell.BoardY);
                SpellCard spellAtDest = serverGame.boardCtrl.GetSpellAt(packet.x, packet.y);
                //then check if legal move
                if (serverGame.ValidMove(spellToMove, packet.x, packet.y))
                    //if legal, do the move
                    serverGame.Move(spellToMove, packet.x, packet.y);

                //create and send notification packets
                //for first spell
                outPacket = new Packet(spellToMove, "MoveSpell");
                outPacketInverted = new Packet(spellToMove, "MoveSpell", true); //true for inverted
                SendPackets(outPacket, outPacketInverted, serverGame, connectionID);

                //for second spell, if was a swap
                if (spellAtDest != null)
                {
                    outPacket = new Packet(spellAtDest, "MoveChar");
                    outPacketInverted = new Packet(spellAtDest, "MoveChar", true);
                    SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                }

                break;
            //TODO do for augment?
            case "Request Update Pips":
                outPacket = new Packet("SetFriendlyPips", packet.num);
                outPacketInverted = new Packet("SetEnemyPips", packet.num);
                SendPackets(outPacket, outPacketInverted, serverGame, connectionID);
                break;
            //TODO discarding a card, rehanding, reshuffling, topdecking, bottomdecking
            //case "Request Discard From Hand":
                //outPacket = new Packet("RemoveFromHand", packet.num);
                //outPacketInverted
                
            default:
                break;
        }


        

    }
    

}
