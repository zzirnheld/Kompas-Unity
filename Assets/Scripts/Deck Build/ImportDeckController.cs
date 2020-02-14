using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImportDeckController : MonoBehaviour
{
    public DeckbuilderController DeckbuildCtrl;
    public TMP_InputField ImportDeckField;
    public TMP_InputField ImportDeckNameField;

    public void ImportDeckFromInput()
    {
        string decklist = ImportDeckField.text;
        string deckName = ImportDeckNameField.text;
        if (string.IsNullOrWhiteSpace(deckName))
        {
            deckName = null;
        }

        DeckbuildCtrl.ImportDeck(decklist, deckName);
        DeckbuildCtrl.SaveDeck();
        FinishDeckImport();
    }

    public void EnableDeckImport()
    {
        this.gameObject.SetActive(true);
    }

    public void FinishDeckImport()
    {
        ImportDeckField.text = "";
        ImportDeckNameField.text = "";
        this.gameObject.SetActive(false);
    }
}
