using KompasCore.Effects;

public class TriggerData
{
	public string triggerCondition;
	public IRestriction<TriggeringEventContext> triggerRestriction;

	public bool optional = false;
	public string blurb = "Trigger";
	public bool showX = false;
	public int orderPriority = 0; //positive means it goes on the stack after anything, negative before
}