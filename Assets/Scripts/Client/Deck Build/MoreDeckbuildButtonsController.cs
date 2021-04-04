using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreDeckbuildButtonsController : MonoBehaviour
{
    public GameObject moreDeckbuildButtons;

    public void ToggleShowMoreButtons(bool hide)
    {
        moreDeckbuildButtons.SetActive(!hide);
    }
}
