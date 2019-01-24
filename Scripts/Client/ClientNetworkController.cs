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
            case "Summon":
                //first create a new character card from the prefab
                CharacterCard charCard = Instantiate(characterPrefab).GetComponent<CharacterCard>();
                //set the image and information of the new character from the packet's info
                charCard.SetInfo(packet.serializedChar);
                charCard.SetImage(charCard.CardName);
                //play the character to the board
                Game.mainGame.boardCtrl.Summon(charCard, charCard.BoardX, charCard.BoardY, charCard.Owner);
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
                Game.mainGame.boardCtrl.Cast(spellCard, spellCard.BoardX, spellCard.BoardY, spellCard.Owner);
                spellCard.transform.localScale = absCardScale;
                break;
            case "Augment":
                AugmentCard augCard = Instantiate(augmentPrefab).GetComponent<AugmentCard>();
                augCard.SetInfo(packet.serializedSpell);
                augCard.SetImage(augCard.CardName);
                Game.mainGame.boardCtrl.Augment(augCard, augCard.BoardX, augCard.BoardY, augCard.Owner);
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
            case "SetEnemyPips":
                if (Game.mainGame is ClientGame) (Game.mainGame as ClientGame).EnemyPips = packet.num;
                break;
            case "RemoveFromBoard":
                Game.mainGame.boardCtrl.RemoveFromBoard(packet.x, packet.y);
                break;
            case "RemoveFromHand": //num is the index in the hand to remove at TODO for enemy hand
                ClientGame.mainClientGame.friendlyHandCtrl.RemoveFromHandAt(packet.num);
                break;
            case "AddToDiscard": //don't need an enemy version because .Discard puts it in the correct one given the owner of toDiscard
                Card toDiscard = GetCardFromPacket(packet);
                ClientGame.mainClientGame.Discard(toDiscard, toDiscard.Owner, true);
                break;
            case "AddToHand":
                Card toHand = GetCardFromPacket(packet);
                ClientGame.mainClientGame.friendlyHandCtrl.AddToHand(toHand);
                break;
            case "IncrementEnemyHand":
                Card toEnemyHand = ClientGame.mainClientGame.enemyDeckCtrl.InstantiateBlankCard();
                ClientGame.mainClientGame.enemyHandCtrl.AddToHand(toEnemyHand);
                break;
            case "DecrementEnemyHand":
                ClientGame.mainClientGame.enemyHandCtrl.RemoveRandomCard();
                break;
            case "RemoveFromFriendlyDeck":
                ClientGame.mainClientGame.friendlyDeckCtrl.RemoveCardWithName(packet.args);
                break;
            case "RemoveFromEnemyDeck":
                ClientGame.mainClientGame.enemyDeckCtrl.RemoveCardWithName(packet.args);
                break;
            case "Draw":
                ClientGame.mainClientGame.friendlyDeckCtrl.RemoveCardWithName(packet.args);
                ClientGame.mainClientGame.friendlyHandCtrl.AddToHand(GetCardFromPacket(packet));
                break;
            case "AddToDeck":
                ClientGame.mainClientGame.friendlyDeckCtrl.PushTopdeck(GetCardFromPacket(packet));
                break;
            case "AddToEnemyDeck":
                ClientGame.mainClientGame.enemyDeckCtrl.AddBlankCard();
                break;
            case "RemoveFromDiscard":
                ClientGame.mainClientGame.friendlyDiscardCtrl.RemoveFromDiscardAt(packet.num);
                break;
            case "RemoveFromEnemyDiscard":
                ClientGame.mainClientGame.enemyDiscardCtrl.RemoveFromDiscardAt(packet.num);
                break;
            case "Remove":
                RemoveCard(GetSerializableCardFromPacket(packet));
                break;
            default:
                Debug.Log("Unrecognized command sent to client");
                break;
        }
    }

    #region Request Actions
    public void RequestSummon(CharacterCard card, int toX, int toY)
    {
        Packet packet = new Packet(card, "Request Summon", toX, toY);
        Send(packet, connectionID);
    }

    public void RequestCast(SpellCard card, int toX, int toY)
    {
        Packet packet = new Packet(card, "Request Cast", toX, toY);
        Send(packet, connectionID);
    }

    public void RequestAugment(AugmentCard card, int toX, int toY)
    {
        Packet packet = new Packet(card, "Request Augment", toX, toY);
        Send(packet, connectionID);
    }

    public void RequestPlay(Card card, int toX, int toY)
    {
        if (card is CharacterCard) RequestSummon(card as CharacterCard, toX, toY);
        else if (card is SpellCard) RequestCast(card as SpellCard, toX, toY);
        else if (card is AugmentCard) RequestAugment(card as AugmentCard, toX, toY);
        else throw new NotImplementedException();
    }

    public void RequestMoveChar(CharacterCard card, int toX, int toY)
    {
        Packet packet = new Packet(card, "Request Move Char", toX, toY);
        Send(packet, connectionID);
    }

    public void RequestMoveSpell(SpellCard card, int toX, int toY)
    {
        Packet packet = new Packet(card, "Request Move Spell", toX, toY);
        Send(packet, connectionID);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        if (card is CharacterCard) RequestMoveChar(card as CharacterCard, toX, toY);
        else if (card is SpellCard) RequestMoveSpell(card as SpellCard, toX, toY);
        else throw new NotImplementedException();
    }

    public void RequestRemoveFromHand(Card card)
    {
        Packet packet = new Packet("Request Remove From Hand", card.GetIndexInList());
        Send(packet, connectionID);
    }

    public void RequestRemoveFromBoard(Card card)
    {
        Packet packet = new Packet("Request Remove From Hand", card.BoardX, card.BoardY);
        Send(packet, connectionID);
    }

    public void RequestRemoveFromDiscard(Card card)
    {
        Packet packet = new Packet("Request Remove From Discard", card.GetIndexInList());
        Send(packet, connectionID);
    }

    public void RequestRemoveFromDeck(Card card)
    {
        Packet packet = new Packet("Request Remove From Deck", card.CardName);
        Send(packet, connectionID);
    }

    public void RequestRemove(Card card)
    {
        if (card.Location == Card.CardLocation.Hand) RequestRemoveFromHand(card);
        else if (card.Location == Card.CardLocation.Field) RequestRemoveFromBoard(card);
        else if (card.Location == Card.CardLocation.Discard) RequestRemoveFromDiscard(card);
        else if (card.Location == Card.CardLocation.Deck) RequestRemoveFromDeck(card);
    }

#endregion

}
