using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.GameCore.Extensions
{
    public static class ServerValidityCheckerExtensions
    {
        public static bool IsValidNormalPlay(this ServerGame game, GameCard card, Space to, ServerPlayer player)
        {
            if (card == null) return false;
            if (game.uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode, always return true for valid play");
                return true;
            }

            //Debug.Log($"Checking validity of playing {card.CardName} to {to}");
            return card.PlayRestriction.IsValidNormalPlay(to, player);
        }

        public static bool IsValidNormalAttach(this ServerGame game, GameCard card, Space to, ServerPlayer player)
        {
            if (game.uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode, always return true for valid augment");
                return true;
            }

            //Debug.Log($"Checking validity augment of {card.CardName} to {to}, on {boardCtrl.GetCardAt(to)}");
            return card != null && card.CardType == 'A' && to.IsValid
                && !game.boardCtrl.IsEmpty(to)
                && card.PlayRestriction.IsValidNormalPlay(to, player);
        }

        public static bool IsValidNormalMove(this ServerGame game, GameCard toMove, Space to, Player by)
        {
            if (game.uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode, always return true for valid move");
                return true;
            }

            //Debug.Log($"Checking validity of moving {toMove.CardName} to {to}");
            if (toMove.Position == to) return false;
            else return toMove.MovementRestriction.IsValidNormalMove(to);
        }

        public static bool ValidAttack(this ServerGame game, GameCard attacker, GameCard defender, ServerPlayer instigator)
        {
            if (game.uiCtrl.DebugMode)
            {
                Debug.LogWarning("Debug mode, always return true for valid attack");
                return attacker != null && defender != null;
            }

            //Debug.Log($"Checking validity of attack of {attacker.CardName} on {defender} by {instigator.index}");
            return attacker.AttackRestriction.IsValidAttack(defender, stackSrc: null);
        }
    }
}