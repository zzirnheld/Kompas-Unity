using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// A description of one space, based on two others
    /// </summary>
    public interface TwoSpaceIdentity
    {
        public Space SpaceFrom(Space first, Space second);
    }

    public class DisplacementSpaceIdentity : TwoSpaceIdentity
    {
        public Space SpaceFrom(Space first, Space second) => first.DisplacementTo(second);
    }
}