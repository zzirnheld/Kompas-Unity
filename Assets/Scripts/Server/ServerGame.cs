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
    public ServerPlayer TurnServerPlayer { get { return ServerPlayers[turnPlayer]; } }
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
                card = CardRepo.InstantiateServerNonAvatar(name, this, player);
                cards.Add(cardCount, card);
                card.ID = cardCount;
                cardCount++;
            }
            player.deckCtrl.AddCard(card);
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

    public void Discard(Card card, IServerStackable stackSrc = null)
    {
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyDiscard(card);
        EffectsController.Trigger(TriggerCondition.Discard, card, stackSrc, null, stackSrc?.ServerController);
        base.Discard(card, stackSrc);
    }

    public void Rehand(Player controller, Card card, IServerStackable stackSrc = null)
    {
        if(controller != card.Controller)
        {
            Debug.LogError($"Card {card.CardName} is being added to the hand of " +
                $"{controller.index} without the correct client being notified." +
                $"This wasn't a problem up until now, but now you need to implement owner index," +
                $"ya numpty");
        }
        EffectsController.Trigger(TriggerCondition.Rehand, card, stackSrc, null, stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyRehand(card);
        base.Rehand(controller, card, stackSrc);
    }

    public void Rehand(Card card, IServerStackable stackSrc = null)
    {
        Rehand(card.Controller, card, stackSrc);
    }

    public void Reshuffle(Card card, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Reshuffle, card, stackSrc, null, stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, card, stackSrc, null, stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyReshuffle(card);
        base.Reshuffle(card, stackSrc);
    }

    public void Topdeck(Card card, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Topdeck, card, stackSrc, null, stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, card, stackSrc, null, stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyTopdeck(card);
        base.Topdeck(card, stackSrc);
    }

    public void Bottomdeck(Card card, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Bottomdeck, card, stackSrc, null, stackSrc?.ServerController);
        EffectsController.Trigger(TriggerCondition.ToDeck, card, stackSrc, null, stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyBottomdeck(card);
        base.Bottomdeck(card, stackSrc);
    }

    public void Play(Card card, int toX, int toY, Player controller, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Play, card, stackSrc, null, stackSrc?.ServerController);
        //note that it's serverPlayers[controller.index] because you can play to the field of someone whose card it isnt
        ServerPlayers[controller.index].ServerNotifier.NotifyPlay(card, toX, toY);
        base.Play(card, toX, toY, controller, stackSrc);
        if (stackSrc == null) EffectsController.CheckForResponse();
    }

    public void MoveOnBoard(Card card, int toX, int toY, bool normalMove, IServerStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Move, card, stackSrc, null, stackSrc?.ServerController);
        ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyMove(card, toX, toY, normalMove);
        base.MoveOnBoard(card, toX, toY, normalMove, stackSrc);
        if (stackSrc == null) EffectsController.CheckForResponse();
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

    public void Negate(Card c, IServerStackable stackSrc = null)
    {
        ServerPlayers[c.ControllerIndex].ServerNotifier.NotifyNegate(c);
        base.Negate(c, stackSrc);
    }

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
            EffectsController.Trigger(TriggerCondition.EachDraw, toDraw, stackSrc, null, ServerPlayers[player]);
            Rehand(toDraw, stackSrc);
            drawn.Add(toDraw);
        }
        EffectsController.Trigger(TriggerCondition.DrawX, null, stackSrc, i, ServerPlayers[player]);
        return drawn;
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

    public void Attack(CharacterCard attacker, CharacterCard defender, ServerPlayer instigator)
    {
        Debug.Log($"ServerNetworkController {attacker.CardName} attacking {defender.CardName} at {defender.BoardX}, {defender.BoardY}");
        //push the attack to the stack, then check if any player wants to respond before resolving it
        EffectsController.PushToStack(new ServerAttack(this, instigator, attacker, defender));
        EffectsController.CheckForResponse();
    }

    //TODO improve this with checking if the square is valid (adj or special case)
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
        return toMove.DistanceTo(toX, toY) + charToMove.SpacesMoved <= charToMove.N
            && (boardCtrl.GetCardAt(toX, toY) == null || boardCtrl.GetCardAt(toX, toY).ControllerIndex == toMove.ControllerIndex);
    }

    public bool ValidAttack(CharacterCard attacker, CharacterCard defender, ServerPlayer instigator)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid attack");
            return attacker != null && defender != null;
        }

        if (attacker == null || defender == null) return false;
        return attacker.DistanceTo(defender) == 1;
    }
    #endregion
    
    public void SwitchTurn()
    {
        turnPlayer = 1 - turnPlayer;
        GiveTurnPlayerPips();
        
        boardCtrl.ResetCardsForTurn();

        //draw for turn and store what was drawn
        Card drawn = Draw(turnPlayer);
        if(drawn != null) TurnServerPlayer.ServerNotifier.NotifyDraw(drawn);
        TurnServerPlayer.ServerNotifier.NotifySetTurn(this, turnPlayer);

        //trigger turn start effects
        EffectsController.Trigger(TriggerCondition.TurnStart, null, null, null, TurnServerPlayer);
        EffectsController.CheckForResponse();
    }
}
