namespace KompasServer.Effects
{
    public class AttachSubeffect : ServerSubeffect
    {
        //the index for the card to be attached to.
        //default is two targets ago
        public int attachmentTarget = -2;

        public override bool Resolve()
        {
            var toAttach = Target;
            var attachTo = Effect.GetTarget(attachmentTarget);

            //if everything goes to plan, resolve the next subeffect
            if (toAttach != null && attachTo != null && attachTo.AddAugment(toAttach, Effect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}