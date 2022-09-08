using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public class UIController : MonoBehaviour
    {
        public const string NoSubtypesUIString = "(No Subtypes)";

        public BoardUIController boardUICtrl;
        public GameMainCardViewController cardViewController;

        public Toggle debugToggle;
        public bool DebugMode { get { return debugToggle.isOn; } }
    }
}