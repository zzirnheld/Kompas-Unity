using System.Collections.Generic;
using UnityEngine;
using KompasCore.GameCore;
using KompasCore.Effects;
using KompasCore.Cards;
using KompasClient.Cards;
using KompasClient.Networking;
using KompasClient.UI;

namespace KompasClient.GameCore
{
    public class ClientGame : Game
    {

        public static ClientGame mainClientGame;

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
        public CardRestriction CurrCardRestriction;
        public SpaceRestriction CurrSpaceRestriction;

        public override int Leyload 
        { 
            get => base.Leyload;
            set
            {
                base.Leyload = value;
                clientUICtrl.Leyload = Leyload;
            }
        }

        private void Start()
        {
            mainGame = this;
            mainClientGame = this;
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
        public void RequestMove(GameCard card, int toX, int toY)
        {
            clientNotifier.RequestMove(card, toX, toY);
        }

        public void RequestPlay(GameCard card, int toX, int toY)
        {
            clientNotifier.RequestPlay(card, toX, toY);
        }

        public void TargetCard(GameCard card)
        {
            if (CurrCardRestriction == null)
            {
                Debug.Log($"Called target card on {card.CardName} while curr card restriction is null");
                return;
            }

            //if the player is currently looking for a target on the board,
            if (targetMode == TargetMode.BoardTarget || targetMode == TargetMode.HandTarget)
            {
                //check if the target fits the restriction, according to us
                if (CurrCardRestriction.Evaluate(card, clientNetworkCtrl.X))
                {
                    //if it fits the restriction, send the proposed target to the server
                    clientNotifier.RequestTarget(card);

                    //put the relevant card back
                    card.PutBack();

                    //and change the game's target mode TODO should this do this
                    targetMode = TargetMode.OnHold;
                }
            }
            else
            {
                Debug.LogError($"Tried to target card {card.CardName} while in not understood target mode {targetMode}");
            }
        }

        public void SetFirstTurnPlayer(int playerIndex)
        {
            FirstTurnPlayer = TurnPlayerIndex = playerIndex;
            clientUICtrl.ChangeTurn(playerIndex);
            clientUICtrl.HideGetDecklistUI();
            RoundCount = 1;
            TurnCount = 1;
        }

        public void EndTurn()
        {
            TurnPlayerIndex = 1 - TurnPlayerIndex;
            ResetCardsForTurn();
            clientUICtrl.ChangeTurn(TurnPlayerIndex);
            if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
            TurnCount++;
        }

        public override GameCard GetCardWithID(int id)
        {
            return cardsByID.ContainsKey(id) ? cardsByID[id] : null;
        }

        public void ShowCardsByZoom(bool zoomed)
        {
            foreach (var c in Cards) c.cardCtrl.ShowForCardType(c.CardType, zoomed);
        }
    }
}