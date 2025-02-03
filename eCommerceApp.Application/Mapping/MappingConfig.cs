using AutoMapper;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Domain.Entities;

namespace eCommerceApp.Application.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CreateCategory, Category>();
            CreateMap<CreateProduct, Product>();
            CreateMap<UpdateProduct, Product>();
            CreateMap<UpdateCategory, Category>();
            CreateMap<GetCategory, Category>();


            CreateMap<Category , CreateCategory>();
            CreateMap<Category , GetCategory>();
            CreateMap<Product , CreateProduct>();
            CreateMap<Product , GetProduct>();
        }
    }
}
