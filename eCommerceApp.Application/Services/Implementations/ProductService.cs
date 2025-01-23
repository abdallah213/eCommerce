using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations
{
    public class ProductService(IGeneric<Product> productRepository , IMapper mapper) : IProductService
    {
        private readonly IGeneric<Product> _productRepo = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ServiceResponse> AddAsync(CreateProduct product)
        {
            var mappedData = _mapper.Map<Product>(product);

            var result = await _productRepo.AddAsync(mappedData);

            return result > 0 ?
                 new ServiceResponse(true, "Product Added!") :
                 new ServiceResponse(true, "Product failed to be Added");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            var result = await _productRepo.DeleteAsync(id);

            return result > 0 ?
                 new ServiceResponse(true , "Product Deleted!"):
                 new ServiceResponse(false, "Product failed to be deleted");
        }

        public async Task<IEnumerable<GetProduct>> GetAllAsync()
        {
            var products = await _productRepo.GetAllAsync();
            if (!products.Any())
                return [];

            return _mapper.Map<IEnumerable<GetProduct>>(products);
        }

        public async Task<GetProduct> GetByIdAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if(product == null)
                return new GetProduct();

            return _mapper.Map<GetProduct>(product);
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateProduct product)
        {
            var mappedData = _mapper.Map<Product>(product);

            var result = await _productRepo.UpdateAsync(mappedData);

            return result > 0 ?
                 new ServiceResponse(true, "Product Updated!") :
                 new ServiceResponse(true, "Product failed to be Updated");
        }
    }
}
