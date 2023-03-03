﻿using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class ChangeAllCardStats : ChangeCardStats
    {
        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction()
            {
                cardRestrictions = new string[]
                {
                    CardRestriction.Character,
                    CardRestriction.Board
                }
            };
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext));
            var buff = Buff;
            foreach (var c in targets) c.AddToStats(buff, Effect);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}