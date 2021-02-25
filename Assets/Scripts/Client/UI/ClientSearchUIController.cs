using KompasClient.GameCore;
using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientSearchUIController : MonoBehaviour
    {
        public ClientUIController clientUICtrl;
        public CardInfoViewClientUIController cardInfoView;
        private ClientGame ClientGame => clientUICtrl.clientGame;

        //search buttons
        public GameObject endButton;

        //card search
        public GameObject cardSearchView;
        //public Image cardSearchImage;
        public GameObject alreadySelectedText;
        public Button searchTargetButton;
        public TMP_Text searchTargetButtonText;
        public Image nextSearchImage;
        public Image prevSearchImage;
        //search
        private int searchIndex = 0;
        private int SearchLength => Searching ? CurrSearchData.toSearch.Length : 1;
        private int NextSearchIndex => (searchIndex + 1) % SearchLength;
        private int PrevSearchIndex => (searchIndex - 1) + (searchIndex == 0 ? SearchLength : 0);
        public bool Searching => ClientGame.searchCtrl.CurrSearchData.HasValue;
        private ClientSearchController.SearchData CurrSearchData => ClientGame.searchCtrl.CurrSearchData.GetValueOrDefault();

        #region search
        public void StartShowingSearch()
        {
            Debug.Assert(Searching, "Curr search data must have a value to show a search.");
            searchIndex = 0;
            SearchShowIndex(searchIndex);
            if (CurrSearchData.targetingSearch) searchTargetButtonText.text = "Choose";
            else searchTargetButtonText.text = "Cancel";
            cardSearchView.SetActive(true);
        }

        /// <summary>
        /// Called by "choose" button
        /// </summary>
        public void SearchSelectedCard()
        {
            //if the list to search through is null, we're not searching atm.
            if (!Searching) return;

            if (!CurrSearchData.targetingSearch) ClientGame.searchCtrl.ResetSearch();
            else
            {
                GameCard searchSelected = CurrSearchData.toSearch[searchIndex];
                ClientGame.searchCtrl.ToggleTarget(searchSelected);
            }
        }

        public void HideSearch() => cardSearchView.SetActive(false);

        public void HideIfNotShowingCurrSearchIndex()
        {
            if (!Searching || cardInfoView.CurrShown != CurrSearchData.toSearch[searchIndex]) 
                HideSearch();
        }

        public void NextCardSearch()
        {
            searchIndex = NextSearchIndex;
            SearchShowIndex(searchIndex);
        }

        public void PrevCardSearch()
        {
            searchIndex = PrevSearchIndex;
            SearchShowIndex(searchIndex);
        }

        public void SendList() => ClientGame.searchCtrl.SendTargets();

        public void SearchShowIndex(int index)
        {
            if (!Searching)
            {
                Debug.LogWarning("Not searching. Hiding search ui in search show index");
                HideSearch();
                return;
            }

            cardSearchView.SetActive(true);

            var toShow = CurrSearchData.toSearch[index];
            cardInfoView.CurrShown = toShow;
            bool currentTgt = CurrSearchData.searched.Contains(toShow);
            alreadySelectedText.SetActive(currentTgt);
            toShow.cardCtrl.ShowCurrentTarget(currentTgt);
            toShow.cardCtrl.ShowValidTarget(!currentTgt);
            nextSearchImage.sprite = CurrSearchData.toSearch[NextSearchIndex].simpleSprite;
            prevSearchImage.sprite = CurrSearchData.toSearch[PrevSearchIndex].simpleSprite;

            endButton.SetActive(CurrSearchData.HaveEnough);
        }

        public void ReshowSearchShown() => SearchShowIndex(searchIndex);
        #endregion
    }
}