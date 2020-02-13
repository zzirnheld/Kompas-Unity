using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDialogController : MonoBehaviour
{
    public TMP_Text ErrorText;

    public void ShowError(string error)
    {
        ErrorText.text = error;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
