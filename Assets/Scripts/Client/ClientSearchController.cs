using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientSearchController : MonoBehaviour
    {
        //this is probably deprecated now that you have no reason to search the discard,
        //but i might eventually let you search your deck, so, uh, nyeh
        public struct SearchData
        {
            public readonly GameCard[] toSearch;
            public readonly ListRestriction listRestriction;
            public readonly bool targetingSearch;
            public readonly List<GameCard> searched;

            public SearchData(GameCard[] toSearch, ListRestriction searchRestriction, bool targetingSearch, List<GameCard> searched)
            {
                this.toSearch = toSearch;
                this.listRestriction = searchRestriction;
                this.targetingSearch = targetingSearch;
                this.searched = searched;
            }
        }

        public SearchData? CurrSearchData { get; private set; } = null;
        private readonly Stack<SearchData> searchStack = new Stack<SearchData>();

        public ClientGame clientGame;
        public ClientSearchUIController clientSearchUICtrl;

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
            if (data.toSearch.Any(c => !c.CurrentlyVisible)) clientSearchUICtrl.StartShowingSearch();
        }

        /// <summary>
        /// If this is the last search, hides everything. If it's not, moves on to the next search
        /// </summary>
        public void ResetSearch()
        {
            //forget what we were searching through. don't just clear the list because that might clear the actual deck or discard
            CurrSearchData = null; //thank god for garbage collection lol :upside down smiley:

            if (searchStack.Count == 0) clientSearchUICtrl.HideSearch();
            else StartSearch(searchStack.Pop());
        }

        /// <summary>
        /// Adds the target, and sends off the list of targets as necessary 
        /// </summary>
        /// <param name="nextTarget"></param>
        /// <returns></returns>
        public void ToggleTarget(GameCard nextTarget)
        {
            if (CurrSearchData.Value.searched.Contains(nextTarget)) RemoveTarget(nextTarget);
            //otherwise, deselect
            else AddTarget(nextTarget);
        }

        public void AddTarget(GameCard nextTarget)
        {
            if(CurrSearchData == null)
            {
                Debug.Log($"Called target card on {nextTarget.CardName} while there's no list of potential targets");
                return;
            }

            //check if the target is a valid potential target
            if (!CurrSearchData.Value.toSearch.Contains(nextTarget))
                Debug.LogError($"Tried to target card {nextTarget.CardName} that isn't a valid target");

            CurrSearchData.Value.searched.Add(nextTarget);
            var listRestriction = CurrSearchData.Value.listRestriction;

            if (listRestriction == null) SendTargets();
            //only do the rest of the operations if adding it doesn't violate the list restriction
            else if (listRestriction.Evaluate(CurrSearchData.Value.searched, clientGame.CurrentPotentialTargets))
            {
                //if we were given a maximum number to be searched, and hit that number, no reason to keep asking
                if (listRestriction.HasMax && CurrSearchData.Value.searched.Count >= listRestriction.maxCanChoose)
                    SendTargets();
            }
            else CurrSearchData.Value.searched.Remove(nextTarget);

            clientSearchUICtrl.ReshowSearchShown();
        }

        public void RemoveTarget(GameCard target)
        {
            CurrSearchData.Value.searched.Remove(target);
            clientSearchUICtrl.ReshowSearchShown();
        }

        public void SendTargets()
        {
            if (CurrSearchData.Value.listRestriction == null)
                clientGame.clientNotifier.RequestTarget(CurrSearchData.Value.searched.FirstOrDefault());
            else 
                clientGame.clientNotifier.RequestListChoices(CurrSearchData.Value.searched);

            ResetSearch();

            //put the relevant card back
            foreach(var card in CurrSearchData.Value.searched) card.PutBack();

            //and change the game's target mode TODO should this do this
            clientGame.targetMode = ClientGame.TargetMode.OnHold;
        }
    }
}