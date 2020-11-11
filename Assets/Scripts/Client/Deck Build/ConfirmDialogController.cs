using TMPro;
using UnityEngine;

namespace KompasDeckbuilder
{
    public class ConfirmDialogController : MonoBehaviour
    {
        public enum ConfirmAction { None = 0, LoadDeck = 1, ToMainMenu = 2, ImportDeck = 3 };
        public string[,] ConfirmButtonTexts = new string[,] {
        {"", "", "" },
        {"Load w/o Saving", "Save and Load", "Cancel Load" },
        {"Exit w/o Saving", "Save and Exit", "Cancel Exit" },
        {"Import w/o Saving", "Save and Import", "Cancel Import" }
    };

        public DeckbuilderController deckbuildCtrl;
        public TMP_Text ConfirmText;
        public TMP_Text SaveAndConfirmText;
        public TMP_Text CancelText;

        private ConfirmAction confirmAction = ConfirmAction.None;

        public void Enable(ConfirmAction confirmAction)
        {
            this.confirmAction = confirmAction;
            this.gameObject.SetActive(true);

            ConfirmText.text = ConfirmButtonTexts[(int)confirmAction, 0];
            SaveAndConfirmText.text = ConfirmButtonTexts[(int)confirmAction, 1];
            CancelText.text = ConfirmButtonTexts[(int)confirmAction, 2];
        }

        public void Confirm()
        {
            switch (confirmAction)
            {
                case ConfirmAction.LoadDeck:
                    deckbuildCtrl.ConfirmLoadDeck();
                    break;
                case ConfirmAction.ToMainMenu:
                    deckbuildCtrl.ConfirmToMainMenu();
                    break;
                case ConfirmAction.ImportDeck:
                    deckbuildCtrl.ConfirmImportDeck();
                    break;
            }

            confirmAction = ConfirmAction.None;
            this.gameObject.SetActive(false);
        }

        public void SaveAndConfirm()
        {
            deckbuildCtrl.SaveDeck();

            Confirm();
        }

        public void Cancel()
        {
            switch (confirmAction)
            {
                case ConfirmAction.LoadDeck:
                    deckbuildCtrl.CancelLoadDeck();
                    break;
            }

            confirmAction = ConfirmAction.None;
            this.gameObject.SetActive(false);
        }
    }
}