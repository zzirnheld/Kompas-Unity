using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.UI
{
	/// <summary>
	/// Defines the behavior for displaying card information, while not specifying the details of the implementation
	/// </summary>
	public abstract class BaseCardViewController : MonoBehaviour
	{
		/// <summary>
		/// The card currently being shown to the user.
		/// </summary>
		protected CardBase shownCard;

		/// <summary>
		/// The card being "focused" on.
		/// If we're not currently doing something like hovering over another card, this is the one we should be showing
		/// </summary>
		public CardBase FocusedCard { get; private set; }
		public CardBase ShownCard => shownCard;

		/// <summary>
		/// Focus on a given card.
		/// If we're not currently doing something like hovering over another card, this is the one we should be showing
		/// </summary>
		/// <param name="card"></param>
		public virtual void Focus(CardBase card)
		{
			FocusedCard = card;
			Show(card);
		}

		/// <summary>
		/// Force an update to the currently shown card's information being displayed
		/// </summary>
		public void Refresh()
		{
			Show(shownCard, true);
		}

		/// <summary>
		/// Request that information be shown that reflects the given card.
		/// </summary>
		/// <param name="card"></param>
		/// <param name="refresh"></param>
		public void Show(CardBase card, bool refresh = false)
		{
			//Unless explicitly refreshing card, if already showing that card, no-op.
			if (card == shownCard && !refresh) return;

			//Debug.Log($"Asked to display {card}");
			//If we're passed in null, we want to show the focused card, if there is one
			shownCard = card ?? FocusedCard;

			//If we're now showing nothing, hide the window and be done
			if (shownCard == null) DisplayNothing();
			else Display();
		}

		/// <summary>
		/// Makes everything display nothing/clear out anything it's showing
		/// </summary>
		protected virtual void DisplayNothing()
		{
			ShowingInfo = false;
		}

		protected virtual void Display()
		{
			//If not showing nothing, make sure we're showing information
			ShowingInfo = true;
			//and display any relevant information for the card
			DisplayCardRulesText();
			DisplayCardNumericStats();
			DisplayCardImage();
		}

		/// <summary>
		/// How to show or hide this card view controller.
		/// Can override for special behavior, but the default is to just enable/disable the GameObject
		/// </summary>
		protected virtual bool ShowingInfo
		{
			set => gameObject.SetActive(value);
		}

		/// <summary>
		/// Display the ShownCard's rules text, like its name, type line, and effect text.
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		protected abstract void DisplayCardRulesText();

		/// <summary>
		/// Display the ShownCard's stats
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		protected abstract void DisplayCardNumericStats();

		/// <summary>
		/// Display the ShownCard's image, as appropriate
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		protected abstract void DisplayCardImage();

	}
}