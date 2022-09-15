using KompasClient.GameCore;
using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Second attempt at a search UI controller
namespace KompasClient.UI.Search
{
    public class SearchUIController : MonoBehaviour
    {
        [Tooltip("The amount by which to offset each card from each other")]
        public int cardOffset;

        [Tooltip("The number of columns of cards to show")]
        public int maxColumns;

        [Tooltip("Prefab of card icon to show each card when searching")]
        public GameObject searchCardPrefab;

        public GameObject endButton;

        public ClientSidebarCardViewController cardViewController;
        public ClientSearchController searchController;

        private readonly List<SearchCardViewController> searchCardViewControllers
            = new List<SearchCardViewController>();
        private bool Searching => searchController.CurrSearchData.HasValue;
        private ClientSearchController.SearchData CurrSearchData => searchController.CurrSearchData.GetValueOrDefault();

        public bool CardCurrentlyTargeted(GameCard card) => searchController.CurrSearchData.HasValue && searchController.CurrSearchData.Value.searched.Contains(card);

        public void HideSearch()
        {
            foreach (var card in searchCardViewControllers) Destroy(card.gameObject);
            searchCardViewControllers.Clear();
            cardViewController.ClearFocusLock();

            gameObject.SetActive(false);
        }

        public void ShowSearch()
        {
            HideSearch();

            if (!Searching)
            {
                Debug.Log("Not searching");
                return;
            }

            gameObject.SetActive(true);
            endButton.SetActive(CurrSearchData.HaveEnough);
            int col = 0;
            int row = 0;
            foreach(var card in CurrSearchData.toSearch)
            {
                if (card.CurrentlyVisible) continue;

                //Create the search view controller
                var scvcGameObject = Instantiate(searchCardPrefab, transform);
                scvcGameObject.transform.localPosition = new Vector3(cardOffset * col, 0, cardOffset * row * -1);
                col++;
                if (col >= maxColumns)
                {
                    col = 0;
                    row++;
                }

                var scvc = scvcGameObject.GetComponent<SearchCardViewController>();
                scvc.Initialize(this, card.CardController);
                searchCardViewControllers.Add(scvc);
            }
        }

        public void OnClick(SearchCardViewController searchCardViewController)
        {
            if (cardViewController.FocusedCard == searchCardViewController.CardController.Card)
                searchController.ToggleTarget(searchCardViewController.CardController.Card);

            //Do focus last so it can accurately display that the card is now a target
            cardViewController.Focus(searchCardViewController.CardController.Card, lockFocus: true);
            endButton.SetActive(CurrSearchData.HaveEnough);
        }
    }
}