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


        //if the inverted packet isn't null, send the packet to the correct player
        if (outPacketInverted == null) return;
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

        Packet outPacket = null;
        Packet outPacketInverted = null;
        int playerIndex = ServerGame.mainServerGame.GetPlayerIndexFromID(connectionID);
        packet.RepairOwner(playerIndex);

        //switch between all the possible requests for the server to handle.
        switch (packet.command)
        {
            //all of the request things include a former request to remove?
            case "Request Summon":
                //first create a new character card from the prefab
                CharacterCard charCard = Instantiate(characterPrefab).GetComponent<CharacterCard>();
                //set the image and information of the new character from the packet's info
                charCard.SetInfo(packet.serializedChar);

                //check if it's a valid location
                if (ServerGame.mainServerGame.ValidBoardPlay(charCard, packet.x, packet.y))
                {
                    //if so, remove the character from where it is, summon the character there
                    RemoveCard(packet.serializedChar);
                    ServerGame.mainServerGame.Play(charCard, packet.x, packet.y, playerIndex, false);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the character and the inverted one
                    outPacket = new Packet(charCard, "Summon");
                    outPacketInverted = new Packet(charCard, "Summon", true);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            case "Request Cast":
                //first create a new spell card from the prefab
                SpellCard spellCard = Instantiate(spellPrefab).GetComponent<SpellCard>();
                //set the image and information of the new spell from the packet's info
                spellCard.SetInfo(packet.serializedSpell);

                //check if it's a valid location
                if (ServerGame.mainServerGame.ValidBoardPlay(spellCard, packet.x, packet.y))
                {
                    //if so, cast the spell there
                    RemoveCard(packet.serializedSpell);
                    ServerGame.mainServerGame.Play(spellCard, packet.x, packet.y, playerIndex, false);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the spell and the inverted one
                    outPacket = new Packet(spellCard, "Cast");
                    outPacketInverted = new Packet(spellCard, "Cast", true);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            case "Request Augment":
                //first create a new augment card from the prefab
                AugmentCard augmentCard = Instantiate(augmentPrefab).GetComponent<AugmentCard>();
                //set the image and information of the new spell from the packet's info
                augmentCard.SetInfo(packet.serializedAug);

                //check if it's a valid location
                if (ServerGame.mainServerGame.ValidBoardPlay(augmentCard, packet.x, packet.y))
                {
                    //if so, apply the augment there
                    RemoveCard(packet.serializedAug);
                    ServerGame.mainServerGame.Play(augmentCard, packet.x, packet.y, playerIndex, false);

                    //then notify the client that sent the request
                    //create a packet with the normal version of the augment and the inverted one
                    outPacket = new Packet(augmentCard, "Augment");
                    outPacketInverted = new Packet(augmentCard, "Augment", true);
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            case "Request Move Char":
                //first get who's moving
                CharacterCard toMove = ServerGame.mainServerGame.boardCtrl.GetCharAt(packet.serializedChar.BoardX, packet.serializedChar.BoardY);
                //then check if legal move
                if (ServerGame.mainServerGame.ValidMove(toMove, packet.x, packet.y))
                {
                    //create notification packets before charToMove's x/y become packet.x and packet.y
                    outPacket = new Packet(toMove, "MoveChar", packet.x, packet.y);
                    outPacketInverted = new Packet(toMove, "MoveChar", packet.x, packet.y, true); //true for inverted
                    //if legal, do the move
                    ServerGame.mainServerGame.Move(toMove, packet.x, packet.y);
                    //send packets
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            case "Request Move Spell":
                //first get who's moving
                SpellCard spellToMove = ServerGame.mainServerGame.boardCtrl.GetSpellAt(packet.serializedSpell.BoardX, packet.serializedSpell.BoardY);
                //then check if legal move
                if (ServerGame.mainServerGame.ValidMove(spellToMove, packet.x, packet.y))
                {
                    //create notification packets before spellToMove's x/y become packet.x and packet.y
                    outPacket = new Packet(spellToMove, "MoveSpell", packet.x, packet.y);
                    outPacketInverted = new Packet(spellToMove, "MoveSpell", packet.x, packet.y, true); //true for inverted
                    //if legal, do the move
                    ServerGame.mainServerGame.Move(spellToMove, packet.x, packet.y);
                    //and send packets
                    SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                }
                break;
            //TODO do for augment?
            case "Request Update Pips":
                //actually update the pips
                ServerGame.mainServerGame.SetPipsGivenPlayerID(connectionID, packet.num);
                outPacket = new Packet("SetFriendlyPips", packet.num);
                outPacketInverted = new Packet("SetEnemyPips", packet.num);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Remove From Hand":
                ServerGame.mainServerGame.RemoveFromHandGivenPlayerID(connectionID, packet.num);
                outPacket = new Packet("RemoveFromHand", packet.num);
                outPacketInverted = new Packet("DecrementEnemyHand");
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Remove From Board":
                ServerGame.mainServerGame.boardCtrl.RemoveFromBoard(packet.x, packet.y);
                outPacket = new Packet("RemoveFromBoard", packet.x, packet.y);
                outPacketInverted = new Packet("RemoveFromBoard", packet.x, packet.y, true);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Remove From Discard":
                ServerGame.mainServerGame.RemoveFromDiscardGivenPlayerID(connectionID, packet.num);
                outPacket = new Packet("RemoveFromDiscard", packet.num);
                outPacketInverted = new Packet("RemoveFromEnemyDiscard", packet.num);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Remove From Deck":
                ServerGame.mainServerGame.RemoveFromDeckGivenPlayerID(connectionID, packet.args);
                outPacket = new Packet("RemoveFromFriendlyDeck", packet.args);
                outPacketInverted = new Packet("RemoveFromEnemyDeck", packet.args);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Draw":
                Card drawn = ServerGame.mainServerGame.DrawGivenPlayerID(connectionID);
                //i want for the client to have the list of cards in their deck, but just not in the right order
                //tell the client who's drawing to draw the card we think is right for them to draw
                outPacket = new Packet(drawn, "Draw", drawn.CardName);
                //other player gets told to remove that card from enemy deck TODO make other player not have this info, only num cards
                outPacketInverted = new Packet("RemoveFromEnemyDeck", drawn.CardName);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                //and make sure that the other player also knows to increment their enemy's hand size
                outPacketInverted = new Packet("IncrementEnemyHand");
                SendPackets(null, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Discard From Hand":
                //remove it from the hand
                Card toDiscard = ServerGame.mainServerGame.RemoveFromHandGivenPlayerID(connectionID, packet.num);
                outPacket = new Packet("RemoveFromHand", packet.num);
                outPacketInverted = new Packet("DecrementEnemyHand");
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                //and add it to the discard
                ServerGame.mainServerGame.AddToDiscardGivenPlayerID(connectionID, toDiscard);
                outPacket = new Packet(toDiscard, "AddToDiscard");
                outPacketInverted = new Packet(toDiscard, "AddToDiscard", true); //addtodiscard will check the owner of the serialized card
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            //TODO request kill, reshuffle, rehand, topdck, bottomdeck (with new client-has-unordered-copy paradigm)
            case "Request Reshuffle":
                //remove it from where it is
                RemoveCard(GetSerializableCardFromPacket(packet));

                Card toReshuffle = GetCardFromPacket(packet);
                //and shuffle it in
                ServerGame.mainServerGame.Players[toReshuffle.Owner].deckCtrl.ShuffleIn(toReshuffle); //todo account for ctrl change
                //then let everyone know
                outPacket = new Packet(toReshuffle, "Remove");
                outPacketInverted = new Packet(toReshuffle, "Remove", true);
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                outPacket = new Packet(toReshuffle, "AddToDeck");
                outPacketInverted = new Packet("AddToEnemyDeck");
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Topdeck":
                Card toTopdeck = GetCardFromPacket(packet);
                //topdeck it
                ServerGame.mainServerGame.Players[toTopdeck.Owner].deckCtrl.PushTopdeck(toTopdeck);
                //then let everyone know
                outPacket = new Packet(toTopdeck, "PushTopdeck");
                outPacketInverted = new Packet("AddToEnemyDeck");
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            case "Request Add To Discard":
                Card toAddToDiscard = GetCardFromPacket(packet);
                ServerGame.mainServerGame.Players[toAddToDiscard.Owner].discardCtrl.AddToDiscard(toAddToDiscard);
                outPacket = new Packet(toAddToDiscard, "PushTopdeck");
                outPacketInverted = new Packet("AddToEnemyDeck");
                SendPackets(outPacket, outPacketInverted, ServerGame.mainServerGame, connectionID);
                break;
            default:
                break;
        }

        //TODO change to using a card ID system for once i make tags, so that it just references the card itself
        

    }


}
