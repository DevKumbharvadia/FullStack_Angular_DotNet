using API.Models;
using API.Repository.Abastract;
using Microsoft.AspNetCore.Mvc;
using ProductMiniApi.Models.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase // Changed from Controller to ControllerBase for API conventions
    {
        private readonly IFileService _fileService;
        private readonly IProductRepository _productRepo;
        private readonly ProductDbContext _dbContext;

        public ProductController(IFileService fs, IProductRepository productRepo, ProductDbContext dbContext)
        {
            _fileService = fs;
            _productRepo = productRepo;
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Add([FromForm] Product model)
        {
            var status = new Status();

            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass valid data";
                return Ok(status);
            }

            if (model.ImageFile != null)
            {
                var fileResult = _fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 1)
                {
                    model.ProductImage = fileResult.Item2; // Set image file name
                }
                else
                {
                    status.StatusCode = 0;
                    status.Message = "Error saving image";
                    return Ok(status);
                }

                var productResult = _productRepo.Add(model);
                if (productResult)
                {
                    status.StatusCode = 1;
                    status.Message = "Product added successfully";
                }
                else
                {
                    status.StatusCode = 0;
                    status.Message = "Error adding product";
                }
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Image file is required";
            }

            return Ok(status);
        }

        [HttpGet] // Added HttpGet attribute
        public IActionResult GetAll()
        {
            var products = _productRepo.GetAll();
            return Ok(products);
        }
    }
}
