using AutoMapper;
using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;

namespace E_commarce_Backend.Profiles
{

    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<Cart, CartDto>()
                .ForMember(
                    dest => dest.Total,
                    opt => opt.MapFrom(src =>
                        src.Items.Sum(i => i.Product.Price * i.Quantity)
                    )
                );

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));
        }
    }

}
