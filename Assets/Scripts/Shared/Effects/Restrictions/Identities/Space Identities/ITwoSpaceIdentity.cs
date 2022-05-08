using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// A description of one space, based on two others
    /// </summary>
    public interface ITwoSpaceIdentity
    {
        public Space SpaceFrom(Space first, Space second);
    }

    public class DisplacementTwoSpaceIdentity : ITwoSpaceIdentity
    {
        public Space SpaceFrom(Space first, Space second) => first.DisplacementTo(second);
    }

    public class SpaceBetweenTwoSpaceIdentity : ITwoSpaceIdentity
    {
        public Space SpaceFrom(Space first, Space second) => first.DirectlyBetween(second);
    }
}