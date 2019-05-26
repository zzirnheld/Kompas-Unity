using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class ServerGame : Game {

    //TODO override all play, move, etc. methods to call base and tell players to do the same
    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public static ServerGame mainServerGame;

    public ServerNetworkController serverNetworkCtrl;

    int currPlayerCount = 0; //current number of players. shouldn't exceed 2
    public int cardCount = 0;

    public HandController player1HandCtrl;
    public DeckController player1DeckCtrl;
    public DiscardController player1DiscardCtrl;

    public GameObject player1HandObj;
    public GameObject player1DeckObj;
    public GameObject player1DiscardObj;

    public HandController player2HandCtrl;
    public DeckController player2DeckCtrl;
    public DiscardController player2DiscardCtrl;

    public GameObject player2HandObj;
    public GameObject player2DeckObj;
    public GameObject player2DiscardObj;

    private void Awake()
    {
        mainGame = this;
        mainServerGame = this;
        stack = new List<Effect>();
    }

    #region players
    public int AddPlayer(NetworkConnection connectionID)
    {
        if (currPlayerCount >= 2) return -1;

        players[currPlayerCount] = new Player(connectionID, currPlayerCount, this);
        if(currPlayerCount == 0)
        {
            players[0].handCtrl = player1HandCtrl;
            players[0].deckCtrl = player1DeckCtrl;
            players[0].discardCtrl = player1DiscardCtrl;
            players[0].handObject = player1HandObj;
            players[0].deckObject = player1DeckObj;
            players[0].discardObject = player1DiscardObj;
        }
        else if(currPlayerCount == 1)
        {
            players[1].handCtrl = player2HandCtrl;
            players[1].deckCtrl = player2DeckCtrl;
            players[1].discardCtrl = player2DiscardCtrl;
            players[1].handObject = player2HandObj;
            players[1].deckObject = player2DeckObj;
            players[1].discardObject = player2DiscardObj;
        }
        currPlayerCount++;
        return currPlayerCount;
    }

    public int GetPlayerIndexFromID(NetworkConnection connectionID)
    {
        for(int i = 0; i < currPlayerCount; i++)
        {
            if (players[i].ConnectionID == connectionID) return i;
        }

        return -1;
    }

    public Player GetPlayerFromID(NetworkConnection connectionID)
    {
        for (int i = 0; i < currPlayerCount; i++)
        {
            if (players[i].ConnectionID == connectionID) return players[i];
        }

        return null;
    }

    public bool HasPlayer2()
    {
        return currPlayerCount >= 2;
    }
    #endregion

    public void GiveTurnPlayerPips()
    {
        int pipsToSet = players[turnPlayer].pips + MaxCardsOnField;
        Players[turnPlayer].pips = pipsToSet;
        if (turnPlayer == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        serverNetworkCtrl.NotifySetPips(this, turnPlayer, pipsToSet, players[turnPlayer].ConnectionID);
    }

    //do action given id
    #region playerIDActions
    public void SetPipsGivenPlayerID(NetworkConnection connectionID, int numPips)
    {
        GetPlayerFromID(connectionID).pips = numPips;
    }

    public Card RemoveFromHandGivenPlayerID(NetworkConnection connectionID, int index)
    {
        return GetPlayerFromID(connectionID).handCtrl.RemoveFromHandAt(index);
    }

    public Card RemoveFromDiscardGivenPlayerID(NetworkConnection connectionID, int index)
    {
        return GetPlayerFromID(connectionID).discardCtrl.CardAt(index, true); //true for remove
    }

    public Card RemoveFromDeckGivenPlayerID(NetworkConnection connectionID, string name)
    {
        return GetPlayerFromID(connectionID).deckCtrl.RemoveCardWithName(name);
    }

    public Card DrawGivenPlayerID(NetworkConnection connectionID)
    {
        return Draw(GetPlayerIndexFromID(connectionID));
    }

    public void AddToDiscardGivenPlayerID(NetworkConnection connectionID, Card card)
    {
        Debug.Log("Discarding to discard of " + GetPlayerFromID(connectionID).index + ", card owner " + card.Owner);
        GetPlayerFromID(connectionID).discardCtrl.AddToDiscard(card);
    }
    #endregion

    //later, upgrade this with checking if the square is valid (adj or special case)
    #region check validity
    public bool ValidBoardPlay(Card card, int toX, int toY)
    {
        Debug.Log("Trying to play " + card.CardName + " to " + toX + ", " + toY);
        return card != null 
            && (card is CharacterCard || card is SpellCard)
            && boardCtrl.ValidIndices(toX, toY) 
            && boardCtrl.GetCardAt(toX, toY) == null;
    }

    public bool ValidAugment(Card card, int toX, int toY)
    {
        return card != null
            && card is AugmentCard
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCharAt(toX, toY) != null;
    }

    public bool ValidMove(Card toMove, int toX, int toY)
    {
        //Debug.Log("validmove checking move " + toMove.CardName + " to " + boardCtrl.GetCardAt(toX, toY) + "boardctrl " 
        //+ boardCtrl.GetCardAt(toX, toY).Owner + " tomove owner " + toMove.Owner);
        if (!(toMove is CharacterCard charToMove)) return false;
        return toMove.DistanceTo(toX, toY) <= charToMove.N
            && (boardCtrl.GetCardAt(toX, toY) == null || boardCtrl.GetCardAt(toX, toY).Owner == toMove.Owner);
    }

    public bool ValidAttack(Card toMove, int toX, int toY)
    {
        if (!(toMove is CharacterCard)) return false;
        return toMove.DistanceTo(toX, toY) == 1;
    }

    #endregion
    
    public void SwitchTurn()
    {
        turnPlayer = 1 - turnPlayer;
        GiveTurnPlayerPips();

        //reset everyone's M
        boardCtrl.ResetCardsM();

        //draw for turn and store what was drawn
        serverNetworkCtrl.DebugDraw(this, turnPlayer, players[turnPlayer].ConnectionID);
        serverNetworkCtrl.SetTurn(this, players[turnPlayer].ConnectionID, turnPlayer);
    }

    #region the stack
    public void PushToStack(Effect eff, int controller)
    {
        eff.serverGame = this;
        eff.effectController = controller;
        stack.Add(eff);
        stackIndex++;
    }

    public void PopFromStack()
    {
        if (stackIndex < 0) return;
        stack.RemoveAt(stackIndex);
        stackIndex--;
    }

    public void CancelStackEntry(int index)
    {
        if (index < 0) return;
        //TODO move the relevant card to grave? call a cancel method?
        stack.RemoveAt(index);
        stackIndex--;
    }

    public void ResolveNextStackEntry()
    {
        if (stackIndex < 0)
        {
            boardCtrl.DiscardSimples();
            return; //done with this stack!
        }
        Effect eff = stack[stackIndex];
        stack.RemoveAt(stackIndex);
        stackIndex--;
        eff.StartResolution();
    }

    public void FinishStackEntryResolution()
    {
        //TODO give opportunity to add things to stack
        //for now, just resolve the next stack entry
        ResolveNextStackEntry();
    }

    public void ResetPassingPriority()
    {
        foreach(Player p in players)
        {
            p.passedPriority = false;
        }
    }

    public void CheckForResponse()
    {
        //since a new thing is being put on the stack, mark both players as having not passed priority
        ResetPassingPriority();

        //check if responses exist. if not, resolve
        if (players[turnPlayer].HoldsPriority())
        {
            //then send them a request to do something or pass priority
            //TODO: send the stack entry encoded somehow?
        }
        else if(players[1 - turnPlayer].HoldsPriority()){
            //then mark the turn player as having passed priority
            players[turnPlayer].passedPriority = true;

            //then ask the non turn player to do something or pass priority
        }
        else
        {
            //if neither player has anything to do, resolve the stack
            ResolveNextStackEntry();
        }
    }
    #endregion the stack

    #region override game mechanics
    public override void Discard(Card card, int player = 0, bool ignoreClientServer = false)
    {
        base.Discard(card, player, ignoreClientServer);
        serverNetworkCtrl.NotifyDiscard(this, card, players[player].ConnectionID);
    }

    public override Card Draw(int player = 0)
    {
        Card c = base.Draw(player);
        serverNetworkCtrl.NotifyDraw(this, c, players[player].ConnectionID);
        return c;
    }

    public override void Move(Card card, int toX, int toY)
    {
        base.Move(card, toX, toY);
        serverNetworkCtrl.NotifyMove(this, card, toX, toY, players[card.Owner].ConnectionID);
    }

    public override void Play(Card card, int toX, int toY, int player, bool remove = true)
    {
        base.Play(card, toX, toY, player, remove);
        int invertedX = serverNetworkCtrl.InvertIndexForController(toX, player);
        int invertedY = serverNetworkCtrl.InvertIndexForController(toY, player);
        serverNetworkCtrl.NotifyPlay(this, card, invertedX, invertedY, players[player].ConnectionID);
    }

    public override void Rehand(Card card, int player = 0)
    {
        base.Rehand(card, player);
        serverNetworkCtrl.NotifyRehand(this, card, players[player].ConnectionID);
    }

    public override CharacterCard SetNESW(int cardID, int n, int e, int s, int w)
    {
        CharacterCard c =  base.SetNESW(cardID, n, e, s, w);
        if(c != null) serverNetworkCtrl.NotifySetNESW(this, c);
        return c;
    }

    public override void Swap(Card card, int toX, int toY)
    {
        base.Swap(card, toX, toY);
        serverNetworkCtrl.NotifyMove(this, card, toX, toY, players[card.Owner].ConnectionID);
    }

    public override void Topdeck(Card card, int player = 0)
    {
        base.Topdeck(card, player);
        serverNetworkCtrl.NotifyTopdeck(this, card, players[player].ConnectionID);
    }
    #endregion override game mechanics
}
