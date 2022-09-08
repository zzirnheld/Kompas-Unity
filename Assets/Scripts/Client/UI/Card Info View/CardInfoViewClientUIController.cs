using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using KompasCore.Cards;
using UnityEngine.EventSystems;
using System.Linq;
using KompasClient.GameCore;
using Newtonsoft.Json;

namespace KompasClient.UI
{
    public class CardInfoViewClientUIController : MonoBehaviour, IPointerExitHandler
    {

        public Sprite charHaze;
        public Sprite nonCharHaze;
        public Sprite charFrame;
        public Sprite nonCharFrame;

        public GameObject conditionParentObject;
        public GameObject negatedObject;
        public GameObject activatedObject;


        public ClientGame clientGame;

        /// <summary>
        /// The currently shown card by this card info view control controller.
        /// Setting this also takes care of showing the relevant info.
        /// </summary>
        public GameCard CurrShown { get; private set; }

        public void Awake()
        {
            var jsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
            Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(jsonAsset.text);
            gameObject.SetActive(false);
        }

        public void ShowForCurrShown()
        {
            //null means show nothing
            if (CurrShown == null)
            {
                ClearSpecialShownInfo();
                gameObject.SetActive(false);
                return;
            }

            conditionParentObject.SetActive(CurrShown.Negated || CurrShown.Activated);
            negatedObject.SetActive(CurrShown.Negated);
            activatedObject.SetActive(CurrShown.Activated);

            ShowEffButtons();
            ShowReminderText();

            searchUICtrl.HideIfNotShowingCurrSearchIndex();

            ShowUniqueCopies();
            ShowCardLinks();
            ShowPipsAvailableForCost();

            gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (searchUICtrl.Searching) searchUICtrl.ReshowSearchShown();
            else CurrShown = null;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                CurrShown = null;
        }
    }
}