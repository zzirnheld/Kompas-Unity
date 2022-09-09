using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.Cards
{
    //[RequireComponent(typeof(ClientGameCard))]
    //[RequireComponent(typeof(ClientCardMouseController))]
    public class ClientCardController : CardController
    {
        public ClientGameCard clientCard;

        public GameObject revealedImage;

        [Header("Dependent MonoBehaviours")]
        public ClientCardMouseController mouseController;

        public override GameCard Card => clientCard;

        public ClientGame ClientGame => clientCard.ClientGame;
        public ClientUIController ClientUIController => ClientGame.clientUIController;

        public bool Revealed
        {
            set => revealedImage.SetActive(value);
        }

        private void OnDestroy()
        {
            Debug.Log("Destroying a client card ctrl. Destroying this ctrl's mouse ctrl.");
            Destroy(mouseController);
        }
    }
}