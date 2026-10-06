using AbmProductos.Entities;
using AbmProductos.Models.DTOs.Requests;
using AbmProductos.Models.DTOs.Responses;
using AbmProductos.Repositories.Interfaces;
using AbmProductos.Services.Interfaces;

namespace AbmProductos.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public List<ProductForReadDto> GetAllProducts()
        {
            return _repository.GetAllProducts().Select(MapToReadDto).ToList();
        }

        public ProductForReadDto? GetProductById(int id)
        {
            var product = _repository.GetProductById(id);
            if (product == null)
            {
                return null;
            }
            return MapToReadDto(product);
        }

        public ProductForReadDto CreateProduct(ProductForCreateDto dto)
        {
            if (NameExists(dto.Name, null))
            {
                throw new InvalidOperationException($"Ya existe un producto con el nombre '{dto.Name}'.");
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            _repository.AddProduct(product);
            return MapToReadDto(product);
        }

        public void UpdateProduct(int id, ProductForUpdateDto dto)
        {
            var product = _repository.GetProductById(id);
            if (product == null)
            {
                return;
            }

            if (NameExists(dto.Name, id))
            {
                throw new InvalidOperationException($"Ya existe un producto con el nombre '{dto.Name}'.");
            }

            var updated = new Product
            {
                Id = id,
                Name = dto.Name,
                Price = dto.Price
            };

            _repository.UpdateProduct(updated);
        }

        public void DeleteProduct(int id)
        {
            var product = _repository.GetProductById(id);
            if (product == null)
            {
                return;
            }
            _repository.DeleteProduct(product);
        }

        public List<ProductForReadDto> SearchProductsByName(string name)
        {
            return _repository.SearchProductsByName(name).Select(MapToReadDto).ToList();
        }

        public ProductStatsDto GetStats()
        {
            var products = _repository.GetAllProducts();

            if (products.Count == 0)
            {
                return new ProductStatsDto
                {
                    Total = 0,
                    AveragePrice = 0,
                    MostExpensiveName = string.Empty
                };
            }

            return new ProductStatsDto
            {
                Total = products.Count(),
                AveragePrice = Math.Round(products.Average(p => p.Price), 2),
                MostExpensiveName = products.OrderByDescending(p => p.Price).First().Name
            };
        }

        private bool NameExists(string name, int? excludedId)
        {
            return _repository.GetAllProducts()
                .Any(p => p.Id != excludedId && string.Equals(p.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private static ProductForReadDto MapToReadDto(Product product)
        {
            return new ProductForReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }
    }
}
