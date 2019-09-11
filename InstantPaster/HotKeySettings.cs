namespace InstantPaster
{
    internal class HotKeySettings
    {
        public string Combination { get; }
        public string Description { get; }
        public ActionType ActionType { get; }
        public string ActionContent { get; }

        public HotKeySettings(string _combination, string _description, ActionType _actionType, string _actionContent)
        {
            Combination = _combination;
            Description = _description;
            ActionType = _actionType;
            ActionContent = _actionContent;
        }
    }
}