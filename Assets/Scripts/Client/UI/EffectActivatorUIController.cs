using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.UI
{
    public class EffectActivatorUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ClientUIController clientUICtrl;
        public GameCard nextShowFor;
        public bool hoveringOverThis;

        public Transform activateableEffParent;
        public GameObject activateableEffPrefab;
        public GameObject cancelButton;
        public TMP_Text cardNameText;

        private readonly List<ActivatableEffectUIController> effectButtons = new List<ActivatableEffectUIController>();

        //todo: make it so only does it when right click over particular card.
        //maybe have a reference to this, and have on pointer enter->exit change the card being shown for?
        //though it should only actually change when the user right clicks or clicks cancel.
        //the variable would just be to track who it should show for,
        //and the right click would be in here,
        //checking if the curring "to be shown for" is null (show nothing/cancel) or non-null

        private void ShowFor(GameCard card)
        {
            if (card != null && card.Effects.Any(eff => clientUICtrl.ShowEffect(eff)))
            {
                //Destroy all buttons that previously existed
                foreach (var button in effectButtons) Destroy(button.gameObject);
                effectButtons.Clear();

                //for each valid effect, create the prefab
                foreach (var eff in card.Effects.Where(e => clientUICtrl.ShowEffect(e)))
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
            else gameObject.SetActive(false);
        }

        public void Show() => ShowFor(nextShowFor);
        public void CancelIfApplicable()
        {
            if (!hoveringOverThis) Cancel();
        }

        public void Cancel()
        {
            nextShowFor = null;
            Show();
        }


        public void OnPointerExit(PointerEventData eventData) => hoveringOverThis = false;
        public void OnPointerEnter(PointerEventData eventData) => hoveringOverThis = true;
    }
}