using UnityEngine;
using TMPro;

namespace KompasClient.UI
{
    public class ReminderTextClientUIController : MonoBehaviour
    {
        public TMP_Text keywordText;
        public TMP_Text descriptionText;

        public void Initialize(string keyword, string description)
        {
            keywordText.text = keyword;
            descriptionText.text = description;
        }
    }

    public class ReminderTextsContainer
    {
        public ReminderTextInfo[] keywordReminderTexts;
    }

    public class ReminderTextInfo
    {
        public string keyword;
        public string reminder;
    }
}