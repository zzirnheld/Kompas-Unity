using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KompasClient.UI
{
    public class ReminderTextClientUIController : MonoBehaviour
    {
        public TMP_Text keywordText;
        public TMP_Text descriptionText;

        public void SetInfo(string keyword, string description)
        {
            keywordText.text = keyword;
            descriptionText.text = description;
        }
    }
}