﻿using KompasCore.Cards;

namespace KompasCore.Effects
{
    public abstract class Trigger
    {
        public const string TurnStart = "Turn Start";
        public const string StackEnd = "Stack End";

        public const string EffectPushedToStack = "Effect Pushed to Stack";

        //change card stats
        public const string StatNumberChange = "Stat Number Change";
        public const string Activate = "Activate";
        public const string Deactivate = "Deactivate";
        public const string Negate = "Negate";
        //X will be equal to the change in the stat
        public const string NChange = "N Change";
        public const string EChange = "E Change";
        public const string SChange = "S Change";
        public const string WChange = "W Change";
        public const string CChange = "C Change";
        public const string AChange = "A Change";
        //X will be equal to the stat's new value
        public const string NSet = "N Set";
        public const string ESet = "E Set";
        public const string SSet = "S Set";
        public const string WSet = "W Set";
        public const string CSet = "C Set";
        public const string ASet = "A Set";

        //combat
        public const string Defends = "Defend";
        public const string Attacks = "Attack";
        public const string TakeCombatDamage = "Take Combat Damage";
        public const string DealCombatDamage = "Deal Combat Damage";
        public const string Battles = "Battle Start";
        public const string BattleEnds = "Battle End";

        //card moving
        public const string EachDraw = "Each Card Drawn";
        public const string DrawX = "Draw";
        public const string Arrive = "Arrive";
        public const string Play = "Play";
        public const string Discard = "Discard";
        public const string Rehand = "Rehand";
        public const string Reshuffle = "Reshuffle";
        public const string Topdeck = "Topdeck";
        public const string Bottomdeck = "Bottomdeck";
        public const string ToDeck = "To Deck";
        public const string Move = "Move";
        public const string Annhilate = "Annihilate";
        public const string Remove = "Remove";

        //Primary card is the card that left the aoe, secondary is the card whose aoe it left
        public const string LeaveAOE = "Leave AOE";

        public const string AugmentAttached = "Augment Attached"; //when an augment becomes applied to a card.
        public const string AugmentDetached = "Augment Detached";
        public const string Augmented = "Augmented"; //when a card has an augment applied to it

        public const string Revealed = "Revealed";
        public const string Vanish = "Vanish";

        public static readonly string[] TriggerConditions = {
            TurnStart, StackEnd, EffectPushedToStack,
            StatNumberChange, Activate, Deactivate, Negate,
            NChange, EChange, SChange, WChange, CChange, AChange,
            NSet, ESet, SSet, WSet, CSet, ASet,
            Defends, Attacks, TakeCombatDamage, DealCombatDamage, Battles, BattleEnds, 
            EachDraw, DrawX, Arrive, Play, Discard, Rehand, Reshuffle, Topdeck, Bottomdeck, ToDeck, Move, Annhilate, Remove, LeaveAOE,
            AugmentAttached, AugmentDetached, Augmented, 
            Revealed, Vanish
        };

        public TriggerData TriggerData { get; }
        public abstract GameCard Source { get; }
        public abstract Effect Effect { get; protected set; }

        public string TriggerCondition => TriggerData.triggerCondition;
        public TriggerRestriction TriggerRestriction => TriggerData.triggerRestriction;
        public bool Optional => TriggerData.optional;
        public string Blurb => TriggerData.blurb ?? Effect.blurb;

        public Trigger(TriggerData triggerData, Effect effect)
        {
            TriggerData = triggerData;
            Effect = effect;
            triggerData.triggerRestriction.Initialize(effect.Game, Source, this, effect);
        }
    }
}