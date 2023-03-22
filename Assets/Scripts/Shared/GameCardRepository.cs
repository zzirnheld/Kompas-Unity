
using System.Linq;
using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class GameCardRepository : CardRepository
    {
        protected T GetCardController<T>(GameObject gameObject) where T : CardController
        {
            var cardCtrlComponents = gameObject
                .GetComponents(typeof(CardController))
                .Where(c => !(c is T));
            foreach (var c in cardCtrlComponents) Destroy(c);

            //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
            return gameObject.GetComponents<T>().Where(c => c is T).First();
        }
    }
}