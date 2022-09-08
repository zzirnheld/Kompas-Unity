using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.UI
{
    /// <summary>
    /// Defines the behavior for displaying card information, while not specifying the details of the implementation
    /// </summary>
    public abstract class CardViewController : MonoBehaviour
    {
        /// <summary>
        /// The card currently being shown to the user.
        /// </summary>
        protected GameCardBase ShownCard;

        /// <summary>
        /// The card being "focused" on.
        /// If we're not currently doing something like hovering over another card, this is the one we should be showing
        /// </summary>
        private GameCardBase focusedCard;

        /// <summary>
        /// Focus on a given card.
        /// If we're not currently doing something like hovering over another card, this is the one we should be showing
        /// </summary>
        /// <param name="card"></param>
        public void Focus(GameCardBase card)
        {
            Show(card);
            focusedCard = card;
            //TODO handle card search ui locking from being able to focus on any other card
        }

        /// <summary>
        /// Force an update to the currently shown card's information being displayed
        /// </summary>
        public void Refresh()
        {
            Show(ShownCard, true);
        }

        /// <summary>
        /// Request that information be shown that reflects the given card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="refresh"></param>
        public void Show(GameCardBase card, bool refresh = false)
        {
            //Unless explicitly refreshing card, if already showing that card, no-op.
            if (card == ShownCard && !refresh) return;

            //If we're passed in null, we want to show the focused card, if there is one
            ShownCard = card ?? focusedCard;

            //If we're now showing nothing, hide the window and be done
            if (ShownCard == null)
            {
                ShowingInfo = false;
                return;
            }

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
        /// Display the ShownCard's rules text, like its name, type line, and effect text
        /// </summary>
        protected abstract void DisplayCardRulesText();

        /// <summary>
        /// Display the ShownCard's stats
        /// </summary>
        protected abstract void DisplayCardNumericStats();

        /// <summary>
        /// Display the ShownCard's image, as appropriate
        /// </summary>
        protected abstract void DisplayCardImage();

    }
}