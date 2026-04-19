namespace E_commarce_Backend.Dtos.NotificationSettings
{
    public class NotificationSettingsDto
    {
        public bool EmailNotificationsEnabled { get; set; }
        public bool PushNotificationsEnabled { get; set; }
        public bool OrderStatusUpdatesEnabled { get; set; }
        public bool PromotionsEnabled { get; set; }
        public bool ProductRestockAlertsEnabled { get; set; }
    }
}
