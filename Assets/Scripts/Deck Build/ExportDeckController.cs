using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExportDeckController : MonoBehaviour
{
    public TMP_InputField DeckField;

    public void Show(string decklist)
    {
        DeckField.text = decklist;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        DeckField.text = "";
    }
}
