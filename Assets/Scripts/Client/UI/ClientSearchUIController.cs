using KompasClient.GameCore;
using KompasCore.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientSearchUIController : MonoBehaviour
    {
        public ClientUIController clientUICtrl;
        private ClientGame ClientGame => clientUICtrl.clientGame;

        //search buttons
        public GameObject endButton;

        //card search
        public GameObject confirmTargetPrompt;
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
                //if not holding shift and you're only choosing one item,
                //or you're holding shift but you're choosing more than one item,
                //find the next different card to skip to
                if (Input.GetKey(KeyCode.LeftShift) != CurrSearchData.listRestriction.ChooseMultiple)
                {
                    for (int i = (searchIndex + 1) % SearchLength; i != searchIndex; i = (i + 1) % SearchLength)
                    {
                        //if they're not the same, not just not identical references, which is what != checks
                        if (CurrSearchData.toSearch[i].CompareTo(CurrSearchData.toSearch[searchIndex]) != 0)
                            return i;
                    }
                }

                //fallback
                return (searchIndex + 1) % SearchLength;
            }
        }
        private int PrevSearchIndex
        {
            get
            {
                //if not holding shift and you're only choosing one item,
                //or you're holding shift but you're choosing more than one item,
                //find the last different card to skip to
                if (!(Input.GetKey(KeyCode.LeftShift) ^ CurrSearchData.listRestriction.ChooseMultiple))
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

                //fallback
                return (searchIndex - 1) + (searchIndex == 0 ? SearchLength : 0);
            }
        }
        public bool Searching => ClientGame.searchCtrl.CurrSearchData.HasValue;
        private ClientSearchController.SearchData CurrSearchData => ClientGame.searchCtrl.CurrSearchData.GetValueOrDefault();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift)) ReshowSearchShownIfSearching();
        }

        #region search
        public void StartShowingSearch()
        {
            Debug.Assert(Searching, "Curr search data must have a value to show a search.");
            searchIndex = 0;
            SearchShowIndex(searchIndex);

            searchTargetButtonText.text = CurrSearchData.targetingSearch ? "Choose" : "Cancel";
            gameObject.SetActive(CurrSearchData.ShouldShowSearchUI);
        }

        /// <summary>
        /// Called by "choose" button
        /// </summary>
        public void SearchSelectedCard()
        {
            //if the list to search through is null, we're not searching atm.
            if (!Searching) return;

            if (CurrSearchData.targetingSearch)
            {
                GameCard searchSelected = CurrSearchData.toSearch[searchIndex];
                ClientGame.searchCtrl.ToggleTarget(searchSelected);
            }
            else ClientGame.searchCtrl.ResetSearch();
        }

        public void HideSearch()
        {
            gameObject.SetActive(false);
            alreadySelectedText.SetActive(false);
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
                clientUICtrl.cardInfoViewUIController.ClearFocusLock();
                return;
            }

            gameObject.SetActive(CurrSearchData.ShouldShowSearchUI);

            var toShow = CurrSearchData.toSearch[index];
            clientUICtrl.cardInfoViewUIController.Focus(toShow);

            bool currentTgt = CurrSearchData.searched.Contains(toShow);
            alreadySelectedText.SetActive(currentTgt);
            toShow.CardController.gameCardViewController.ShowCurrentTarget(currentTgt);
            toShow.CardController.gameCardViewController.ShowValidTarget(!currentTgt);
            nextSearchImage.sprite = CurrSearchData.toSearch[NextSearchIndex].SimpleSprite;
            prevSearchImage.sprite = CurrSearchData.toSearch[PrevSearchIndex].SimpleSprite;

            endButton.SetActive(CurrSearchData.HaveEnough);
            var progress = CurrSearchData.SearchProgress;
            if (progress != null) clientUICtrl.UpdateCurrState(numTargetsChosen: progress);
        }

        /// <summary>
        /// Show the currently search-looking-at card, if there is any.
        /// If not searching, hides the search ui appropriately.
        /// </summary>
        public void ReshowSearchShown() => SearchShowIndex(searchIndex);

        public void ReshowSearchShownIfSearching() { if (Searching) ReshowSearchShown(); }
        #endregion
    }
}