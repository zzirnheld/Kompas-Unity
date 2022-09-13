using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace KompasClient.UI
{
    public class ReminderTextUIController : MonoBehaviour
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

        public Dictionary<string, string> KeywordToReminder { get; } = new Dictionary<string, string>();

        public void Initialize()
        {
            foreach(var rti in keywordReminderTexts)
            {
                KeywordToReminder.Add(rti.keyword, rti.reminder);
            }
        }
    }

    public class ReminderTextInfo
    {
        public string keyword;
        public string reminder;
    }
}