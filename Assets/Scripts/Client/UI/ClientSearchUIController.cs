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
        private ClientGame ClientGame => clientUICtrl.clientGame;

        //search buttons
        public GameObject endButton;

        //card search
        public GameObject cardSearchView;
        public Image cardSearchImage;
        public GameObject alreadySelectedText;
        public Button searchTargetButton;
        public TMP_Text searchTargetButtonText;
        public TMP_Text nSearchText;
        public TMP_Text eSearchText;
        public TMP_Text sSearchText;
        public TMP_Text wSearchText;
        public TMP_Text cSearchText;
        public TMP_Text aSearchText;
        //search
        private int searchIndex = 0;
        private ClientSearchController.SearchData? CurrSearchData => ClientGame.searchCtrl.CurrSearchData;

        #region search
        public void StartShowingSearch()
        {
            searchIndex = 0;
            SearchShowIndex(searchIndex);
            if (CurrSearchData.Value.targetingSearch) searchTargetButtonText.text = "Choose";
            else searchTargetButtonText.text = "Cancel";
            cardSearchView.SetActive(true);
        }

        public void SearchSelectedCard()
        {
            //if the list to search through is null, we're not searching atm.
            if (CurrSearchData == null) return;

            if (!CurrSearchData.Value.targetingSearch) ClientGame.searchCtrl.ResetSearch();
            else
            {
                GameCard searchSelected = CurrSearchData.Value.toSearch[searchIndex];
                ClientGame.searchCtrl.ToggleTarget(searchSelected);
            }
        }

        public void HideSearch() => cardSearchView.SetActive(false);

        public void NextCardSearch()
        {
            searchIndex++;
            searchIndex %= CurrSearchData.Value.toSearch.Length;
            SearchShowIndex(searchIndex);
        }

        public void PrevCardSearch()
        {
            searchIndex--;
            if (searchIndex < 0) searchIndex += CurrSearchData.Value.toSearch.Length;
            SearchShowIndex(searchIndex);
        }

        public void SendList() => ClientGame.searchCtrl.SendTargets();

        public void SearchShowIndex(int index)
        {
            if (!CurrSearchData.HasValue)
            {
                HideSearch();
                return;
            }

            var toShow = CurrSearchData.Value.toSearch[index];
            cardSearchImage.sprite = toShow.detailedSprite;
            alreadySelectedText.SetActive(CurrSearchData.Value.searched.Contains(toShow));

            endButton.SetActive(CurrSearchData.Value.listRestriction.HaveEnough(CurrSearchData.Value.searched.Count));

            nSearchText.text = $"N\n{toShow.N}";
            eSearchText.text = $"E\n{toShow.E}";
            sSearchText.text = $"S\n{toShow.S}";
            wSearchText.text = $"W\n{toShow.W}";
            cSearchText.text = $"C\n{toShow.C}";
            aSearchText.text = $"A\n{toShow.A}";

            nSearchText.gameObject.SetActive(toShow.CardType == 'C');
            eSearchText.gameObject.SetActive(toShow.CardType == 'C');
            sSearchText.gameObject.SetActive(toShow.CardType == 'C');
            wSearchText.gameObject.SetActive(toShow.CardType == 'C');
            cSearchText.gameObject.SetActive(toShow.CardType == 'S');
            aSearchText.gameObject.SetActive(toShow.CardType == 'A');
        }

        public void ReshowSearchShown() => SearchShowIndex(searchIndex);

        public void SelectShownSearchCard() 
            => clientUICtrl.cardInfoViewUICtrl.CurrShown = CurrSearchData.Value.toSearch[searchIndex];
        #endregion
    }
}