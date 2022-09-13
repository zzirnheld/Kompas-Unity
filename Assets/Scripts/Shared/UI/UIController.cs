using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public abstract class UIController : MonoBehaviour
    {
        public const string NoSubtypesUIString = "(No Subtypes)";

        public abstract SidebarCardViewController CardViewController { get; }
        public abstract IReminderTextParentController ReminderTextParentUIController { get; }

        public Toggle debugToggle;
        public bool DebugMode { get { return debugToggle.isOn; } }

        public abstract bool AllowDragging { get; }
    }
}