using KompasCore.Cards;

namespace KompasCore.Effects
{
    public abstract class Attack : IStackable
    {
        public readonly Player controller;
        public readonly GameCard attacker;
        public readonly GameCard defender;

        public GameCard Source => attacker;
        public Player Controller => controller;

        /// <summary>
        /// Constructor should be called when the attack is declared
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        public Attack(Player controller, GameCard attacker, GameCard defender)
        {
            this.controller = controller != null ? controller : throw new System.ArgumentNullException("controller", "Cannot have null controller of attack");
            this.attacker = attacker != null ? attacker : throw new System.ArgumentNullException("attacker", "Cannot have null attacker");
            this.defender = defender != null ? defender : throw new System.ArgumentNullException("defender", "Cannot have null defender");
        }
    }
}