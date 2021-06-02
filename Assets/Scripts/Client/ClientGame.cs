using KompasClient.Cards;
using KompasClient.Effects;
using KompasClient.Networking;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientGame : Game
    {
        public GameObject AvatarPrefab;

        public override Player[] Players => ClientPlayers;
        public ClientPlayer[] ClientPlayers;

        public Dictionary<int, ClientGameCard> cardsByID = new Dictionary<int, ClientGameCard>();
        public IEnumerable<ClientGameCard> ClientCards => cardsByID.Values;
        public override IEnumerable<GameCard> Cards => ClientCards;

        public ClientHandController friendlyHandCtrl;
        public ClientDeckController friendlyDeckCtrl;
        public ClientDiscardController friendlyDiscardCtrl;

        public GameObject friendlyHandObj;
        public GameObject friendlyDeckObj;
        public GameObject friendlyDiscardObj;

        public ClientDummyHandController enemyHandCtrl;
        public ClientDeckController enemyDeckCtrl;
        public ClientDiscardController enemyDiscardCtrl;

        public GameObject enemyHandObj;
        public GameObject enemyDeckObj;
        public GameObject enemyDiscardObj;

        public ClientNetworkController clientNetworkCtrl;
        public ClientNotifier clientNotifier;
        public ClientUIController clientUICtrl;
        public ClientUISettingsController clientUISettingsCtrl;
        public ClientEffectsController clientEffectsCtrl;

        //turn players?
        public bool FriendlyTurn => TurnPlayerIndex == 0;

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
                if (value != null) clientUICtrl.boardUICtrl.ShowSpaceTargets(space => value.Contains(space));
                else clientUICtrl.boardUICtrl.ShowSpaceTargets(_ => false);
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
                clientUICtrl.Leyload = Leyload;
            }
        }

        private void Awake()
        {
            clientUISettingsCtrl.LoadSettings();
            ApplySettings();
        }

        public override void OnClickBoard(int x, int y)
        {
            clientNotifier.RequestSpaceTarget(x, y);
        }

        public void MarkCardDirty(GameCard card) => dirtyCardList.Add(card);

        public void PutCardsBack()
        {
            foreach (var c in dirtyCardList) c.PutBack();
            dirtyCardList.Clear();
        }

        public void SetAvatar(int player, string json, int avatarID)
        {
            if (player >= 2) throw new System.ArgumentException();

            var owner = ClientPlayers[player];
            var avatar = cardRepo.InstantiateClientAvatar(json, this, owner, avatarID);
            owner.Avatar = avatar;
            Space to = player == 0 ? Space.NearCorner : Space.FarCorner;
            avatar.Play(to, owner);
        }

        public void Delete(GameCard card)
        {
            card.Remove();
            cardsByID.Remove(card.ID);
            Destroy(card.gameObject);
        }

        //requesting
        public void SetFirstTurnPlayer(int playerIndex)
        {
            FirstTurnPlayer = TurnPlayerIndex = playerIndex;
            clientUICtrl.ChangeTurn(playerIndex);
            clientUICtrl.HideGetDecklistUI();
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
            clientUICtrl.ChangeTurn(TurnPlayerIndex);
            if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
            TurnCount++;
            clientUICtrl.FriendlyPips = Players[0].Pips;
            clientUICtrl.EnemyPips = Players[1].Pips;
        }

        public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

        public void ShowCardsByZoom(bool zoomed)
        {
            //TODO make this better with a dirty list
            foreach (var c in Cards.Where(c => c != null && c.gameObject.activeSelf))
            {
                c.cardCtrl.ShowForCardType(c.CardType, zoomed);
            }
        }

        /// <summary>
        /// Makes cards show again, in case information changed after the packet.
        /// </summary>
        public void RefreshShownCards() => ShowCardsByZoom(ClientCameraController.Main.Zoomed);

        public void EffectActivated(ClientEffect eff)
        {
            clientUICtrl.SetCurrState($"{(eff.Controller.Friendly ? "Friendly" : "Enemy")} {eff.Source.CardName} Effect Activated",
                eff.blurb);
        }

        public void StackEmptied()
        {
            targetMode = TargetMode.Free;
            clientUICtrl.SetCurrState("Stack Empty");
            foreach (var c in Cards) c.ResetForStack();
            ShowNoTargets();
        }

        public void ApplySettings()
        {
            var uiSettings = clientUISettingsCtrl.ClientUISettings;
            ClientCameraController.ZoomThreshold = uiSettings.zoomThreshold;
            foreach (var card in ClientCards) card.clientCardCtrl.ApplySettings(uiSettings);
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
            foreach (var card in Cards) card.cardCtrl.HideTarget();
        }

        /// <summary>
        /// Show valid target highlight for current potential targets
        /// </summary>
        public void ShowValidCardTargets()
        {
            if (CurrentPotentialTargets != null)
            {
                foreach (var card in CurrentPotentialTargets) card.cardCtrl.ShowValidTarget();
            }
            else ShowNoTargets();
        }
        #endregion targeting
    }
}