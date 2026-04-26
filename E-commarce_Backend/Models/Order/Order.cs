namespace E_commarce_Backend.Models.order
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; } = "pending";

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; }
        public List<OrderStatusHistory> StatusHistory { get; set; } = new();
        public string PaymentRef { get; set; } // Paymob Order ID
    }
}
