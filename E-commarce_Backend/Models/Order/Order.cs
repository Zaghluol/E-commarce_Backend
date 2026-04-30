namespace E_commarce_Backend.Models.order
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "PendingPayment";

        public string ShippingAddress { get; set; }

        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new();

        public List<OrderStatusHistory> StatusHistory { get; set; } = new();

        public string PaymentRef { get; set; }
    }
}
