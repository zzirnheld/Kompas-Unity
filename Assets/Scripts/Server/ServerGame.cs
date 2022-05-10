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
using System.Threading.Tasks;

namespace KompasServer.GameCore
{
    public class ServerGame : Game
    {
        public const int MinDeckSize = 49;

        public const int AvatarNPenalty = 15;
        public const int AvatarEBonus = 15;
        public const int AvatarWPenalty = 15;
        public const int AvatarShield = 15;

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

        public bool GameHasStarted { get; private set; } = false;

        public ServerEffect CurrEffect { get; set; }
        public override IStackable CurrStackEntry => EffectsController.CurrStackEntry;
        public override IEnumerable<IStackable> StackEntries => EffectsController.StackEntries;
        public override bool NothingHappening => EffectsController.NothingHappening && Players.All(s => s.PassedPriority);

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
            if (currPlayerCount >= 2)
            {
                foreach (ServerPlayer p in ServerPlayers) GetDeckFrom(p);
            }

            return currPlayerCount;
        }

        private void GetDeckFrom(ServerPlayer player) => player.ServerNotifier.GetDecklist();

        //TODO for future logic like limited cards, etc.
        private bool ValidDeck(List<string> deck)
        {
            //first name should be that of the Avatar
            if (!cardRepo.CardNameIsCharacter(deck[0]))
            {
                Debug.LogError($"{deck[0]} isn't a character, so it can't be the Avatar");
                return false;
            }
            if (uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode enabled, always accepting a decklist");
                return true;
            }
            if (deck.Count < MinDeckSize)
            {
                Debug.LogError($"Deck {deck} too small");
                return false;
            }

            return true;
        }

        private List<string> SanitizeDeck(string decklist)
        {
            return decklist.Split('\n')
                .Where(c => !string.IsNullOrWhiteSpace(c) && CardRepository.CardExists(c))
                .ToList();
        }

        public async Task SetDeck(ServerPlayer player, string decklist)
        {
            List<string> deck = SanitizeDeck(decklist);

            if (ValidDeck(deck)) player.ServerNotifier.DeckAccepted();
            else
            {
                GetDeckFrom(player);
                return;
            }

            AvatarServerGameCard avatar;
            lock (AddCardsLock)
            {
                //otherwise, set the avatar and rest of the deck
                avatar = cardRepo.InstantiateServerAvatar(deck[0], this, player, cardCount++) ??
                    throw new System.ArgumentException($"Failed to load avatar for card {deck[0]}");
                deck.RemoveAt(0); //take out avatar before telling player to add the rest of the deck
            }

            foreach (string name in deck)
            {
                ServerGameCard card;
                lock (AddCardsLock)
                {
                    card = cardRepo.InstantiateServerNonAvatar(name, this, player, cardCount);
                    if (card == null) continue;
                    cardsByID.Add(cardCount, card);
                    cardCount++;
                }
                Debug.Log($"Adding new card {card.CardName} with id {card.ID}");
                player.deckCtrl.ShuffleIn(card);
                player.ServerNotifier.NotifyCreateCard(card, wasKnown: false);
            }

            player.Avatar = avatar;
            avatar.Play(player.AvatarCorner, player, new GameStartStackable());
            lock (CheckAvatarsLock)
            {
                if (Players.Any(player => player.Avatar == null)) return;
            }
            await StartGame();
        }

        public async Task StartGame()
        {
            //set initial pips to 0
            Debug.Log($"Starting game. Player 0 avatar is null? {Players[0].Avatar == null}. Player 1 is null? {Players[1].Avatar == null}.");
            Players[0].Pips = 0;
            Players[1].Pips = 0;

            //determine who goes first and tell the players
            FirstTurnPlayer = Random.value > 0.5f ? 0 : 1;
            TurnPlayerIndex = FirstTurnPlayer;

            foreach (var p in ServerPlayers)
            {
                p.ServerNotifier.SetFirstTurnPlayer(FirstTurnPlayer);
                p.Avatar.SetN(0, stackSrc: null);
                p.Avatar.SetE(p.Avatar.E + AvatarEBonus, stackSrc: null);
                p.Avatar.SetW(0, stackSrc: null);
                DrawX(p.index, 5, stackSrc: null);
            }

            GameHasStarted = true;

            await TurnStartOperations(notFirstTurn: false);
        }
        #endregion

        #region turn
        public async Task TurnStartOperations(bool notFirstTurn = true)
        {
            if (notFirstTurn)
            {
                if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
                TurnCount++;
            }

            TurnServerPlayer.ServerNotifier.NotifyYourTurn();
            ResetCardsForTurn();

            TurnPlayer.Pips += Leyload;
            if (notFirstTurn) Draw(TurnPlayerIndex);

            //do hand size
            EffectsController.PushToStack(new ServerHandSizeStackable(this, TurnServerPlayer), default);

            //trigger turn start effects
            var context = new ActivationContext(game: this, player: TurnServerPlayer);
            EffectsController.TriggerForCondition(Trigger.TurnStart, context);

            await EffectsController.CheckForResponse();
        }


        public async Task SwitchTurn()
        {
            TurnPlayerIndex = 1 - TurnPlayerIndex;
            Debug.Log($"Turn swapping to the turn of index {TurnPlayerIndex}");

            await TurnStartOperations();
        }
        #endregion turn

        public List<GameCard> DrawX(int player, int x, IStackable stackSrc = null)
        {
            List<GameCard> drawn = new List<GameCard>();
            Player controller = Players[player];
            int cardsDrawn;
            for (cardsDrawn = 0; cardsDrawn < x; cardsDrawn++)
            {
                var toDraw = controller.deckCtrl.Topdeck;
                if (toDraw == null) break;
                var eachDrawContext = new ActivationContext(game: this, mainCardBefore: toDraw, stackableCause: stackSrc, player: controller);
                toDraw.Rehand(controller, stackSrc);
                eachDrawContext.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.EachDraw, eachDrawContext);
                drawn.Add(toDraw);
            }
            var context = new ActivationContext(game: this, stackableCause: stackSrc, player: controller, x: cardsDrawn);
            EffectsController.TriggerForCondition(Trigger.DrawX, context);
            return drawn;
        }
        public GameCard Draw(int player, IStackable stackSrc = null)
            => DrawX(player, 1, stackSrc).FirstOrDefault();

        /// <param name="manual">Whether a player instigated the attack without an effect.</param>
        /// <returns>The Attack object created by starting this attack</returns>
        public ServerAttack Attack(GameCard attacker, GameCard defender, ServerPlayer instigator, IStackable stackSrc, bool manual = false)
        {
            Debug.Log($"{attacker.CardName} attacking {defender.CardName} at {defender.Position}");
            //push the attack to the stack, then check if any player wants to respond before resolving it
            var attack = new ServerAttack(this, instigator, attacker, defender);
            EffectsController.PushToStack(attack, new ActivationContext(game: this, stackableCause: stackSrc, stackableEvent: attack, player: instigator));
            //check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
            attack.Declare(stackSrc);
            if (manual) attacker.SetAttacksThisTurn(attacker.attacksThisTurn + 1);
            return attack;
        }

        public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

        public ServerPlayer ServerControllerOf(GameCard card) => ServerPlayers[card.ControllerIndex];

        public void DumpGameInfo()
        {
            Debug.Log("BEGIN GAME INFO DUMP");
            Debug.Log("Cards:");
            foreach (var c in Cards) Debug.Log(c.ToString());

            Debug.Log($"Cards on board:\n{boardCtrl.ToString()}");

            Debug.Log(EffectsController.ToString());
        }
    }
}