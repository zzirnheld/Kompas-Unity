using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Second attempt at a search UI controller
namespace KompasClient.UI.Search
{
    public class SearchUIController : MonoBehaviour, IEqualityComparer<GameCard>
    {
        [Tooltip("The amount by which to offset each card from each other")]
        public int cardOffset;

        [Tooltip("The number of columns of cards to show")]
        public int maxColumns;

        [Tooltip("Prefab of card icon to show each card when searching")]
        public GameObject searchCardPrefab;

        [Tooltip("Prefab of stackable thing to show a stack of cards while searching")]
        public GameObject searchStack;

        public GameObject endButton;

        public ClientSidebarCardViewController cardViewController;
        public ClientSearchController searchController;

        private readonly List<GameObject> searchGameObjects = new List<GameObject>();
        private bool Searching => searchController.CurrSearchData.HasValue;
        private ClientSearchController.SearchData CurrSearchData => searchController.CurrSearchData.GetValueOrDefault();

        private SearchCardViewController lastClicked;

        public bool CardCurrentlyTargeted(GameCard card) => searchController.CurrSearchData.HasValue && searchController.CurrSearchData.Value.searched.Contains(card);

        public void HideSearch()
        {
            foreach (var go in searchGameObjects) Destroy(go);
            searchGameObjects.Clear();
            cardViewController.ClearFocusLock();

            gameObject.SetActive(false);
        }

        #region Comparison
        //TODO de-duplicate by location, stats differences
        //mark location on the card somehow if/when you do, so player knows why they see 2 diff't ones
        public bool Equals(GameCard a, GameCard b)
        {
            return a.CardName == b.CardName
                && a.Location == b.Location
                && CurrSearchData.searched.Contains(a) == CurrSearchData.searched.Contains(b);
        }

        public int GetHashCode(GameCard obj) => obj.CardName.GetHashCode() + obj.Location.GetHashCode() + CurrSearchData.searched.Contains(obj).GetHashCode();
        #endregion Comparison

        public void ShowSearch()
        {
            HideSearch();

            if (!Searching)
            {
                Debug.Log("Not searching");
                return;
            }

            var toSearch = CurrSearchData.toSearch.GroupBy(c => c, this).ToList();
            //Clear focus so player must click on card twice to select
            if (CurrSearchData.toSearch.Contains(cardViewController.FocusedCard)
                && cardViewController.FocusedCard is GameCard gameCard
                && gameCard.InHiddenLocation) //TODO maybe remove this one? or maybe ignore the top one? dunno
                cardViewController.Focus(null);

            gameObject.SetActive(true);
            endButton.SetActive(CurrSearchData.HaveEnough);
            int col = 0;
            int row = 0;
            foreach(var stackedCards in toSearch)
            {
                var shownCards = stackedCards.Where(card => !card.Controller.gameObject.activeSelf).ToList();

                if (shownCards.Count == 0) continue;

                //Instantiate the stack of cards
                var stackObject = Instantiate(searchStack, transform);
                //Debug.Log("Instantiating stack object");
                searchGameObjects.Add(stackObject);
                var offset = new Vector3(cardOffset * col, 0, cardOffset * row * -1);
                //Debug.Log($"Moving stack object to {offset}");
                stackObject.transform.localPosition = offset;
                Debug.Log(stackObject.transform.localPosition);
                col++;
                if (col >= maxColumns)
                {
                    col = 0;
                    row++;
                }

                //Add the cards
                var stackCtrl = stackObject.GetComponent<StackableEntitiesController>();
                List<GameObject> cardObjects = new List<GameObject>();
                foreach(var card in shownCards)
                {
                    //Create the search view controller
                    var scvcGameObject = Instantiate(searchCardPrefab, stackObject.transform);
                    var scvc = scvcGameObject.GetComponent<SearchCardViewController>();
                    scvc.Initialize(this, card.CardController);
                    cardObjects.Add(scvcGameObject);
                }
                stackCtrl.Initalize(cardObjects);
            }
        }

        public void OnClick(SearchCardViewController searchCardViewController)
        {
            if (cardViewController.FocusedCard == searchCardViewController.CardController.Card)
                searchController.ToggleTarget(searchCardViewController.CardController.Card);

            //Do focus last so it can accurately display that the card is now a target
            cardViewController.Focus(searchCardViewController.CardController.Card, lockFocus: true);
            endButton.SetActive(CurrSearchData.HaveEnough);

            if (lastClicked != null) lastClicked.Refresh();
            lastClicked = searchCardViewController;
        }
    }
}