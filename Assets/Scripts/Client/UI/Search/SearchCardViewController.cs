using KompasCore.Cards;
using KompasCore.UI;
using UnityEngine;

namespace KompasClient.UI.Search
{
	public class SearchCardViewController : GameCardlikeViewController
	{
		public CardController CardController { get; private set; }

		private SearchUIController searchUIController;

		public void Initialize(SearchUIController searchUIController, CardController cardController)
		{
			CardController = cardController;
			Debug.Log($"Showing {CardController.Card} as search card view ctrl");
			Focus(cardController.Card);
			DisplaySpecialEffects();

			this.searchUIController = searchUIController;
		}

		protected override string DisplayN(int n) => $"{n}";
		protected override string DisplayE(int e) => $"{e}";
		protected override string DisplayS(int s) => $"{s}";
		protected override string DisplayW(int w) => $"{w}";
		protected override string DisplayC(int c) => $"{c}";
		protected override string DisplayA(int a) => $"{a}";

		protected override void DisplaySpecialEffects()
		{
			base.DisplaySpecialEffects();
			//Never show the valid target object. it's implied.
			validTargetObject.SetActive(false);
		}

		public void OnMouseOver()
		{
			//if the mouse is currently over a ui element, don't swap what you're seeing
			//if (EventSystem.current.IsPointerOverGameObject()) return;
			//Debug.Log($"Showing {CardController.Card}");

			//TODO still hover over even if mouse is on the effect/attack blocks, lol
			searchUIController.cardViewController.Show(CardController.Card);
		}

		public void OnMouseExit()
		{
			if (searchUIController.cardViewController.FocusedCard != shownCard
				&& searchUIController.cardViewController.ShownCard == shownCard)
				searchUIController.cardViewController.Show(null);
		}

		public void OnMouseUp()
		{
			Debug.Log($"Clicked {CardController.Card} for search");
			searchUIController.OnClick(this);
			DisplaySpecialEffects();
		}
	}
}