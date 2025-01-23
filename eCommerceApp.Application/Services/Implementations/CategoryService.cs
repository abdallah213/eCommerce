using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Interfaces;

namespace eCommerceApp.Application.Services.Implementations
{
    public class CategoryService(IGeneric<Category> categoryRepository, IMapper mapper) : ICategoryService
    {
        private readonly IGeneric<Category> _categoryRepo = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ServiceResponse> AddAsync(CreateCategory category)
        {
            var mappedData = _mapper.Map<Category>(category);

            var result = await _categoryRepo.AddAsync(mappedData);

            return result > 0 ?
                 new ServiceResponse(true, "category Added!") :
                 new ServiceResponse(true, "category failed to be Added");
        }

        public async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            var result = await _categoryRepo.DeleteAsync(id);

            return result > 0 ?
                 new ServiceResponse(true, "category Deleted!") :
                 new ServiceResponse(false, "category failed to be deleted");
        }

        public async Task<IEnumerable<GetCategory>> GetAllAsync()
        {
            var categores = await _categoryRepo.GetAllAsync();
            if (!categores.Any())
                return [];

            return _mapper.Map<IEnumerable<GetCategory>>(categores);
        }

        public async Task<GetCategory> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                return new GetCategory();

            return _mapper.Map<GetCategory>(category);
        }

        public async Task<ServiceResponse> UpdateAsync(UpdateCategory category)
        {
            var mappedData = _mapper.Map<Category>(category);

            var result = await _categoryRepo.UpdateAsync(mappedData);

            return result > 0 ?
                 new ServiceResponse(true, "category Updated!") :
                 new ServiceResponse(true, "category failed to be Updated");
        }
    }
}
