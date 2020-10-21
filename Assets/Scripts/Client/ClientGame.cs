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

        public HandController friendlyHandCtrl;
        public DeckController friendlyDeckCtrl;
        public DiscardController friendlyDiscardCtrl;

        public GameObject friendlyHandObj;
        public GameObject friendlyDeckObj;
        public GameObject friendlyDiscardObj;

        public ClientDummyHandController enemyHandCtrl;
        public DeckController enemyDeckCtrl;
        public DiscardController enemyDiscardCtrl;

        public GameObject enemyHandObj;
        public GameObject enemyDeckObj;
        public GameObject enemyDiscardObj;

        public ClientNetworkController clientNetworkCtrl;
        public ClientNotifier clientNotifier;
        public ClientUIController clientUICtrl;

        //targeting
        private GameCard[] currentPotentialTargets;
        public GameCard[] CurrentPotentialTargets 
        { 
            get => currentPotentialTargets; 
            private set
            {
                CurrentPotentialTargets = value;
                ShowValidCardTargets();
            } 
        }
        public int[] PotentialTargetIds
        {
            get => CurrentPotentialTargets.Select(c => c.ID).ToArray();
            //the following should work, because, to quote the docuemntation,
            //"The null-conditional operators are short-circuiting."
            set => CurrentPotentialTargets = value?.Select(i => GetCardWithID(i)).Where(c => c != null).ToArray();
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

        //TODO make client aware that effects have been pushed to stack
        private bool stackEmpty = true;
        public override bool NothingHappening => stackEmpty;

        public bool canZoom = false;

        public override int Leyload 
        { 
            get => base.Leyload;
            set
            {
                base.Leyload = value;
                clientUICtrl.Leyload = Leyload;
            }
        }

        public override void OnClickBoard(int x, int y)
        {
            clientNotifier.RequestSpaceTarget(x, y);
        }

        public void PutCardsBack()
        {
            foreach (var c in Cards) c.PutBack();
        }

        public void SetAvatar(int player, string avatarName, int avatarID)
        {
            if (player >= 2) throw new System.ArgumentException();

            var owner = ClientPlayers[player];
            var avatar = cardRepo.InstantiateClientAvatar(avatarName, this, owner, avatarID);
            owner.Avatar = avatar;
            avatar.Play(player * 6, player * 6, owner);
        }

        public void Delete(GameCard card)
        {
            card.Remove();
            cardsByID.Remove(card.ID);
            Destroy(card.gameObject);
        }

        //requesting
        public void TargetCard(GameCard card)
        {
            if (CurrentPotentialTargets == null)
            {
                Debug.Log($"Called target card on {card.CardName} while there's no list of potential targets");
                return;
            }

            //if the game is currently looking for a target you can click on,
            if (targetMode == TargetMode.CardTarget)
            {
                //check if the target is a valid potential target
                if (CurrentPotentialTargets.Contains(card))
                {
                    //if it fits the restriction, send the proposed target to the server
                    clientNotifier.RequestTarget(card);

                    //put the relevant card back
                    card.PutBack();

                    //and change the game's target mode TODO should this do this
                    targetMode = TargetMode.OnHold;
                }
            }
            else Debug.LogError($"Tried to target card {card.CardName} " +
                $"while in a targetmode {targetMode} where we weren't looking for a target");
        }

        public void SetFirstTurnPlayer(int playerIndex)
        {
            FirstTurnPlayer = TurnPlayerIndex = playerIndex;
            clientUICtrl.ChangeTurn(playerIndex);
            clientUICtrl.HideGetDecklistUI();
            RoundCount = 1;
            TurnCount = 1;
            canZoom = true;
        }

        public void EndTurn()
        {
            TurnPlayerIndex = 1 - TurnPlayerIndex;
            ResetCardsForTurn();
            clientUICtrl.ChangeTurn(TurnPlayerIndex);
            if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
            TurnCount++;
        }

        public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

        public void ShowCardsByZoom(bool zoomed)
        {
            foreach (var c in Cards)
            {
                if(c.gameObject.activeSelf) c.cardCtrl.ShowForCardType(c.CardType, zoomed);
            }
        }

        public void RefreshShownCards() => ShowCardsByZoom(ClientCameraController.Main.Zoomed);

        public void EffectActivated(ClientEffect eff)
        {
            stackEmpty = false;
            clientUICtrl.SetCurrState($"{(eff.Controller.index == 0 ? "Friendly" : "Enemy")} {eff.Source.CardName} Effect Activated",
                eff.Blurb);
        }

        public void StackEmptied()
        {
            stackEmpty = true;
            clientUICtrl.SetCurrState("Stack Empty");
            foreach (var c in Cards) c.ResetForStack();
            ShowNoTargets();
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
    }
}