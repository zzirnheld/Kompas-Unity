using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
	public class ImageOnlyCardViewController : BaseCardViewController
	{
		public Image image;

		protected override void DisplayCardImage()
		{
			string cardFileName = shownCard.FileName;
			var cardImageSprite = Resources.Load<Sprite>($"Simple Sprites/{cardFileName}");
			image.sprite = cardImageSprite;
		}

		protected override void DisplayCardNumericStats() { }

		protected override void DisplayCardRulesText() { }
	}
}