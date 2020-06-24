using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class ServerGame : Game {

    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"
    
    public Dictionary<int, ServerGameCard> cardsByID = new Dictionary<int, ServerGameCard>();
    public IEnumerable<ServerGameCard> ServerCards => cardsByID.Values;
    public override IEnumerable<GameCard> Cards => ServerCards;

    public readonly object AddCardsLock = new object();
    public readonly object CheckAvatarsLock = new object();

    public ServerEffectsController EffectsController;

    public override Player[] Players => ServerPlayers;
    public ServerPlayer[] ServerPlayers;
    public ServerPlayer TurnServerPlayer => ServerPlayers[TurnPlayerIndex];
    public int cardCount = 0;
    private int currPlayerCount = 0; //current number of players. shouldn't exceed 2

    public override int Leyload
    {
        get => base.Leyload;
        set
        {
            base.Leyload = value;
            ServerPlayers[0].ServerNotifier.NotifySetLeyload(Leyload);
        }
    }

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
        else player.ServerNotifier.DeckAccepted();

        AvatarServerGameCard avatar;
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
            ServerGameCard card;
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
        avatar.Play(player.index * 6, player.index * 6, player, null);
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

    public List<GameCard> DrawX(int player, int x, IStackable stackSrc = null)
    {
        List<GameCard> drawn = new List<GameCard>();
        Player controller = Players[player];
        int i;
        for (i = 0; i < x; i++)
        {
            var toDraw = controller.deckCtrl.PopTopdeck();
            if (toDraw == null) break;
            EffectsController.Trigger(TriggerCondition.EachDraw, cardTriggerer: toDraw, stackTrigger: stackSrc, triggerer: ServerPlayers[player]);
            toDraw.Rehand(controller, stackSrc);
            drawn.Add(toDraw);
        }
        EffectsController.Trigger(TriggerCondition.DrawX, stackTrigger: stackSrc, triggerer: ServerPlayers[player], x: i);
        return drawn;
    }
    public GameCard Draw(int player, IStackable stackSrc = null)
    {
        var drawn = DrawX(player, 1, stackSrc);
        return drawn.Count > 0 ? drawn[0] : null;
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
        int pipsToSet = TurnPlayer.pips + Leyload;
        GivePlayerPips(TurnServerPlayer, pipsToSet);
    }

    public void Attack(GameCard attacker, GameCard defender, ServerPlayer instigator)
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
    public bool ValidBoardPlay(GameCard card, int toX, int toY, ServerPlayer player)
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

    public bool ValidAugment(GameCard card, int toX, int toY, ServerPlayer player)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid augment");
            return true;
        }

        return card != null
            && card.CardType == 'A'
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCardAt(toX, toY) != null
            && card.PlayRestriction.EvaluateNormalPlay(toX, toY, player);
    }

    public bool ValidMove(GameCard toMove, int toX, int toY)
    {
        if (uiCtrl.DebugMode)
        {
            Debug.LogWarning("Debug mode, always return true for valid move");
            return true;
        }

        return toMove.MovementRestriction.Evaluate(toX, toY);
    }

    public bool ValidAttack(GameCard attacker, GameCard defender, ServerPlayer instigator)
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
        GameCard drawn = Draw(TurnPlayerIndex);
        if(drawn != null) TurnServerPlayer.ServerNotifier.NotifyDraw(drawn);
        TurnServerPlayer.ServerNotifier.NotifySetTurn(this, TurnPlayerIndex);

        //trigger turn start effects
        EffectsController.Trigger(TriggerCondition.TurnStart, triggerer: TurnServerPlayer);
        EffectsController.CheckForResponse();
    }

    public override GameCard GetCardWithID(int id)
    {
        return cardsByID.ContainsKey(id) ? cardsByID[id] : null;
    }
}
