using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerGame : Game {

    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public static ServerGame mainServerGame;

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
        stack = new List<IStackable>();
        triggerMap = new Dictionary<TriggerCondition, List<Trigger>>();
        foreach(TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            triggerMap.Add(c, new List<Trigger>());
        }
    }

    #region players
    public int AddPlayer(TcpClient tcpClient)
    {
        if (currPlayerCount >= 2) return -1;

        players[currPlayerCount].SetInfo(tcpClient, currPlayerCount, this);
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
            players[0].enemy = players[1];
            players[1].enemy = players[0];
        }
        currPlayerCount++;
        return currPlayerCount;
    }

    public bool HasPlayer2()
    {
        return currPlayerCount >= 2;
    }
    #endregion

    public void Lose(int playerIndex)
    {
        //TODO
    }

    public void GivePlayerPips(Player player, int pipsToSet)
    {
        player.pips = pipsToSet;
        if (player.index == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        TurnPlayer.ServerNotifier.NotifySetPips(pipsToSet);
    }

    public void GiveTurnPlayerPips()
    {
        int pipsToSet = TurnPlayer.pips + MaxCardsOnField;
        GivePlayerPips(TurnPlayer, pipsToSet);
    }

    public void CheckForDeath(CharacterCard toCheck, IStackable stackSrc)
    {
        if(toCheck.E <= 0)
        {
            //first, trigger anything that would go off of this thing dying, so it knows it's about to die (moving from field)
            Trigger(TriggerCondition.Discard, toCheck, stackSrc, null);
            //then notify the players
            toCheck.Controller.ServerNotifier.NotifyDiscard(toCheck);
            //then actually discard it
            toCheck.Discard();
            //don't call check for response on stack because anything that causes things to die,
            //attacks or effects, will call check for response once it's done resolving.
        }
    }

    //later, upgrade this with checking if the square is valid (adj or special case)
    #region check validity
    public bool ValidBoardPlay(Card card, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid play");
            return true;
        }

        Debug.Log("Trying to play " + card.CardName + " to " + toX + ", " + toY);
        return card != null 
            && (card is CharacterCard || card is SpellCard)
            && boardCtrl.ValidIndices(toX, toY) 
            && boardCtrl.GetCardAt(toX, toY) == null;
    }

    public bool ValidAugment(Card card, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid augment");
            return true;
        }

        return card != null
            && card is AugmentCard
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCharAt(toX, toY) != null;
    }

    public bool ValidMove(Card toMove, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid move");
            return true;
        }

        if (!(toMove is CharacterCard charToMove)) return false;
        return toMove.DistanceTo(toX, toY) <= charToMove.N
            && (boardCtrl.GetCardAt(toX, toY) == null || boardCtrl.GetCardAt(toX, toY).ControllerIndex == toMove.ControllerIndex);
    }

    public bool ValidAttack(Card toMove, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid attack");
            return true;
        }

        if (!(toMove is CharacterCard)) return false;
        return toMove.DistanceTo(toX, toY) == 1;
    }

    #endregion
    
    public void SwitchTurn()
    {
        turnPlayer = 1 - turnPlayer;
        GiveTurnPlayerPips();

        //reset everyone's M
        boardCtrl.ResetCardsForTurn();

        //draw for turn and store what was drawn
        Card drawn = Draw();
        if(drawn != null) TurnPlayer.ServerNotifier.NotifyDraw(drawn);
        TurnPlayer.ServerNotifier.NotifySetTurn(this, turnPlayer);

        //trigger turn start effects
        Trigger(TriggerCondition.TurnStart, null, null, null);
        CheckForResponse();
    }

    #region the stack
    public void PushToStack(IStackable eff, int controller)
    {
        stack.Add(eff);
        stackIndex++;
    }

    public void PushToStack(Effect eff, int controller)
    {
        eff.serverGame = this;
        eff.effectControllerIndex = controller;
        PushToStack(eff as IStackable, controller);
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
        stack.RemoveAt(index);
        stackIndex--;
    }

    public void ResolveNextStackEntry()
    {
        if (stackIndex < 0)
        {
            TurnPlayer.ServerNotifier.DiscardSimples();
            boardCtrl.DiscardSimples();
            return; //done with this stack!
        }
        IStackable eff = stack[stackIndex];
        stack.RemoveAt(stackIndex);
        stackIndex--;
        eff.StartResolution();
    }

    /// <summary>
    /// The last effect that resolved is now done.
    /// </summary>
    public void FinishStackEntryResolution()
    {
        CheckForResponse();
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

    #region triggers
    public void Trigger(TriggerCondition condition, Card triggerer, IStackable stackTrigger, int? x)
    {
        Debug.Log($"Attempting to trigger {condition}, with triggerer {triggerer.CardName}, triggered by a null stacktrigger? {stackTrigger == null}, x={x}");
        foreach (Trigger t in triggerMap[condition])
        {
            t.TriggerIfValid(triggerer, stackTrigger, x);
        }
    }

    public void RegisterTrigger(TriggerCondition condition, Trigger trigger)
    {
        List<Trigger> triggers = triggerMap[condition];
        if (triggers == null)
        {
            triggers = new List<Trigger>();
            triggerMap.Add(condition, triggers);
        }
        triggers.Add(trigger);
    }
    #endregion triggers
}
