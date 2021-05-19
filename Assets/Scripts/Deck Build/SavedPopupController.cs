using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedPopupController : MonoBehaviour
{
    public const float SavedPopupShowDuration = 1;

    private float showTime;

    public void Show()
    {
        gameObject.SetActive(true);
        showTime = 0f;
    }

    private void Update()
    {
        showTime += Time.deltaTime;
        if (showTime > SavedPopupShowDuration) gameObject.SetActive(false);
    }
}
