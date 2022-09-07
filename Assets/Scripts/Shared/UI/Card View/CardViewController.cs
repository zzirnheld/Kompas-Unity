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
            Show(card, focusedCard == card);
            focusedCard = card;
            //TODO handle card search ui locking from being able to focus on any other card
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

            ShownCard = card;

            //Display any relevant information for the card
            DisplayCardRulesText();
            DisplayCardNumericStats();
            DisplayCardImage();
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