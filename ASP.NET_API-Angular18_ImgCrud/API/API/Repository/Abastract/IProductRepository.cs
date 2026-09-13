using API.Models;

namespace API.Repository.Abastract
{
    public interface IProductRepository
    {
        bool Add(Product model);
        IEnumerable<Product> GetAll();
    }
}
