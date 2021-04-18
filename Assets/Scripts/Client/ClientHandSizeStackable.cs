using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasCore.Effects;
using KompasCore.Cards;
using KompasClient.Cards;
using KompasClient.GameCore;

namespace KompasClient.Effects {
    public class ClientHandSizeStackable : HandSizeStackable, IClientStackable
    {
        private readonly ClientPlayer clientController;

        public override Player Controller => clientController;
        
        //Hand size stackable doesn't come from any card
        public override GameCard Source => default;

        public Sprite PrimarySprite => default;
        public CardController PrimaryCardController => default;

        public Sprite SecondarySprite => default;
        public CardController SecondaryCardController => default;

        public string StackableBlurb => $"{(Controller.Friendly ? "Friendly" : "Enemy")} reshuffle to {MaxHandSize} in hand";

        public ClientHandSizeStackable(ClientPlayer clientController)
        {
            this.clientController = clientController;
        }
    }
}