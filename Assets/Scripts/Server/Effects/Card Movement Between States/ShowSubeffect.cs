using KompasCore.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    /// <summary>
    /// Like a RevealSubeffect, but isn't impossible if the card is already revealed.
    /// </summary>
    public class ShowSubeffect : CardChangeStateSubeffect
    {
        public override bool IsImpossible() => CardTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (CardTarget == null) throw new NullCardException(TargetWasNull);

            try
            {
                CardTarget.Reveal(Effect);
            }
            catch (AlreadyKnownException) { }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}