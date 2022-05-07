using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    public interface ActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context);
    }

    public class MainCardBeforeSpaceIdentity : ActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context) => context.mainCardInfoBefore.Position;
    }

    public class ContextSpaceSpaceIdentity : ActivationContextSpaceIdentity
    {
        public Space SpaceFrom(ActivationContext context) => context.space;
    }

    public class TwoSpaceIdentityTriggerSpaceIdentity : ActivationContextSpaceIdentity
    {
        public ActivationContextSpaceIdentity firstSpaceIdentity;
        public ActivationContextSpaceIdentity secondSpaceIdentity;

        public TwoSpaceIdentity compositionSpaceIdentity;

        public Space SpaceFrom(ActivationContext context)
        {
            Space first = firstSpaceIdentity.SpaceFrom(context);
            Space second = secondSpaceIdentity.SpaceFrom(context);
            return compositionSpaceIdentity.SpaceFrom(first, second);
        }
    }
}