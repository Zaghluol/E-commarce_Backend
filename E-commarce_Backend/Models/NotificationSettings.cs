using System.ComponentModel.DataAnnotations;

namespace E_commarce_Backend.Models
{
    public class NotificationSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool PushNotificationsEnabled { get; set; } = true;
        public bool OrderStatusUpdatesEnabled { get; set; } = true;
        public bool PromotionsEnabled { get; set; } = true;
        public bool ProductRestockAlertsEnabled { get; set; } = true;
    }
}
