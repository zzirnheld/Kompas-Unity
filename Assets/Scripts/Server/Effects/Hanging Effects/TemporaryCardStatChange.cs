using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
    public class TemporaryCardStatChange : HangingEffect
    {
        private readonly GameCard buffRecipient;
        private readonly CardStats buff;

        public TemporaryCardStatChange(ServerGame game, TriggerRestriction triggerRestriction, string endCondition,
            string fallOffCondition, TriggerRestriction fallOffRestriction, Effect sourceEff,
            ActivationContext currentContext, GameCard buffRecipient, CardStats buff)
            : base(game, triggerRestriction, endCondition, fallOffCondition, fallOffRestriction, sourceEff, currentContext, removeIfEnd: true)
        {
            this.buffRecipient = buffRecipient ?? throw new System.ArgumentNullException("buffRecipient", "Null characcter card in temporary nesw buff");
            this.buff = buff;

            buffRecipient.AddToStats(buff, stackSrc: sourceEff);
        }

        protected override void Resolve(ActivationContext context)
            => buffRecipient.AddToStats(-1 * buff, stackSrc: sourceEff);
    }
}