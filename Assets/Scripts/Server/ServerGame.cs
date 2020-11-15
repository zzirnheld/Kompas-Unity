using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using KompasServer.Cards;
using KompasServer.Effects;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

namespace KompasServer.GameCore
{
    public class ServerGame : Game
    {
        public const int AvatarEBonus = 10;
        public const int AvatarWPenalty = 10;

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

        public ServerEffect CurrEffect { get; set; }
        public override IStackable CurrStackEntry => EffectsController.CurrStackEntry;
        public override bool NothingHappening => EffectsController.NothingHappening;

        public override int TurnCount 
        { 
            get => base.TurnCount;
            protected set
            {
                Leyload += value - TurnCount;
                base.TurnCount = value;
            }
        }

        public override int Leyload 
        { 
            get => base.Leyload;
            set
            {
                base.Leyload = value;
                ServerPlayers[0].ServerNotifier.NotifyLeyload(Leyload);
            }
        }

        public void Init(UIController uiCtrl, CardRepository cardRepo)
        {
            this.uiCtrl = uiCtrl;
            base.cardRepo = cardRepo;
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

        private void GetDeckFrom(ServerPlayer player) => player.ServerNotifier.GetDecklist();

        public void GetDecks()
        {
            //ask the players for their avatars (and decks at the same time)
            foreach (ServerPlayer p in ServerPlayers) GetDeckFrom(p);

            //if avatars are returned, then set the pips to start with and start the game
            //that should happen in server network controller
        }

        //TODO for future logic like limited cards, etc.
        private bool ValidDeck(List<string> deck)
        {
            if (uiCtrl.DebugMode) return true;
            if (deck.Count < 49) return false;
            //first name should be that of the Avatar
            if (cardRepo.GetCardFromName(deck[0]).cardType != 'C') return false;

            return true;
        }

        private List<string> SanitizeDeck(string decklist)
        {
            return decklist.Split('\n')
                .Where(c => !string.IsNullOrWhiteSpace(c) && CardRepository.CardExists(c))
                .ToList();
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
                avatar = cardRepo.InstantiateServerAvatar(deck[0], this, player, cardCount++);
                if (avatar == null)
                {
                    Debug.LogError($"Error in loading avatar for {decklist}");
                    GetDeckFrom(player);
                    return;
                }
            }

            //take out avatar before telling player to add the rest of the deck
            deck.RemoveAt(0);

            foreach (string name in deck)
            {
                ServerGameCard card;
                lock (AddCardsLock)
                {
                    card = cardRepo.InstantiateServerNonAvatar(name, this, player, cardCount);
                    cardsByID.Add(cardCount, card);
                    cardCount++;
                }
                player.deckCtrl.ShuffleIn(card);
                if (card != null) Debug.Log($"Adding new card {card.CardName} with id {card.ID}");
                player.ServerNotifier.NotifyAddToDeck(card);
            }

            Debug.Log($"Setting avatar for player {player.index}");
            player.Avatar = avatar;
            //avatar.Play(player.index * 6, player.index * 6, player);
            Debug.Log($"Avatar successfully played? {avatar.Play(player.index * 6, player.index * 6, player)}");
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
            Players[0].Pips = 0; //Players[1].Avatar.S / 2;
            Players[1].Pips = 0; //Players[0].Avatar.S / 2;

            //determine who goes first and tell the players
            FirstTurnPlayer = TurnPlayerIndex = Random.value > 0.5f ? 0 : 1;

            foreach (var p in ServerPlayers) 
            {
                p.ServerNotifier.SetFirstTurnPlayer(FirstTurnPlayer);
                p.Avatar.SetE(p.Avatar.E + AvatarEBonus);
                p.Avatar.SetW(p.Avatar.W - AvatarWPenalty);
                DrawX(p.index, 5);
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
                var toDraw = controller.deckCtrl.Topdeck;
                if (toDraw == null) break;
                var eachDrawContext = new ActivationContext(card: toDraw, stackable: stackSrc, triggerer: controller);
                EffectsController.TriggerForCondition(Trigger.EachDraw, eachDrawContext);
                toDraw.Rehand(controller, stackSrc);
                drawn.Add(toDraw);
            }
            var context = new ActivationContext(stackable: stackSrc, triggerer: controller, x: i);
            EffectsController.TriggerForCondition(Trigger.DrawX, context);
            return drawn;
        }
        public GameCard Draw(int player, IStackable stackSrc = null)
        {
            var drawn = DrawX(player, 1, stackSrc);
            return drawn.Count > 0 ? drawn[0] : null;
        }

        public void GivePlayerPips(Player player, int pipsToSet) => player.Pips = pipsToSet;

        public void GiveTurnPlayerPips() => GivePlayerPips(TurnPlayer, TurnPlayer.Pips + Leyload);

        public void Attack(GameCard attacker, GameCard defender, ServerPlayer instigator, bool playerInitiated = false)
        {
            Debug.Log($"{attacker.CardName} attacking {defender.CardName} at {defender.BoardX}, {defender.BoardY}");
            //push the attack to the stack, then check if any player wants to respond before resolving it
            var attack = new ServerAttack(this, instigator, attacker, defender);
            EffectsController.PushToStack(attack, new ActivationContext());
            //check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
            attack.Declare();
            if (playerInitiated) attacker.SetAttacksThisTurn(attacker.attacksThisTurn + 1);
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

            Debug.Log($"Checking validity of playing {card.CardName} to {toX}, {toY}");
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

            Debug.Log($"Checking validity augment of {card.CardName} to {toX}, {toY}, on {boardCtrl.GetCardAt(toX, toY)}");
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

            Debug.Log($"Checking validity of moving {toMove.CardName} to {toX}, {toY}");
            if (toMove.Position == (toX, toY) || (toMove.IsAvatar && !toMove.Summoned)
                || EffectsController.CurrStackEntry != null) return false;
            return toMove.MovementRestriction.EvaluateNormalMove(toX, toY);
        }

        public bool ValidAttack(GameCard attacker, GameCard defender, ServerPlayer instigator)
        {
            if (uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode, always return true for valid attack");
                return attacker != null && defender != null;
            }

            Debug.Log($"Checking validity of attack of {attacker.CardName} on {defender} by {instigator.index}");
            return attacker.AttackRestriction.Evaluate(defender);
        }
        #endregion

        public void SwitchTurn()
        {
            TurnPlayerIndex = 1 - TurnPlayerIndex;
            if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
            TurnCount++;
            GiveTurnPlayerPips();

            ResetCardsForTurn();

            //draw for turn and store what was drawn
            Draw(TurnPlayerIndex);
            TurnServerPlayer.ServerNotifier.NotifySetTurn(this, TurnPlayerIndex);

            //do hand size
            EffectsController.PushToStack(new ServerHandSizeStackable(this, EffectsController, TurnServerPlayer), default);

            //trigger turn start effects
            var context = new ActivationContext(triggerer: TurnServerPlayer);
            EffectsController.TriggerForCondition(Trigger.TurnStart, context);

            EffectsController.CheckForResponse();
        }

        public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

        public ServerPlayer ServerControllerOf(GameCard card) => ServerPlayers[card.ControllerIndex];
    }
}