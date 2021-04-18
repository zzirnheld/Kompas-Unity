using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasClient.Effects
{
    public interface IClientStackable : IStackable
    {
        /// <summary>
        /// The primary card for this stackable. The source of an effect, or an attacker
        /// </summary>
        Sprite PrimarySprite { get; }

        /// <summary>
        /// The secondary card for this stackable. The defender
        /// </summary>
        Sprite SecondarySprite { get; }

        /// <summary>
        /// The blurb for this stackable
        /// </summary>
        string StackableBlurb { get; }
    }
}