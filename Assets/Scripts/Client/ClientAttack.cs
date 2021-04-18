
using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Effects
{
    public class ClientAttack : Attack, IClientStackable
    {
        public Sprite PrimarySprite => attacker.simpleSprite;
        public CardController PrimaryCardController => attacker.cardCtrl;

        public Sprite SecondarySprite => defender.simpleSprite;
        public CardController SecondaryCardController => defender.cardCtrl;

        public string StackableBlurb => $"{attacker.CardName} attacks {defender.CardName}";

        public ClientAttack(Player controller, GameCard attacker, GameCard defender) : base(controller, attacker, defender) { }
    }
}