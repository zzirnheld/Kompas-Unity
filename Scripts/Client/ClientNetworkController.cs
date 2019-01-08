using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkController : NetworkController {
    
    private int connectionID;

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

    public void ParseCommand(byte[] buffer)
    {
        Packet packet = Deserialize(buffer);
        if (packet == null) return;
        switch (packet.command)
        {
            case "Summon":
                //first create a new character card from the prefab
                CharacterCard charCard = Instantiate(characterPrefab).GetComponent<CharacterCard>();
                //set the image and information of the new character from the packet's info
                charCard.SetInfo(packet.serializedChar);
                charCard.SetImage(charCard.CardName);
                //play the character to the board
                Game.mainGame.boardCtrl.Summon(charCard, charCard.BoardX, charCard.BoardY, charCard.Friendly);
                //make sure the card scales correctly
                charCard.transform.localScale = absCardScale;
                break;
            case "Cast":
                //create a spell card from the prefab
                SpellCard spellCard = Instantiate(spellPrefab).GetComponent<SpellCard>();
                //set its info and image
                spellCard.SetInfo(packet.serializedSpell);
                spellCard.SetImage(spellCard.CardName);
                //play the char to the board TODO have server send reversed indices
                Game.mainGame.boardCtrl.Cast(spellCard, spellCard.BoardX, spellCard.BoardY, spellCard.Friendly);
                spellCard.transform.localScale = absCardScale;
                break;
            case "Augment":
                AugmentCard augCard = Instantiate(augmentPrefab).GetComponent<AugmentCard>();
                augCard.SetInfo(packet.serializedSpell);
                augCard.SetImage(augCard.CardName);
                Game.mainGame.boardCtrl.Augment(augCard, augCard.BoardX, augCard.BoardY, augCard.Friendly);
                augCard.transform.localScale = absCardScale;
                break;
            case "MoveChar":
                Game.mainGame.boardCtrl.Move(
                    Game.mainGame.boardCtrl.GetCharAt(6 - packet.serializedChar.BoardX, 6 - packet.serializedChar.BoardY),
                    6 - packet.x, 
                    6 - packet.y);
                break;
            case "MoveSpell":
                Game.mainGame.Move(
                    Game.mainGame.boardCtrl.GetSpellAt(6 - packet.serializedSpell.BoardX, 6 - packet.serializedSpell.BoardY),
                    6 - packet.x, 
                    6 - packet.y);
                break;
            case "SetNESW":
                Game.mainGame.boardCtrl.GetCharAt(6 - packet.serializedChar.BoardX, 6 - packet.serializedChar.BoardY)
                    .SetNESW(packet.serializedChar.n, packet.serializedChar.e, packet.serializedChar.s, packet.serializedChar.w);
                break;
            case "SetFriendlyPips":
                if(Game.mainGame is ClientGame) (Game.mainGame as ClientGame).FriendlyPips = packet.num;
                break;
        }
    }

}
