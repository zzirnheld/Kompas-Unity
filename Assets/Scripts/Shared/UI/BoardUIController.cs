using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.UI
{
	public abstract class BoardUIController : MonoBehaviour
	{
		public const float BoardLenOffset = 7f;
		public const float LenOneSpace = 2f;
		public const float SpaceOffset = LenOneSpace / 2f;
		public const float CardHeight = 0.15f;

		public abstract BoardController BoardController { get; }
		public abstract UIController UIController { get; }

		public GameObject spaceCueControllerPrefab;
		public Transform spaceCueCubesParent;
		private readonly SpaceCueController[,] spaceCueControllers = new SpaceCueController[Space.BoardLen, Space.BoardLen];
		private GameCardBase currShowingFor;

		private enum CueIndex
		{
			WestCorner, NW3SW2, NW3SW1, NW3SW0,
			NW2SW2, NW2SW1, NW2SW0,
			NW1SW1, NW1SW0,
			Center
		}
		[EnumNamedArray(typeof(CueIndex))]
		public GameObject[] spaceCueControllerSampling;
		private static Space CueIndexMapping(CueIndex index) => index switch
		{
			CueIndex.WestCorner => (6, 0),
			CueIndex.NW3SW2 => (6, 1),
			CueIndex.NW3SW1 => (6, 2),
			CueIndex.NW3SW0 => (6, 3),

			CueIndex.NW2SW2 => (5, 1),
			CueIndex.NW2SW1 => (5, 2),
			CueIndex.NW2SW0 => (5, 3),

			CueIndex.NW1SW1 => (4, 2),
			CueIndex.NW1SW0 => (4, 3),

			CueIndex.Center => (3, 3),

			_ => throw new System.ArgumentException($"Invalid index {index}", "index")
		};

		public static readonly Vector3[,] SpacePositions = new Vector3[7, 7];
		public static Vector3 GridIndicesToCardPos(int x, int y) => SpacePositions[x, y];

		public virtual bool InstantiateCues => true;

		private void Awake()
		{
			if (!InstantiateCues) return;

			foreach (CueIndex cueIndex in Enum.GetValues(typeof(CueIndex)))
			{
				InstantiateCue(cueIndex, false, false);
				InstantiateCue(cueIndex, true, false);
				InstantiateCue(cueIndex, false, true);
				InstantiateCue(cueIndex, true, true);
			}

			//fill in gaps
			for (int x = 0; x < 7; x++)
			{
				for (int y = 0; y < 7; y++)
				{
					if (spaceCueControllers[x, y] != default) continue;
					var localPosition = spaceCueControllers[y, x].transform.localPosition;
					InstantiateCue(x, y, new Vector3(-localPosition.z, localPosition.y, -localPosition.x));
				}
			}
		}

		private void InstantiateCue(CueIndex cueIndex, bool flipX, bool flipZ)
		{
			var (x, y) = CueIndexMapping(cueIndex);
			var (i, j) = (flipX ? 6 - x : x, flipZ ? 6 - y : y);

			//Debug.Log($"Index {cueIndex} out of range of {spaceCueControllerSampling.Length}");
			var position = spaceCueControllerSampling[(int)cueIndex].transform.localPosition;
			var flippedPosition = new Vector3(flipX ? -position.x : position.x, position.y, flipZ ? -position.z : position.z);
			InstantiateCue(i, j, flippedPosition);
		}

		private void InstantiateCue(int x, int y, Vector3 localPosition)
		{
			if (spaceCueControllers[x, y] != default) return;
			GameObject cue = Instantiate(spaceCueControllerPrefab, spaceCueCubesParent);
			cue.transform.localPosition = localPosition;
			cue.name = $"Cue x{x} y{y}";
			spaceCueControllers[x, y] = cue.GetComponent<SpaceCueController>();
			spaceCueControllers[x, y].Init(this, (x, y));
			SpacePositions[x, y] = localPosition + new Vector3(0, 0.05f, 0);
		}

		public virtual void OnMouseDown()
		{
			//select nothing
			UIController.CardViewController.Show(null);
		}

		public void ShowForCard(GameCardBase card)
		{
			currShowingFor = card;

			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					var cue = spaceCueControllers[i, j];

					if (card.MovementRestriction.IsValidNormalMove((i, j)))
						cue.Show(SpaceCueController.CueType.Move);
					else if (card.MovementRestriction.WouldBeValidNormalMove((i, j)))
						cue.Show(SpaceCueController.CueType.MoveOpenGamestate);
					else if (card.AttackingDefenderRestriction.IsValidAttack(BoardController.GetCardAt((i, j)), stackSrc: null))
						cue.Show(SpaceCueController.CueType.Attack);
					else if (card.PlayRestriction.IsRecommendedNormalPlay((i, j), card.Controller))
						cue.Show(SpaceCueController.CueType.Play);
					else
						cue.Clear();
				}
			}
		}

		public void RefreshShownCard() => ShowForCard(currShowingFor);

		public void ShowNothing()
		{
			foreach (var cue in spaceCueControllers) cue.Clear();
		}

		public void ShowSpaceTargets(Func<(int, int), bool> predicate)
		{
			for (int x = 0; x < 7; x++)
			{
				for (int y = 0; y < 7; y++)
				{
					spaceCueControllers[x, y].Show(SpaceCueController.CueType.Target, predicate((x, y)));
				}
			}
		}

		public abstract void Clicked(Space position, GameCard focusCardOverride = null);

		public void Refresh()
		{
			foreach (var card in BoardController.Cards.Where(c => c != null))
			{
				Place(card.CardController, card.Position);
			}
		}
		
		private void Place(CardController card, Space position)
		{
			card.transform.localScale = Vector3.one;
			card.transform.SetParent(transform);
			MoveTo(card, position);
			card.gameObject.SetActive(true);
		}

		/// <summary>
		/// Updates the local position of this card, given a board position
		/// </summary>
		private void MoveTo(CardController card, (int x, int y) to)
		{
			card.transform.localPosition = GridIndicesToCardPos(to.x, to.y);
		}
	}
}