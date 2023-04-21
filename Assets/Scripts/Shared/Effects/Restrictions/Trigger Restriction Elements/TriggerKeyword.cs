using System.Linq;
using KompasServer.Cards;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public class TriggerKeyword : TriggerRestrictionElement
    {
        public string keyword;

        private IRestriction<TriggeringEventContext> [] elements;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            elements = ServerCardRepository.InstantiateTriggerKeyword(keyword);
        }

        protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
            => elements.All(tre => tre.IsValid(context, secondaryContext));
    }
}