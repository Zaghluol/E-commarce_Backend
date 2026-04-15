namespace E_commarce_Backend.Dtos.Coupon
{
    public class ValidateCouponDto
    {
        public string Code { get; set; } = null!;
        public decimal CartTotal { get; set; }
    }
}
