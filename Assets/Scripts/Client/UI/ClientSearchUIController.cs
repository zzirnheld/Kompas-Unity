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
        public int SearchLength => Searching ? CurrSearchData.toSearch.Length : 1;
        private int NextSearchIndex
        {
            get 
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    for (int i = (searchIndex + 1) % SearchLength; i != searchIndex; i = (i + 1) % SearchLength)
                    {
                        //if they're not the same, not just not identical references, which is what != checks
                        if (CurrSearchData.toSearch[i].CompareTo(CurrSearchData.toSearch[searchIndex]) != 0)
                            return i;
                    }
                }
                
                //fallback, or if holding shift
                return (searchIndex + 1) % SearchLength;
            }
        }
        private int PrevSearchIndex
        {
            get
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    for (int i = (searchIndex - 1) + (searchIndex == 0 ? SearchLength : 0); 
                        i != searchIndex; 
                        i = (i - 1) + (i == 0 ? SearchLength : 0))
                    {
                        //if they're not the same, not just not identical references, which is what != checks
                        if (CurrSearchData.toSearch[i].CompareTo(CurrSearchData.toSearch[searchIndex]) != 0)
                            return i;
                    }
                }

                //fallback, or if holding shift
                return (searchIndex - 1) + (searchIndex == 0 ? SearchLength : 0);
            }
        }
        public bool Searching => ClientGame.searchCtrl.CurrSearchData.HasValue;
        private ClientSearchController.SearchData CurrSearchData => ClientGame.searchCtrl.CurrSearchData.GetValueOrDefault();

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift)) ReshowSearchShownIfSearching();
        }

        #region search
        public void StartShowingSearch()
        {
            Debug.Assert(Searching, "Curr search data must have a value to show a search.");
            searchIndex = 0;
            SearchShowIndex(searchIndex);
            if (CurrSearchData.targetingSearch) searchTargetButtonText.text = "Choose";
            else searchTargetButtonText.text = "Cancel";
            cardSearchView.SetActive(CurrSearchData.AnyToSearchNotVisible);
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

        public void HideSearch()
        {
            cardSearchView.SetActive(false);
            alreadySelectedText.SetActive(false);
        }

        public void HideIfNotShowingCurrSearchIndex()
        {
            if (!Searching || searchIndex >= SearchLength || cardInfoView.CurrShown != CurrSearchData.toSearch[searchIndex]) 
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
            if (!Searching || index >= SearchLength)
            {
                //Debug.LogWarning("Not searching. Hiding search ui in search show index");
                HideSearch();
                return;
            }

            cardSearchView.SetActive(CurrSearchData.AnyToSearchNotVisible);

            var toShow = CurrSearchData.toSearch[index];
            cardInfoView.ShowInfoFor(toShow);
            bool currentTgt = CurrSearchData.searched.Contains(toShow);
            alreadySelectedText.SetActive(currentTgt);
            toShow.cardCtrl.ShowCurrentTarget(currentTgt);
            toShow.cardCtrl.ShowValidTarget(!currentTgt);
            nextSearchImage.sprite = CurrSearchData.toSearch[NextSearchIndex].simpleSprite;
            prevSearchImage.sprite = CurrSearchData.toSearch[PrevSearchIndex].simpleSprite;

            endButton.SetActive(CurrSearchData.HaveEnough);
            var progress = CurrSearchData.SearchProgress;
            if(progress != null) clientUICtrl.UpdateCurrState(numTargetsChosen: progress);
        }

        /// <summary>
        /// Show the currently search-looking-at card, if there is any.
        /// If not searching, hides the search ui appropriately.
        /// </summary>
        public void ReshowSearchShown() => SearchShowIndex(searchIndex);

        public void ReshowSearchShownIfSearching () { if (Searching) ReshowSearchShown(); }
        #endregion
    }
}