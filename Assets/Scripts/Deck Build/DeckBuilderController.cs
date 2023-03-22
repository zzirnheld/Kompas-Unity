using KompasCore.GameCore;
using KompasDeckbuilder.UI.Deck;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KompasDeckbuilder.UI
{
    public class DeckBuilderController : MonoBehaviour
    {
        public CardRepository cardRepo;

        public DeckPaneController deckPaneController;
        public DeckbuilderCardViewController cardViewController;

        public DeckBuilderCardController CurrentDrag { get; set; }

        public void DragEnteredIndex(int index)
        {
            if (null == CurrentDrag) return;

            CurrentDrag.transform.SetSiblingIndex(index);
            deckPaneController.deckController.ChangeDeckIndex(CurrentDrag, index);
        }


        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyUp(KeyCode.S)) deckPaneController.saveController.SaveDeck();
                else if (Input.GetKeyUp(KeyCode.N))
                {
                    deckPaneController.deckController.ClearDeck();
                    deckPaneController.saveController.ShowSaveAs();
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape)) ToMainMenu();
        }

        public void ToMainMenu()
        {
            //load the main menu scene
            SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);
        }

    }
}