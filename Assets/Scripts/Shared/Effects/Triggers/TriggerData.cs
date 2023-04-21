using KompasCore.Effects.Restrictions.TriggerRestrictionElements;

public class TriggerData
{
    public string triggerCondition;
    public AllOf triggerRestriction;

    public bool optional = false;
    public string blurb = "Trigger";
    public bool showX = false;
    public int orderPriority = 0; //positive means it goes on the stack after anything, negative before
}