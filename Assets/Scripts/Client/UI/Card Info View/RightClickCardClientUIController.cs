using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.UI
{
    /// <summary>
    /// Handles the buttons that show when you right-click a card,
    /// allowing you to activate any effects it has
    /// </summary>
    public class RightClickCardClientUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ClientUIController clientUICtrl;

        public Transform activateableEffParent;
        public GameObject activateableEffPrefab;
        public GameObject cancelButton;
        public TMP_Text cardNameText;

        private bool hoveringOverThis;

        private readonly List<ActivatableEffectUIController> effectButtons = new List<ActivatableEffectUIController>();

        void Update()
        {
            //If we're not hovering over this and we just released a left click, get rid of the effect activation buttons
            if (Input.GetMouseButtonUp(0) && !hoveringOverThis) Clear();
        }

        public void Show(GameCard card)
        {
            Debug.Log($"Showing {card?.CardName}");
            Effect[] effects = card?.Effects.Where(ShowEffect).ToArray();

            if (effects?.Any() ?? false)
            {
                //Destroy all buttons that previously existed
                foreach (var button in effectButtons) Destroy(button.gameObject);
                effectButtons.Clear();

                //for each valid effect, create the prefab
                foreach (var eff in effects)
                {
                    var obj = Instantiate(activateableEffPrefab, activateableEffParent);
                    var btn = obj.GetComponent<ActivatableEffectUIController>();
                    btn.Params = (eff, this);
                    effectButtons.Add(btn);
                }

                //then, make sure cancel is last in list. otherwise, it'll be the first one,
                //with effects after it.
                cancelButton.transform.SetAsLastSibling();

                //set name
                cardNameText.text = card.CardName;

                transform.position = Input.mousePosition;
                gameObject.SetActive(true);
            }
            else Clear();
        }

        private bool ShowEffect(Effect eff) => eff.CanBeActivatedBy(eff.Game.Players[0]);

        /// <summary>
        /// Clear out data and hide this right-click UI
        /// </summary>
        public void Clear()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerExit(PointerEventData eventData) => hoveringOverThis = false;
        public void OnPointerEnter(PointerEventData eventData) => hoveringOverThis = true;
    }
}