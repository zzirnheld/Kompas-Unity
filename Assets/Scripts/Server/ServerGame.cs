using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class ServerGame : Game {

    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public readonly object AddCardsLock = new object();
    public readonly object CheckAvatarsLock = new object();

    public ServerEffectsController EffectsController;

    public override Player[] Players => ServerPlayers;
    public ServerPlayer[] ServerPlayers;
    public ServerPlayer TurnServerPlayer => ServerPlayers[TurnPlayerIndex];
    public int cardCount = 0;
    private int currPlayerCount = 0; //current number of players. shouldn't exceed 2

    public void Init(UIController uiCtrl, CardRepository cardRepo)
    {
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
        if (uiCtrl.DebugMode) return true;
        if (deck.Count < 50) return false;
        //first name should be that of the Avatar
        if (CardRepo.GetCardFromName(deck[0]).cardType != 'C') return false;

        return true;
    }

    private List<string> SanitizeDeck(string decklist)
    {
        string[] cards = decklist.Split('\n');
        List<string> deck = new List<string>();
        foreach (string name in cards)
        {
            if (string.IsNullOrWhiteSpace(name)) continue;
            if (CardNameIndices.ContainsKey(name)) deck.Add(name);
        }

        return deck;
    }

    public void SetDeck(ServerPlayer player, string decklist)
    {
        List<string> deck = SanitizeDeck(decklist);

        if (!ValidDeck(deck))
        {
            Debug.LogError($"INVALID DECK {decklist}");
            //request deck again from that player
            GetDeckFrom(player);
            return;
        }

        AvatarCard avatar;
        lock (AddCardsLock)
        {
            //otherwise, set the avatar and rest of the deck
            avatar = CardRepo.InstantiateServerAvatar(deck[0], this, player, cardCount++);
            if (avatar == null)
            {
                Debug.LogError($"Error in loading avatar for {decklist}");
                GetDeckFrom(player);
                return;
            }
        }

        //take out avatar before telling player to add the rest of the deck
        deck.RemoveAt(0);

        foreach(string name in deck)
        {
            Card card;
            lock (AddCardsLock)
            {
                card = CardRepo.InstantiateServerNonAvatar(name, this, player, cardCount);
                cardsByID.Add(cardCount, card);
                cardCount++;
            }
            player.deckCtrl.ShuffleIn(card);
            if (card != null) Debug.Log($"Adding new card {card.CardName} with id {card.ID}");
            player.ServerNotifier.NotifyAddToDeck(card);
        }

        Debug.Log($"Setting avatar for player {player.index}");
        player.Avatar = avatar;
        player.ServerNotifier.NotifyAddToDeck(avatar);
        Play(avatar, player.index * 6, player.index * 6, player);
        //if both players have decks now, then start the game
        lock (CheckAvatarsLock)
        {
            foreach (Player p in Players)
            {
                if (p.Avatar == null) return;
            }
        }
        StartGame();
    }

    public void StartGame()
    {
        //set initial pips (based on avatars' S)
        Debug.Log($"Starting game. Player 0 avatar is null? {Players[0].Avatar == null}. Player 1 is null? {Players[1].Avatar == null}.");
        Players[0].pips = Players[1].Avatar.S;
        Players[1].pips = Players[0].Avatar.S;

        //determine who goes first and tell the players
        TurnPlayerIndex = Random.value > 0.5f ? 0 : 1;
        ServerPlayers[TurnPlayerIndex].ServerNotifier.YoureFirst();
        ServerPlayers[1 - TurnPlayerIndex].ServerNotifier.YoureSecond();
        ServerPlayers[0].ServerNotifier.NotifySetPips(ServerPlayers[0].pips);
        ServerPlayers[1].ServerNotifier.NotifySetPips(ServerPlayers[1].pips);

        foreach(var player in ServerPlayers)
        {
            DrawX(player.index, 5);
        }
        GiveTurnPlayerPips();
    }
    #endregion

    #region move card between areas
    //so that notify stuff can be sent in the server
    public void Discard(Card card, IServerStackable stackSrc)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyDiscard(card);
        EffectsController.Trigger(TriggerCondition.Discard, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        base.Discard(card);

        //if we just discarded an augment, note that, and trigger de-augment
        if (card.CardType == 'A')
            EffectsController.Trigger(TriggerCondition.AugmentDetached, 
                cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
    }

    public override void Discard(Card card) => Discard(card, null);

    public void Rehand(Player controller, Card card, IServerStackable stackSrc)
    {
        if(controller != card.Controller)
        {
            Debug.LogError($"Card {card.CardName} is being added to the hand of " +
                $"{controller.index} without the correct client being notified." +
                $"This wasn't a problem up until now, but now you need to implement owner index," +
                $"ya numpty");
        }
        EffectsController.Trigger(TriggerCondition.Rehand, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyRehand(card);
        base.Rehand(controller, card);
    }

    public override void Rehand(Player controller, Card card) => Rehand(controller, card, null);

    public void Rehand(Card card, IServerStackable stackSrc = null)
    {
        Rehand(card.Controller, card, stackSrc);
    }

    public override void Rehand(Card card) => Rehand(card, null);

    public void Reshuffle(Card card, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Reshuffle, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyReshuffle(card);
        base.Reshuffle(card);
    }

    public override void Reshuffle(Card card) => Reshuffle(card, null);

    public void Topdeck(Card card, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Topdeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyTopdeck(card);
        base.Topdeck(card);
    }

    public override void Topdeck(Card card) => Topdeck(card, null);

    public void Bottomdeck(Card card, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Bottomdeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyBottomdeck(card);
        base.Bottomdeck(card);
    }

    public override void Bottomdeck(Card card) => Bottomdeck(card, null);

    public void Play(Card card, int toX, int toY, Player controller, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Play, 
            cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, space: (toX, toY));
        //note that it's serverPlayers[controller.index] because you can play to the field of someone whose card it isnt
        ServerPlayers[controller.index].ServerNotifier.NotifyPlay(card, toX, toY);
        base.Play(card, toX, toY, controller);

        //if we just played an augment, note that, and trigger augment
        if (card.CardType == 'A') 
            EffectsController.Trigger(TriggerCondition.AugmentAttached, 
                cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);

        //then, once everything was done, if this was a player-initiated action, check for responses
        if (stackSrc == null) EffectsController.CheckForResponse();
    }

    public override void Play(Card card, int toX, int toY, Player controller) => Play(card, toX, toY, controller, null);

    public void MoveOnBoard(Card card, int toX, int toY, bool normalMove, IServerStackable stackSrc)
    {
        EffectsController.Trigger(TriggerCondition.Move, 
            cardTriggerer: card, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController, space: (toX, toY));
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyMove(card, toX, toY, normalMove);
        base.MoveOnBoard(card, toX, toY, normalMove);
        if (stackSrc == null) EffectsController.CheckForResponse();
    }

    public override void MoveOnBoard(Card card, int toX, int toY, bool normalMove) => MoveOnBoard(card, toX, toY, normalMove, null);

    public void Dispel(SpellCard spellCard, IServerStackable stackSrc)
    {
        SetNegated(spellCard, true, stackSrc);
        Discard(spellCard, stackSrc);
    }
    #endregion move card between areas

    public override void SetStats(SpellCard spellCard, int c)
    {
        ServerPlayers[spellCard.ControllerIndex].ServerNotifier.NotifySetSpellStats(spellCard, c);
        base.SetStats(spellCard, c);
    }

    public override void SetStats(CharacterCard charCard, int n, int e, int s, int w)
    {
        Debug.Log($"Setting stats of {charCard.CardName} to {n}/{e}/{s}/{w}");
        ServerPlayers[charCard.ControllerIndex].ServerNotifier.NotifySetNESW(charCard, n, e, s, w);
        base.SetStats(charCard, n, e, s, w);
        if (charCard.E <= 0)
        {
            Discard(charCard);
            //don't call check for response on stack because anything that causes things to die,
            //attacks or effects, will call check for response once it's done resolving.
        }
    }

    public void SetNegated(Card c, bool negated, IServerStackable stackSrc)
    {
        ServerPlayers[c.ControllerIndex].ServerNotifier.NotifySetNegated(c, negated);
        base.SetNegated(c, negated);
    }

    public override void SetNegated(Card c, bool negated) => SetNegated(c, negated, null);

    public void SetActivated(Card c, bool activated, IServerStackable stackSrc)
    {
        ServerPlayers[c.ControllerIndex].ServerNotifier.NotifyActivate(c, activated);
        base.SetActivated(c, activated);
        //If this is the first activation, trigger "activate"
        if (c.Activations == 1) 
            EffectsController.Trigger(TriggerCondition.Activate,
                cardTriggerer: c, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
        //If this is the last deactivation, trigger "deactivate"
        else if (c.Activations == 0) 
            EffectsController.Trigger(TriggerCondition.Deactivate,
                cardTriggerer: c, stackTrigger: stackSrc, triggerer: stackSrc?.ServerController);
    }

    public override void SetActivated(Card c, bool activated) => SetActivated(c, activated, null);

    public void ChangeControl(Card c, Player controller, IServerStackable stackSrc)
    {
        ServerPlayers[c.ControllerIndex].ServerNotifier.NotifyChangeController(c, controller);
        base.ChangeControl(c, controller);
        //TODO triggers?
    }

    public override void ChangeControl(Card c, Player controller) => ChangeControl(c, controller, null);

    public Card Draw(int player, IServerStackable stackSrc = null)
    {
        var drawn = DrawX(player, 1, stackSrc);
        return drawn.Count > 0 ? drawn[0] : null;
    }

    public List<Card> DrawX(int player, int x, IServerStackable stackSrc = null)
    {
        List<Card> drawn = new List<Card>();
        int i;
        for (i = 0; i < x; i++)
        {
            Card toDraw = Players[player].deckCtrl.PopTopdeck();
            if (toDraw == null) break;
            EffectsController.Trigger(TriggerCondition.EachDraw, cardTriggerer: toDraw, stackTrigger: stackSrc, triggerer: ServerPlayers[player]);
            Rehand(toDraw, stackSrc);
            drawn.Add(toDraw);
        }
        EffectsController.Trigger(TriggerCondition.DrawX, stackTrigger: stackSrc, triggerer: ServerPlayers[player], x: i);
        return drawn;
    }

    public void GivePlayerPips(ServerPlayer player, int pipsToSet)
    {
        player.pips = pipsToSet;
        if (player.index == 0) uiCtrl.UpdateFriendlyPips(pipsToSet);
        else uiCtrl.UpdateEnemyPips(pipsToSet);
        player.ServerNotifier.NotifySetPips(pipsToSet);
    }

    public void GiveTurnPlayerPips()
    {
        int pipsToSet = TurnPlayer.pips + MaxCardsOnField;
        GivePlayerPips(TurnServerPlayer, pipsToSet);
    }

    public void Attack(CharacterCard attacker, CharacterCard defender, ServerPlayer instigator)
    {
        Debug.Log($"ServerNetworkController {attacker.CardName} attacking {defender.CardName} at {defender.BoardX}, {defender.BoardY}");
        //push the attack to the stack, then check if any player wants to respond before resolving it
        var attack = new ServerAttack(this, instigator, attacker, defender);
        EffectsController.PushToStack(attack);
        //check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
        attack.Declare();
        EffectsController.CheckForResponse();
    }

    #region check validity
    public bool ValidBoardPlay(Card card, int toX, int toY, ServerPlayer player)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid play");
            return true;
        }

        Debug.Log("Trying to play " + card.CardName + " to " + toX + ", " + toY);
        return card != null
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCardAt(toX, toY) == null
            && card.PlayRestriction.EvaluateNormalPlay(toX, toY, player);
    }

    public bool ValidAugment(Card card, int toX, int toY, ServerPlayer player)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid augment");
            return true;
        }

        return card != null
            && card is AugmentCard
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCardAt(toX, toY) != null
            && card.PlayRestriction.EvaluateNormalPlay(toX, toY, player);
    }

    public bool ValidMove(Card toMove, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid move");
            return true;
        }

        return toMove.MovementRestriction.Evaluate(toX, toY);
    }

    public bool ValidAttack(CharacterCard attacker, CharacterCard defender, ServerPlayer instigator)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid attack");
            return attacker != null && defender != null;
        }

        return attacker.AttackRestriction.Evaluate(defender);
    }
    #endregion
    
    public void SwitchTurn()
    {
        TurnPlayerIndex = 1 - TurnPlayerIndex;
        GiveTurnPlayerPips();
        
        boardCtrl.ResetCardsForTurn(TurnPlayer);

        //draw for turn and store what was drawn
        Card drawn = Draw(TurnPlayerIndex);
        if(drawn != null) TurnServerPlayer.ServerNotifier.NotifyDraw(drawn);
        TurnServerPlayer.ServerNotifier.NotifySetTurn(this, TurnPlayerIndex);

        //trigger turn start effects
        EffectsController.Trigger(TriggerCondition.TurnStart, triggerer: TurnServerPlayer);
        EffectsController.CheckForResponse();
    }
}
