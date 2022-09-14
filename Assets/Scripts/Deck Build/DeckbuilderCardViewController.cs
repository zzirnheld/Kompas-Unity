using KompasCore.UI;

namespace KompasDeckbuilder.UI
{
    public class DeckbuilderCardViewController : TypicalCardViewController
    {
        public DeckbuilderReminderTextsParentController reminderTextsParentController;
        public override IReminderTextParentController ReminderTextsUIController => reminderTextsParentController;
    }
}