using API.Models;
using API.Repository.Abastract;
using API.Repository.Abastract;

namespace API.Repository.Implementation
{
    public class ProductRepostory : IProductRepository
    {
		private readonly ProductDbContext _context;
		public ProductRepostory(ProductDbContext context)
		{
			this._context = context;
		}
        public bool Add(Product model)
        {
			try
			{
				_context.Products.Add(model);
				_context.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{

				return false;
			}
        }

        public IEnumerable<Product> GetAll()
		{
			return _context.Products.ToList();
		}


    }
}
