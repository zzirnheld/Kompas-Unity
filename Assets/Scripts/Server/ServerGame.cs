using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ServerGame : Game {

    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public readonly object SetAvatarLock = new object();

    public GameObject AvatarPrefab;

    public override Player[] Players => ServerPlayers;
    public ServerPlayer[] ServerPlayers;
    public ServerPlayer TurnServerPlayer { get { return ServerPlayers[turnPlayer]; } }
    public int cardCount = 0;
    private int currPlayerCount = 0; //current number of players. shouldn't exceed 2

    //game startup flags
    private bool haveAvatars = false;

    private void Awake()
    {
        mainGame = this;
        stack = new List<IStackable>();
        triggerMap = new Dictionary<TriggerCondition, List<Trigger>>();
        foreach(TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            triggerMap.Add(c, new List<Trigger>());
        }
    }

    public void Init(MouseController mouseCtrl, UIController uiCtrl, CardRepository cardRepo)
    {
        this.mouseCtrl = mouseCtrl;
        this.uiCtrl = uiCtrl;
        CardRepo = cardRepo;
    }

    #region players and game starting
    public int AddPlayer(TcpClient tcpClient)
    {
        Debug.Log($"Adding player #{currPlayerCount}");
        if (currPlayerCount >= 2) return -1;

        Players[currPlayerCount].SetInfo(tcpClient, currPlayerCount);
        currPlayerCount++;

        //if at least two players, start the game startup process by getting avatars
        if (currPlayerCount >= 2) GetDecks();

        return currPlayerCount;
    }

    private void GetDeckFrom(ServerPlayer player)
    {
        player.ServerNotifier.GetDecklist();
    }

    public void GetDecks()
    {
        //ask the players for their avatars (and decks at the same time)
        foreach(ServerPlayer p in ServerPlayers)
        {
            GetDeckFrom(p);
        }

        //if avatars are returned, then set the pips to start with and start the game
        //that should happen in server network controller
    }

    //for future logic like limited cards, etc.
    private bool ValidDeck(List<string> deck)
    {
        if (deck.Count < 50) return false;
        //first name should be that of the Avatar
        if (CardRepo.GetCardFromName(deck[0]).cardType != 'C') return false;

        return true;
    }

    public void SetDeck(ServerPlayer player, string decklist)
    {
        string[] cards = decklist.Split('\n');
        List<string> deck = new List<string>();
        foreach(string name in cards)
        {
            if (CardNameIndices.ContainsKey(name)) deck.Add(name);
        }

        if (!ValidDeck(deck))
        {
            //request deck again from that player
            GetDeckFrom(player);
            return;
        }

        //otherwise, set the avatar and rest of the deck
        AvatarCard avatar = CardRepo.InstantiateAvatar(deck[0], AvatarPrefab);
        if(avatar == null)
        {
            GetDeckFrom(player);
            return;
        }

        foreach(string name in deck)
        {
            Card card = player.deckCtrl.AddCard(name, cardCount, player);
            cardCount++;
            player.ServerNotifier.NotifyAddToDeck(card);
        }

        lock (SetAvatarLock)
        {
            //if both players have decks now, then start the game
            foreach(Player p in Players)
            {
                if (p.Avatar = null) return;
            }

            StartGame();
        }
    }

    public void StartGame()
    {
        //set initial pips (based on avatars' S)
        Players[0].pips = Players[1].Avatar.S;
        Players[1].pips = Players[0].Avatar.S;

        //determine who goes first and tell the players
        int first = Random.value > 0.5f ? 0 : 1;
        ServerPlayers[first].ServerNotifier.YoureFirst();
        ServerPlayers[1 - first].ServerNotifier.YoureSecond();
    }
    #endregion

    #region move card between areas
    //so that notify stuff can be sent in the server
    private void Remove(Card card)
    {
        switch (card.Location)
        {
            case CardLocation.Field:
                boardCtrl.RemoveFromBoard(card);
                break;
            case CardLocation.Discard:
                card.Controller.discardCtrl.RemoveFromDiscard(card);
                break;
            case CardLocation.Hand:
                card.Controller.handCtrl.RemoveFromHand(card);
                break;
            case CardLocation.Deck:
                card.Controller.deckCtrl.RemoveFromDeck(card);
                break;
            default:
                Debug.LogError($"Tried to remove card from invalid location {card.Location}");
                break;
        }
    }

    public override void Discard(Card card)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyDiscard(card);
        base.Discard(card);
    }

    public override void Rehand(Player controller, Card card)
    {
        if(controller != card.Controller)
        {
            Debug.LogError($"Card {card.CardName} is being added to the hand of " +
                $"{controller.index} without the correct client being notified");
        }
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyRehand(card);
        base.Rehand(controller, card);
    }

    public override void Rehand(Card card)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyRehand(card);
        base.Rehand(card);
    }

    public override void Reshuffle(Card card)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyReshuffle(card);
        base.Reshuffle(card);
    }

    public override void Topdeck(Card card)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyTopdeck(card);
        base.Topdeck(card);
    }

    public override void Play(Card card, int toX, int toY, Player controller)
    {
        //note that it's serverPlayers[controller.index] because you can play to the field of someone whose card it isnt
        ServerPlayers[controller.index].ServerNotifier.NotifyPlay(card, toX, toY);
        base.Play(card, toX, toY, controller);
    }

    public override void MoveOnBoard(Card card, int toX, int toY)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyMove(card, toX, toY);
        base.MoveOnBoard(card, toX, toY);
    }
    #endregion move card between areas

    public Card Draw(int player = 0)
    {
        Card toDraw = Players[player].deckCtrl.PopTopdeck();
        Rehand(toDraw);
        return toDraw;
    }

    public void Lose(int playerIndex)
    {
        throw new System.NotImplementedException();
    }

    public void GivePlayerPips(Player player, int pipsToSet)
    {
        player.pips = pipsToSet;
        if (player.index == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        TurnServerPlayer.ServerNotifier.NotifySetPips(pipsToSet);
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
            Discard(toCheck);
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
        if(drawn != null) TurnServerPlayer.ServerNotifier.NotifyDraw(drawn);
        TurnServerPlayer.ServerNotifier.NotifySetTurn(this, turnPlayer);

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
            TurnServerPlayer.ServerNotifier.DiscardSimples();
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
        foreach(Player p in Players)
        {
            p.passedPriority = false;
        }
    }

    public void CheckForResponse()
    {
        //since a new thing is being put on the stack, mark both players as having not passed priority
        ResetPassingPriority();

        //check if responses exist. if not, resolve
        if (Players[turnPlayer].HoldsPriority())
        {
            //then send them a request to do something or pass priority
            //TODO: send the stack entry encoded somehow?
        }
        else if(Players[1 - turnPlayer].HoldsPriority()){
            //then mark the turn player as having passed priority
            Players[turnPlayer].passedPriority = true;

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
