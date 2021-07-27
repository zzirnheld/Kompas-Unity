using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientSearchController : MonoBehaviour
    {
        public struct SearchData
        {
            public readonly GameCard[] toSearch;
            public readonly ListRestriction listRestriction;
            public readonly bool targetingSearch;
            public readonly List<GameCard> searched;

            public SearchData(GameCard[] toSearch, ListRestriction searchRestriction, bool targetingSearch, List<GameCard> searched)
            {
                this.toSearch = toSearch;
                Array.Sort(this.toSearch);
                this.listRestriction = searchRestriction;
                this.targetingSearch = targetingSearch;
                this.searched = searched;
            }

            /// <summary>
            /// Whether the list restriction of this search data determines that enough cards have <b>already</b> been searched 
            /// that the search can end before the maximum possible number of cards have been searched.
            /// </summary>
            public bool HaveEnough => listRestriction.HaveEnough(searched.Count);

            /// <summary>
            /// Whether any cards currently able to be searched can't currently be seen and clicked on.
            /// </summary>
            public bool AnyToSearchNotVisible => toSearch.Any(c => !c.CurrentlyVisible);

            public string SearchProgress
            {
                get
                {
                    int numSearched = searched.Count;
                    if (listRestriction == null) return null;
                    else if (listRestriction.HasMin && listRestriction.HasMax)
                        return $"{numSearched} / {listRestriction.minCanChoose} - {listRestriction.maxCanChoose}";
                    else if (listRestriction.HasMax) return $"{numSearched} / {listRestriction.maxCanChoose}";
                    else if (listRestriction.HasMin) return $"{numSearched} / {listRestriction.minCanChoose}";
                    else return null;
                }
            }
        }

        public SearchData? CurrSearchData { get; private set; } = null;
        private readonly Stack<SearchData> searchStack = new Stack<SearchData>();

        /// <summary>
        /// Whether there's currently any card that the player can target in their search, that isn't visible right in front of them.
        /// </summary>
        public bool CanSearchNotVisibleCard => CurrSearchData.HasValue && CurrSearchData.Value.AnyToSearchNotVisible;

        public ClientGame clientGame;
        public ClientSearchUIController clientSearchUICtrl;
        public ConfirmTargetsUIController confirmTargetsCtrl;

        public void StartSearch(GameCard[] list, ListRestriction listRestriction, bool targetingSearch = true)
            => StartSearch(new SearchData(list, listRestriction, targetingSearch, new List<GameCard>()));

        public void StartSearch(SearchData data)
        {
            //if the list is empty, don't search
            if (data.toSearch.Length == 0) return;

            //if should search and already searching, remember current search
            if (CurrSearchData.HasValue) searchStack.Push(CurrSearchData.Value);

            CurrSearchData = data;
            Debug.Log($"Searching a list of {data.toSearch.Length} cards: {string.Join(",", data.toSearch.Select(c => c.CardName))}");

            //initiate search process
            if (CanSearchNotVisibleCard) clientSearchUICtrl.StartShowingSearch();
        }

        /// <summary>
        /// If this is the last search, hides everything. If it's not, moves on to the next search
        /// </summary>
        public void ResetSearch()
        {
            //forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
            CurrSearchData = null; //thank god for garbage collection lol :upside down smiley:

            if (clientSearchUICtrl.SearchLength == 0) clientSearchUICtrl.HideSearch();
            else if(searchStack.Count > 0) StartSearch(searchStack.Pop());
        }

        /// <summary>
        /// Adds the target, and sends off the list of targets as necessary 
        /// </summary>
        /// <param name="nextTarget"></param>
        /// <returns></returns>
        public void ToggleTarget(GameCard nextTarget)
        {
            //if it's already selected, deselect it
            if (CurrSearchData.Value.searched.Contains(nextTarget))
            {
                RemoveTarget(nextTarget);
                nextTarget.cardCtrl.ShowValidTarget(clientGame.CurrentTargetType, valid: true);
                nextTarget.cardCtrl.ShowCurrentTarget(current: false);
            }
            //otherwise, deselect
            else AddTarget(nextTarget);
        }

        /// <summary>
        /// Adds the target to the current list of targets, if applicable
        /// </summary>
        /// <param name="nextTarget"></param>
        private void AddTarget(GameCard nextTarget)
        {
            if(CurrSearchData == null)
            {
                Debug.LogError($"Called target card on {nextTarget.CardName} while there's no list of potential targets");
                return;
            }

            //check if the target is a valid potential target
            if (!CurrSearchData.Value.toSearch.Contains(nextTarget))
            {
                Debug.LogError($"Tried to target card {nextTarget.CardName} that isn't a valid target");
                return;
            }

            CurrSearchData.Value.searched.Add(nextTarget);
            nextTarget.cardCtrl.ShowCurrentTarget();
            nextTarget.cardCtrl.ShowValidTarget(clientGame.CurrentTargetType, valid: false);
            // Debug.Log($"Added {nextTarget.CardName}, targets are now {string.Join(",", CurrSearchData.Value.searched.Select(c => c.CardName))}");

            var listRestriction = CurrSearchData.Value.listRestriction;

            if (listRestriction == null) SendTargets();
            //only do the rest of the operations if adding it doesn't violate the list restriction
            //if we were given a maximum number to be searched, and hit that number, no reason to keep asking
            else if (listRestriction.HasMax && CurrSearchData.Value.searched.Count >= listRestriction.maxCanChoose)
                SendTargets();

            clientSearchUICtrl.ReshowSearchShown();
        }

        public void RemoveTarget(GameCard target)
        {
            CurrSearchData.Value.searched.Remove(target);
            clientSearchUICtrl.ReshowSearchShown();
        }

        public void ResetCurrentTargets()
        {
            var currTargets = CurrSearchData.Value.searched.ToArray();
            foreach (var c in currTargets) RemoveTarget(c);
        }

        public void SendTargets(bool confirmed = false)
        {
            if (clientGame.ClientUISettings.confirmTargets == ConfirmTargets.Prompt && !confirmed)
            {
                confirmTargetsCtrl.Show(CurrSearchData.Value.searched);
                return;
            }

            Debug.Log($"Sending targets {string.Join(",", CurrSearchData.Value.searched.Select(c => c.CardName))} " +
                $"while in target mode {clientGame.targetMode}, with a list restriction {CurrSearchData.Value.listRestriction}");
            if (clientGame.targetMode == ClientGame.TargetMode.HandSize)
                clientGame.clientNotifier.RequestHandSizeChoices(CurrSearchData.Value.searched.Select(c => c.ID).ToArray());
            else if (clientGame.targetMode == ClientGame.TargetMode.CardTarget)
                clientGame.clientNotifier.RequestTarget(CurrSearchData.Value.searched.FirstOrDefault());
            else if (clientGame.targetMode == ClientGame.TargetMode.CardTargetList)
                clientGame.clientNotifier.RequestListChoices(CurrSearchData.Value.searched);
            else throw new System.ArgumentException($"Unknown target mode {clientGame.targetMode} in search ctrl");

            //put the relevant card back
            foreach (var card in CurrSearchData.Value.searched) card.PutBack();

            ResetSearch();

            //and change the game's target mode TODO should this do this
            clientGame.targetMode = ClientGame.TargetMode.OnHold;
        }
    }
}