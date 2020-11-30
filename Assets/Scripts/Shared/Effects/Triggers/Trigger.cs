using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    public abstract class Trigger
    {
        public const string TurnStart = "Turn Start";

        //change card stats
        public const string NESWChange = "NESW Change";
        public const string Activate = "Activate";
        public const string Deactivate = "Deactivate";
        public const string Negate = "Negate";
        public const string NChange = "N Change";
        public const string EChange = "E Change";
        public const string SChange = "S Change";
        public const string WChange = "W Change";
        public const string CChange = "C Change";
        public const string AChange = "A Change";

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
        public const string AugmentAttached = "Augment Attached"; //when an augment becomes applied to a card.
        public const string AugmentDetached = "Augment Detached";
        public const string Augmented = "Augmented"; //when a card has an augment applied to it

        public static readonly string[] TriggerConditions = {
            TurnStart,
            NESWChange, Activate, Deactivate, NChange, EChange, SChange, WChange, CChange, AChange,
            Defends, Attacks, TakeCombatDamage, DealCombatDamage, Battles, BattleEnds,
            EachDraw, DrawX, Arrive, Play, Discard, Rehand, Reshuffle, Topdeck, Bottomdeck, ToDeck, Move, Annhilate, Remove, AugmentAttached, AugmentDetached
        };

        public TriggerData TriggerData { get; }
        public abstract GameCard Source { get; }
        public abstract Effect Effect { get; protected set; }

        public string TriggerCondition => TriggerData.triggerCondition;
        public TriggerRestriction TriggerRestriction => TriggerData.triggerRestriction;
        public bool Optional => TriggerData.optional;
        public string Blurb => TriggerData.blurb;

        public Trigger(TriggerData triggerData, Effect effect)
        {
            TriggerData = triggerData;
            Effect = effect;
            triggerData.triggerRestriction.Initialize(effect.Game, Source, this);
        }
    }
}