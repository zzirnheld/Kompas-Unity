using KompasClient.Cards;
using KompasClient.Effects;
using KompasClient.Networking;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasCore.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientGame : Game
    {
        public ClientCardRepository cardRepo;
        public override CardRepository CardRepository => cardRepo;

        [Header("Prefabs")]
        public GameObject AvatarPrefab;

        [Header("Networking MonoBehaviours")]
        public ClientNetworkController clientNetworkCtrl;
        public ClientNotifier clientNotifier;

        [Header("Game location controllers")]
        public ClientBoardController clientBoardController;
        public override BoardController BoardController => clientBoardController;

        [Header("Effects")]
        public ClientEffectsController clientEffectsCtrl;

        [Header("Players")]
        public ClientPlayer[] clientPlayers;
        public override Player[] Players => clientPlayers;
        public ClientPlayer FriendlyPlayer => clientPlayers[0];

        [Header("UI")]
        public ClientUIController clientUIController;
        public override UIController UIController => clientUIController;


        private readonly Dictionary<int, ClientGameCard> cardsByID = new Dictionary<int, ClientGameCard>();
        public override IEnumerable<GameCard> Cards => cardsByID.Values;

        public ClientSettings ClientSettings => clientUIController.clientUISettingsController.ClientSettings;

        //turn players?
        public bool FriendlyTurn => TurnPlayer == FriendlyPlayer;

        //search
        public ClientSearchController searchCtrl;

        //targeting
        public int targetsWanted;
        private GameCard[] currentPotentialTargets;
        public GameCard[] CurrentPotentialTargets
        {
            get => currentPotentialTargets;
            private set
            {
                currentPotentialTargets = value;
                ShowValidCardTargets();
            }
        }

        private (int, int)[] currentPotentialSpaces;
        public (int, int)[] CurrentPotentialSpaces
        {
            get => currentPotentialSpaces;
            set
            {
                currentPotentialSpaces = value;
                if (value != null) clientUIController.boardUIController.ShowSpaceTargets(space => value.Contains(space));
                else clientUIController.boardUIController.ShowSpaceTargets(_ => false);
            }
        }

        public override bool NothingHappening => !clientEffectsCtrl.StackEntries.Any();
        public override IEnumerable<IStackable> StackEntries => clientEffectsCtrl.StackEntries;

        public bool canZoom = false;

        //dirty card set
        private readonly HashSet<GameCard> dirtyCardList = new HashSet<GameCard>();

        public override int Leyload
        {
            get => base.Leyload;
            set
            {
                base.Leyload = value;
                clientUIController.Leyload = Leyload;
                //Refresh next turn pips shown.
                foreach (var player in Players)
                {
                    player.Pips = player.Pips;
                }
            }
        }

        private void Awake()
        {
            clientUIController.clientUISettingsController.LoadSettings();
            ApplySettings();
        }

        public void AddCard(ClientGameCard card)
        {
            if (card.ID != -1) cardsByID.Add(card.ID, card);
        }

        public void MarkCardDirty(GameCard card) => dirtyCardList.Add(card);

        public void PutCardsBack()
        {
            foreach (var c in dirtyCardList) c.CardController.PutBack();
            dirtyCardList.Clear();
        }

        public void SetAvatar(int player, string json, int avatarID)
        {
            if (player >= 2) throw new System.ArgumentException();

            var owner = clientPlayers[player];
            var avatar = cardRepo.InstantiateClientAvatar(json, owner, avatarID);
            avatar.KnownToEnemy = true;
            owner.Avatar = avatar;
            Space to = player == 0 ? Space.NearCorner : Space.FarCorner;
            avatar.Play(to, owner);
        }

        public void Delete(GameCard card)
        {
            card.Remove();
            cardsByID.Remove(card.ID);
            Destroy(card.CardController.gameObject);
        }

        //requesting
        public void SetFirstTurnPlayer(int playerIndex)
        {
            FirstTurnPlayer = TurnPlayerIndex = playerIndex;
            clientUIController.ChangeTurn(playerIndex);
            clientUIController.HideGetDecklistUI();
            RoundCount = 1;
            TurnCount = 1;
            canZoom = true;
            //force updating of pips to show correct messages.
            //there's probably a better way to do this.
            foreach (var player in Players) player.Pips = player.Pips;
        }

        public void SetTurn(int index)
        {
            TurnPlayerIndex = index;
            ResetCardsForTurn();
            clientUIController.ChangeTurn(TurnPlayerIndex);
            if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
            TurnCount++;
            clientUIController.FriendlyPips = Players[0].Pips;
            clientUIController.EnemyPips = Players[1].Pips;
        }

        public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

        public void ShowCardsByZoom(bool zoomed)
        {
            //TODO make this better with a dirty list
            foreach (var c in Cards.Where(c => c != null && c.CardController.gameObject.activeSelf))
            {
                c.CardController.gameCardViewController.Refresh();
            }
        }

        /// <summary>
        /// Makes cards show again, in case information changed after the packet.
        /// </summary>
        public void Refresh()
        {
            ShowCardsByZoom(ClientCameraController.Main.Zoomed);
            clientUIController.cardInfoViewUIController.Refresh();
        }

        public void EffectActivated(ClientEffect eff)
        {
            clientUIController.SetCurrState($"{(eff.Controller.Friendly ? "Friendly" : "Enemy")} {eff.Source.CardName} Effect Activated",
                eff.blurb);
        }

        public void StackEmptied()
        {
            clientUIController.TargetMode = TargetMode.Free;
            clientUIController.SetCurrState("Stack Empty");
            foreach (var c in Cards) c.ResetForStack();
            ShowNoTargets();
        }

        public void ApplySettings()
        {
            ClientCameraController.ZoomThreshold = ClientSettings.zoomThreshold;
            clientUIController.ApplySettings(ClientSettings);
            foreach (var card in Cards) card.CardController.gameCardViewController.Refresh();
        }

        #region targeting
        /// <summary>
        /// Sets up the client for the player to select targets
        /// </summary>
        public void SetPotentialTargets(int[] ids, ListRestriction listRestriction)
        {
            CurrentPotentialTargets = ids?.Select(i => GetCardWithID(i)).Where(c => c != null).ToArray();
            searchCtrl.StartSearch(CurrentPotentialTargets, listRestriction);
        }

        public void ClearPotentialTargets()
        {
            CurrentPotentialTargets = null;
            searchCtrl.ResetSearch();
        }

        /// <summary>
        /// Makes each card no longer show any highlight about its status as a target
        /// </summary>
        public void ShowNoTargets()
        {
            foreach (var card in Cards) card.CardController.gameCardViewController.Refresh();
        }

        /// <summary>
        /// Show valid target highlight for current potential targets
        /// </summary>
        public void ShowValidCardTargets()
        {
            if (CurrentPotentialTargets != null)
            {
                foreach (var card in CurrentPotentialTargets) card.CardController.gameCardViewController.Refresh();
            }
            else ShowNoTargets();
        }

        public override bool IsCurrentTarget(GameCard card) => searchCtrl.CurrSearchData.HasValue && searchCtrl.CurrSearchData.Value.searched.Contains(card);
        public override bool IsValidTarget(GameCard card) => searchCtrl.CurrSearchData.HasValue && searchCtrl.CurrSearchData.Value.toSearch.Contains(card);
        #endregion targeting
    }
}