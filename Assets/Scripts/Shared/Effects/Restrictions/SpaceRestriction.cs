using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class SpaceRestriction
    {
        public Subeffect Subeffect { get; private set; }
        public GameCard Source { get; private set; }
        public Game Game => Source.Game;
        public Player Controller => Source.Controller;
        public Effect Effect { get; private set; }

        #region space restrictions
        //adjacency
        public const string AdjacentToSource = "Adjacent to Source";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string AdjacentToCoords = "Adjacent to Coords";
        public const string AdjacentToCardRestriction = "Adjacent to a Card that Fits Restriction";

        public const string ConnectedToSourceBy = "Connected to Source by Cards Fitting Restriction";
        public const string ConnectedToSourceBySpaces = "Connected to Source by Spaces Fitting Restriction";
        public const string ConnectedToTargetBy = "Connected to Target by";
        public const string ConnectedToTargetBySpaces = "Connected to Target by Spaces Fitting Restriction";
        public const string ConnectedToTargetByXSpaces = "Connected to Target by X Spaces Fitting Restriction";
        public const string ConnectedToAvatarBy = "Connected to Avatar by";

        public const string InAOE = "In AOE";
        public const string NotInAOE = "Not In AOE";
        public const string InTargetsAOE = "In Target's AOE";
        public const string InAOEOf = "In AOE Of";
        public const string NotInAOEOf = "Not In AOE Of";
        public const string LimitAdjacentCardsFittingRestriction = "Limit Number of Adjacent Cards Fitting Restriction";
        public const string InAOEOfNumberFittingRestriction = "In AOE of Number of Cards Fitting Restriction";
        public const string InAOESourceAlsoIn = "In AOE Source is Also In";

        public const string SubjectiveDisplacementFromSource = "Subjective Displacement from Source";
        public const string BehindSource = "Behind Source";

        //distance
        public const string DistanceToSourceFitsXRestriction = "Distance to Source Fits X Restriction";
        public const string DistanceToTargetFitsXRestriction = "Distance to Target Fits X Restriction";

        public const string FurtherFromSourceThanTarget = "Further from Source than Target";
        public const string FurtherFromSourceThanCoords = "Further from Source than Coords";
        public const string TowardsSourceFromTarget = "Towards Source from Target";
        public const string TowardsTargetFromSource = "Towards Target from Source";
        public const string DirectlyAwayFromTarget = "Directly Away from Target";

        //misc
        public const string CanPlayTarget = "Can Play Target to This Space";
        public const string CanMoveTarget = "Can Move Target to This Space";
        public const string CanMoveSource = "Can Move Source to This Space";
        public const string Empty = "Empty";
        public const string Surrounded = "Surrounded";
        public const string CardHereFitsRestriction = "Card Here Fits Restriction";

        public const string OnSourcesDiagonal = "On Source's Diagonal";
        public const string OnTargetsDiagonal = "On Target's Diagonal";

        public const string OnEdge = "On Edge of Board";
        public const string Corner = "Corner";
        //TODO: eventually make a "targetdirection" subeffect that appends the direction as a Space to the list of coords,
        // then replace these with something comparing directions
        public const string SameDirectionFromTargetAsSpace = "Same Direction From Target As Source From Space";
        public const string OppositeDirectionFromTargetThanSpace = "Opposite Direction From Target Than Space";
        #endregion space restrictions

        public string[] spaceRestrictions;
        public CardRestriction adjacencyRestriction;
        public CardRestriction limitAdjacencyRestriction;
        public int adjacencyLimit;
        public CardRestriction connectednessRestriction;
        public SpaceRestriction spaceConnectednessRestriction;
        public CardRestriction hereFitsRestriction;
        public CardRestriction inAOEOfRestriction;

        public XRestriction distanceXRestriction;
        public XRestriction connectedSpacesXRestriction;
        public XRestriction numberOfCardsInAOEOfRestriction;

        public string[] playRestrictionsToIgnore = new string[0];

        public int constant;

        public int displacementX;
        public int displacementY;

        public string blurb = "";
        public bool mustBeEmpty = true;

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(GameCard source, Player controller, Effect effect)
        {
            Source = source;
            Effect = effect;

            adjacencyRestriction?.Initialize(source, effect);
            connectednessRestriction?.Initialize(source, effect);
            spaceConnectednessRestriction?.Initialize(source, controller, effect);
            limitAdjacencyRestriction?.Initialize(source, effect);
            hereFitsRestriction?.Initialize(source, effect);
            inAOEOfRestriction?.Initialize(source, effect);
            distanceXRestriction?.Initialize(source);
            numberOfCardsInAOEOfRestriction?.Initialize(source);

            initialized = true;
        }

        public void Initialize(Subeffect subeffect)
        {
            Initialize(subeffect.Source, subeffect.Controller, subeffect.Effect);
            Subeffect = subeffect;
            adjacencyRestriction?.Initialize(subeffect);
            connectednessRestriction?.Initialize(subeffect);
            spaceConnectednessRestriction?.Initialize(subeffect);
            limitAdjacencyRestriction?.Initialize(subeffect);
            hereFitsRestriction?.Initialize(subeffect);
            distanceXRestriction?.Initialize(subeffect.Source, subeffect);
        }

        /// <summary>
        /// Whether the given restriction is valid for the given space.
        /// </summary>
        /// <param name="restriction">The restriction to be evaluated</param>
        /// <param name="x">The x coordinate of the space</param>
        /// <param name="y">The y coordinate of the space</param>
        /// <param name="theoreticalTarget">If this space restriction is being considered with a theoretical additional target, this is it</param>
        /// <returns></returns>
        private bool RestrictionValid(string restriction, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            //would use ?? but GameCard inherits from monobehavior which overrides comparison with null
            var target = theoreticalTarget != null ? theoreticalTarget : Subeffect?.Target;

            return restriction switch
            {
                //adjacency
                AdjacentToSource => Source.IsAdjacentTo(space),
                AdjacentToTarget => target?.IsAdjacentTo(space) ?? false,
                AdjacentToCoords => space.AdjacentTo(Subeffect.Space),
                AdjacentToCardRestriction => Game.boardCtrl.CardsAdjacentTo(space).Any(c => adjacencyRestriction.Evaluate(c, context)),

                ConnectedToSourceBy => Game.boardCtrl.ShortestPath(Subeffect.Source, space, connectednessRestriction, context) < 50,
                ConnectedToSourceBySpaces => Game.boardCtrl.ShortestPath(Subeffect.Source.Position, space, s => spaceConnectednessRestriction.Evaluate(s, context)) < 50,
                ConnectedToTargetBy => Game.boardCtrl.ShortestPath(target, space, connectednessRestriction, context) < 50,
                ConnectedToTargetBySpaces => Game.boardCtrl.ShortestPath(target.Position, space, s => spaceConnectednessRestriction.Evaluate(s, context)) < 50,
                ConnectedToTargetByXSpaces => connectedSpacesXRestriction.Evaluate(Game.boardCtrl.ShortestPath(target.Position, space,
                                                s => spaceConnectednessRestriction.Evaluate(s, context))),
                ConnectedToAvatarBy => Game.boardCtrl.ShortestPath(Source.Controller.Avatar, space, connectednessRestriction, context) < 50,

                InAOE => Source.SpaceInAOE(space),
                NotInAOE => !Source.SpaceInAOE(space),
                InTargetsAOE => target.SpaceInAOE(space),
                InAOEOf => Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context)),
                NotInAOEOf => !Game.Cards.Any(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context)),
                InAOEOfNumberFittingRestriction => numberOfCardsInAOEOfRestriction.Evaluate(Game.Cards.Count(c => c.SpaceInAOE(space) && inAOEOfRestriction.Evaluate(c, context))),
                LimitAdjacentCardsFittingRestriction => Game.boardCtrl.CardsAdjacentTo(space)
                                                                        .Where(c => limitAdjacencyRestriction.Evaluate(c, context))
                                                                        .Count() <= adjacencyLimit,
                InAOESourceAlsoIn => Game.Cards.Any(c => c.SpaceInAOE(space) && c.CardInAOE(Source)),

                SubjectiveDisplacementFromSource => Controller.SubjectiveCoords(space).DisplacementTo(Controller.SubjectiveCoords(Source.Position)) == (displacementX, displacementY),
                BehindSource => Source.SpaceBehind(space),

                //distance
                DistanceToSourceFitsXRestriction => distanceXRestriction.Evaluate(Source.DistanceTo(space)),
                DistanceToTargetFitsXRestriction => distanceXRestriction.Evaluate(target.DistanceTo(space)),

                FurtherFromSourceThanTarget => Source.DistanceTo(space) > Source.DistanceTo(target),
                FurtherFromSourceThanCoords => Source.DistanceTo(space) > Source.DistanceTo(Subeffect.Space),

                TowardsSourceFromTarget => Source.DistanceTo(space) < Source.DistanceTo(target),
                TowardsTargetFromSource => target.DistanceTo(space) < target.DistanceTo(Source),

                DirectlyAwayFromTarget => target.SpaceDirectlyAwayFrom(space, Source),

                //misc
                CanPlayTarget => target.PlayRestriction.EvaluateEffectPlay(space, Subeffect.Effect, Subeffect.Player, context, ignoring: playRestrictionsToIgnore),
                CanMoveTarget => target.MovementRestriction.EvaluateEffectMove(space),
                CanMoveSource => Source.MovementRestriction.EvaluateEffectMove(space),

                Empty => Game.boardCtrl.GetCardAt(space) == null,
                Surrounded => Game.boardCtrl.Surrounded(space),
                CardHereFitsRestriction => hereFitsRestriction.Evaluate(Game.boardCtrl.GetCardAt(space), context),
                OnSourcesDiagonal => Source.SameDiagonal(space),
                OnTargetsDiagonal => target.SameDiagonal(space),
                OnEdge => space.IsEdge,
                Corner => space.IsCorner,
                SameDirectionFromTargetAsSpace => target.Position.DirectionFromThisTo(space) == Subeffect.Space.DirectionFromThisTo(Source.Position),
                OppositeDirectionFromTargetThanSpace => Source.Position.DirectionFromThisTo(space) * -1 == Source.Position.DirectionFromThisTo(Subeffect.Space),

                _ => throw new ArgumentException($"Invalid space restriction {restriction}", "restriction"),
            };
        }

        private bool RestrictionValidWithDebug(string r, Space space, GameCard theoreticalTarget, ActivationContext context)
        {
            bool success = RestrictionValid(r, space, theoreticalTarget, context);
            if (!success) Debug.Log($"Space resetriction {r} was flouted by {space}");
            return success;
        }

        public bool Evaluate(Space space, ActivationContext context, GameCard theoreticalTarget = null)
        {
            if (!initialized) throw new ArgumentException("Space restriction not initialized!");
            if (!space.Valid) return false;
            if (mustBeEmpty && Game.boardCtrl.GetCardAt(space) != null)
            {
                //Debug.Log($"Space for {Source.CardName} needed to be empty and wasn't.");
                return false;
            }

            return spaceRestrictions.All(r => RestrictionValidWithDebug(r, space, theoreticalTarget, context));
        }

        public override string ToString() => JsonUtility.ToJson(this);
    }
}