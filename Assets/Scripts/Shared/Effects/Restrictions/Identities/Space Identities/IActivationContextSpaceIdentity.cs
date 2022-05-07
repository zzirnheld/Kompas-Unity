using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context);
    }

    public class MainCardBeforeSpaceIdentity : IActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context) => context.mainCardInfoBefore.Position;
    }

    public class ContextSpaceSpaceIdentity : IActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context) => context.space;
    }

    public class TwoSpaceIdentityTriggerSpaceIdentity : IActivationContextSpaceIdentity
    {
        public IActivationContextSpaceIdentity firstSpaceIdentity;
        public IActivationContextSpaceIdentity secondSpaceIdentity;

        public ITwoSpaceIdentity compositionSpaceIdentity;

        public Space SpaceFrom(ActivationContext context)
        {
            Space first = firstSpaceIdentity.SpaceFrom(context);
            Space second = secondSpaceIdentity.SpaceFrom(context);
            return compositionSpaceIdentity.SpaceFrom(first, second);
        }
    }
}