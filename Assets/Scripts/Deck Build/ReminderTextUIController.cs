using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

namespace KompasDeckbuilder.UI
{
	public class ReminderTextUIController : MonoBehaviour
	{
		public TMP_Text keywordText;
		public TMP_Text descriptionText;

		public Outline highlightOutline;
		public Outline normalOutline;

		public void Initialize(string keyword, string description, bool highlighted)
		{
			keywordText.text = keyword;
			descriptionText.text = description;

			highlightOutline.enabled = highlighted;
			normalOutline.enabled = !highlighted;
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