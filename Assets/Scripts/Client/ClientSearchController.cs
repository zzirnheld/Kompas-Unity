using KompasClient.UI;
using KompasClient.UI.Search;
using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.GameCore
{
	public class ClientSearchController : MonoBehaviour
	{
		public class SearchData
		{
			public readonly GameCard[] toSearch;
			public readonly IListRestriction listRestriction;
			public readonly bool targetingSearch;
			public readonly List<GameCard> searched;

			public SearchData(GameCard[] toSearch, IListRestriction listRestriction, bool targetingSearch, List<GameCard> searched)
			{
				this.toSearch = toSearch;
				Array.Sort(this.toSearch);
				this.listRestriction = listRestriction;
				this.targetingSearch = targetingSearch;
				this.searched = searched;
			}

			/// <summary>
			/// Whether the list restriction of this search data determines that enough cards have <b>already</b> been searched 
			/// that the search can end before the maximum possible number of cards have been searched.
			/// </summary>
			public bool HaveEnough => listRestriction?.HaveEnough(searched.Count) ?? false;

			/// <summary>
			/// Whether any cards currently able to be searched can't currently be seen and clicked on.
			/// </summary>
			private bool AnyToSearchNotVisible => toSearch.Any(c => !c.Controller.gameObject.activeSelf);
			public bool ShouldShowSearchUI => AnyToSearchNotVisible || HaveEnough || listRestriction.GetStashedMaximum() == int.MaxValue;

			public string SearchProgress
			{
				get
				{
					int numSearched = searched.Count;
					int min = listRestriction?.GetStashedMinimum() ?? 0;
					int max = listRestriction?.GetStashedMaximum() ?? 0;

					if (listRestriction == null) return null;
					else if (min > 0 && max < int.MaxValue)
						return $"{numSearched} / {min} - {max}";
					else if (max < int.MaxValue) return $"{numSearched} / up to {max}";
					else if (min > 0) return $"{numSearched} / at least {min}";
					else return null;
				}
			}
		}

		[Header("Related MonoBehaviours")]
		public ClientGame clientGame;
		public SearchUIController clientSearchUICtrl;
		public ConfirmTargetsUIController confirmTargetsCtrl;

		public SearchData CurrSearchData { get; private set; } = null;
		private readonly Stack<SearchData> searchStack = new Stack<SearchData>();

		/// <summary>
		/// Whether there's currently any card that the player can target in their search, that isn't visible right in front of them.
		/// </summary>
		public bool ShouldShowSearchUI => CurrSearchData?.ShouldShowSearchUI ?? false;

		public void StartSearch(GameCard[] list, IListRestriction listRestriction, bool targetingSearch = true)
			=> StartSearch(new SearchData(list, listRestriction, targetingSearch, new List<GameCard>()));

		public void StartSearch(SearchData data)
		{
			//if the list is empty, don't search
			if (data.toSearch.Length == 0)
			{
				clientSearchUICtrl.HideSearch();
				return;
			}

			//if should search and already searching, remember current search
			if (CurrSearchData != null) searchStack.Push(CurrSearchData);

			CurrSearchData = data;
			Debug.Log($"Searching a list of {data.toSearch.Length} cards: {string.Join(",", data.toSearch.Select(c => c.CardName))}");

			//initiate search process
			clientSearchUICtrl.ShowSearch();
		}

		/// <summary>
		/// If this is the last search, hides everything. If it's not, moves on to the next search
		/// </summary>
		public void ResetSearch()
		{
			//forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
			CurrSearchData = null; //thank god for garbage collection lol :upside down smiley:

			//if (clientSearchUICtrl.SearchLength == 0)
			clientSearchUICtrl.HideSearch();
			//TODO fix this old logic? don't think it's necessary
			//else if (searchStack.Count > 0) StartSearch(searchStack.Pop());
		}

		/// <summary>
		/// Adds the target, and sends off the list of targets as necessary 
		/// </summary>
		/// <param name="nextTarget"></param>
		/// <returns></returns>
		public void ToggleTarget(GameCard nextTarget)
		{
			//if it's already selected, deselect it
			if (CurrSearchData.searched.Contains(nextTarget)) RemoveTarget(nextTarget);
			//otherwise, deselect
			else AddTarget(nextTarget);
		}

		/// <summary>
		/// Adds the target to the current list of targets, if applicable
		/// </summary>
		/// <param name="nextTarget"></param>
		private void AddTarget(GameCard nextTarget)
		{
			Debug.Log($"Tried to add {nextTarget} as next target");
			if (CurrSearchData == null)
			{
				Debug.LogError($"Called target card on {nextTarget.CardName} while there's no list of potential targets");
				return;
			}

			//check if the target is a valid potential target
			if (!CurrSearchData.toSearch.Contains(nextTarget))
			{
				Debug.LogError($"Tried to target card {nextTarget.CardName} that isn't a valid target");
				return;
			}

			var listRestriction = CurrSearchData.listRestriction;
			if (listRestriction.Deduplicate(CurrSearchData.searched).Count()
				== listRestriction.Deduplicate(CurrSearchData.searched.Append(nextTarget)).Count())
			{
				Debug.LogError($"Allowed user to target non-distinct card {nextTarget} when they had already seen {string.Join(",", CurrSearchData.searched.Select(c => c.CardName))}");
				return;
			}

			CurrSearchData.searched.Add(nextTarget);
			//TODO make be handled by card view controller
			// Debug.Log($"Added {nextTarget.CardName}, targets are now {string.Join(",", CurrSearchData.Value.searched.Select(c => c.CardName))}");

			if (listRestriction == null) SendTargets();
			//if we were given a maximum number to be searched, and hit that number, no reason to keep asking
			else if (CurrSearchData.searched.Count == listRestriction.GetStashedMaximum()) SendTargets();

			clientGame.clientUIController.cardInfoViewUIController.Refresh();
		}

		public void RemoveTarget(GameCard target)
		{
			Debug.Log($"Tried to remove {target} as next target");
			CurrSearchData.searched.Remove(target);
			clientGame.clientUIController.cardInfoViewUIController.Refresh();
		}

		public void ResetCurrentTargets()
		{
			var currTargets = CurrSearchData.searched.ToArray();
			foreach (var c in currTargets) RemoveTarget(c);
		}

		public void SendTargets(bool confirmed = false)
		{
			if (clientGame.clientUIController.clientUISettingsController.ClientSettings.confirmTargets == ConfirmTargets.Prompt && !confirmed)
			{
				confirmTargetsCtrl.Show(CurrSearchData.searched);
				return;
			}

			var targetMode = clientGame.clientUIController.TargetMode;
			Debug.Log($"Sending targets {string.Join(",", CurrSearchData.searched.Select(c => c.CardName))} " +
				$"while in target mode {targetMode}, with a list restriction {CurrSearchData.listRestriction}");
			if (targetMode == TargetMode.HandSize)
				clientGame.clientNotifier.RequestHandSizeChoices(CurrSearchData.searched.Select(c => c.ID).ToArray());
			else if (targetMode == TargetMode.CardTarget)
				clientGame.clientNotifier.RequestTarget(CurrSearchData.searched.FirstOrDefault());
			else if (targetMode == TargetMode.CardTargetList)
				clientGame.clientNotifier.RequestListChoices(CurrSearchData.searched);
			else throw new System.ArgumentException($"Unknown target mode {targetMode} in search ctrl");

			//put the relevant card back
			foreach (var card in CurrSearchData.searched) card.CardController.PutBack();

			ResetSearch();

			//and change the game's target mode TODO should this do this
			clientGame.clientUIController.TargetMode = TargetMode.OnHold;
		}

		public bool IsCurrentlyTargeted(GameCard card) => CurrSearchData?.searched.Contains(card) ?? false;
		/*public bool IsCurrentlyTargeted(GameCard card)
		{
			Debug.Log($"Curr search data searched contains {card}? {CurrSearchData?.searched.Contains(card)}");
			return CurrSearchData?.searched.Contains(card) ?? false;
		}*/

		public bool IsValidTarget(GameCard card) => (CurrSearchData?.toSearch.Contains(card) ?? false) && !IsCurrentlyTargeted(card);
		/*public bool IsValidTarget(GameCard card)
		{
			Debug.Log($"Curr search data to search {CurrSearchData?.toSearch} contains {card}? {CurrSearchData?.toSearch.Contains(card)}");
			return (CurrSearchData?.toSearch.Contains(card) ?? false) && !IsCurrentlyTargeted(card);

		}*/
	}
}