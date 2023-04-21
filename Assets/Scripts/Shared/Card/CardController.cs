using UnityEngine;
using KompasCore.UI;
using System.Linq;

namespace KompasCore.Cards
{
	[RequireComponent(typeof(GameCardViewController))]
	/// <summary>
	/// Controls the card's physical behavior.
	/// </summary>
	public abstract class CardController : MonoBehaviour
	{
		public GameCardViewController gameCardViewController;
		public StackableEntitiesController stackedAugmentsController;

		public abstract GameCard Card { get; }

		public bool ShownInSearch { get; set; }

		public virtual void SetPhysicalLocation(CardLocation location)
		{
			Debug.Log($"Card controller of {Card.CardName} setting physical location in {Card.Location} to {Card.Position}");
			if (location == CardLocation.Nowhere) return;

			gameCardViewController.Refresh();

			//is the card augmenting something?
			if (Card.Attached)
			{
				gameObject.SetActive(true);
				Card.AugmentedCard.CardController.SpreadOutAugs();
				return;
			}

			//Here on out, we assume the card's not an augment
			SetRotation();
			Card.GameLocation.Refresh();
			transform.localScale = Vector3.one;

			/*
			switch (location)
			{
				case CardLocation.Nowhere: break;
				case CardLocation.Deck:
					if (ShownInSearch) break;
					//transform.SetParent(Card.Controller.deckObject.transform);
					//gameObject.SetActive(false); //Setting active/inactive is also currently handled by the GameCardViewController. 
					//TODO: make setActive only be handled by one of these
					break;
				case CardLocation.Board:
				case CardLocation.Hand:
					Card.Controller.handCtrl.SpreadOutCards();
					break;
				case CardLocation.Discard:
				case CardLocation.Annihilation:
					SetRotation();
					break;
				default: throw new System.ArgumentException($"Invalid card location {location} to put card physically at");
			}*/

			SpreadOutAugs();

		}

		protected virtual Transform BoardTransform => Card.Game.BoardController.gameObject.transform;

		public void SpreadOutAugs()
		{
			//Debug.Log($"{Card} has {Card.Augments} and a {stackedAugmentsController}");
			if (Card.Augments.Count == 0) stackedAugmentsController.gameObject.SetActive(false);
			else 
			{
				var augObjs = Card.Augments.Select(c => c.CardController.gameObject).ToArray();
				stackedAugmentsController.gameObject.SetActive(true);
				foreach (var obj in augObjs)
				{
					obj.gameObject.transform.parent = stackedAugmentsController.gameObject.transform;
					obj.gameObject.transform.localScale = Vector3.one * 2f;
				}
				stackedAugmentsController.ShownObjects = augObjs;
			}
		}

		public void SetRotation()
		{
			//Debug.Log($"Setting rotation of {Card.CardName}, controlled by {Card.ControllerIndex}, known? {Card.KnownToEnemy}");
			int yRotation = 180 * Card.ControllerIndex;
			int zRotation = 180 * (Card.KnownToEnemy ? 0 : Card.ControllerIndex);
			transform.eulerAngles = new Vector3(0, yRotation, zRotation);
		}

		public void PutBack() => SetPhysicalLocation(Card.Location);
	}
}