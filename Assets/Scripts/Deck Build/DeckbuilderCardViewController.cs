using KompasCore.GameCore;
using KompasCore.UI;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI
{
	public class DeckbuilderCardViewController : TypicalCardViewController
	{
		[Header("Deckbuilder-specific information")]
		public DeckbuilderReminderTextsParentController reminderTextsParentController;
		public override IReminderTextParentController ReminderTextsUIController => reminderTextsParentController;

		//Null to use screen space overlay for intersecting links
		protected override Camera Camera => null;

		protected override void Display()
		{
			base.Display();
			cardModelController.ShowZoom(UIController.ZoomLevel.ZoomedInWithEffectText, ShownCard.CardType == 'C');
		}

		//Show all keywords at all times
		protected override void DisplayReminderTextBlurb()
		{
			var keywordsAndBlurbs = CardRepository.Keywords
				.Where(keyword => effText.text.Contains(keyword))
				.Select(keyword => (keyword, CardRepository.Reminders.KeywordToReminder[keyword]))
				.ToList();
			int hoveredLink = TMP_TextUtilities.FindIntersectingLink(effText, Input.mousePosition, Camera);
			string keywordToHighlight = null;
			if (hoveredLink != -1)
			{
				var linkInfo = effText.textInfo.linkInfo[hoveredLink];
				keywordToHighlight = linkInfo.GetLinkID();
			}
			reminderTextsParentController.Show(keywordsAndBlurbs, keywordToHighlight);
		}
	}
}