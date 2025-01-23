using eCommerceApp.Application.DTOs.Category;

namespace eCommerceApp.Application.DTOs.Product
{
    public class GetCategory : CategoryBase
    {
        public Guid Id { get; set; }
        public ICollection<GetProduct> Products { get; set; } = [];
    }
}
