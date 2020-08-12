using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KompasDeckbuilder
{
    public class SaveDeckAsController : MonoBehaviour
    {
        public TMP_Text deckNameText;
        public DeckbuilderController deckbuildCtrl;

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        public void Confirm()
        {
            deckbuildCtrl.SaveDeckAs(deckNameText.text);
            Hide();
        }

        public void Cancel() => Hide();
    }
}