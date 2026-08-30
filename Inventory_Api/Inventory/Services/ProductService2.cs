using Inventory.Data;
using Inventory.DTOs;
using Inventory.Models;
using Inventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Inventory.Services
{
    public class ProductService2 : IProductService2
    {
        private readonly IProductRepository2 _productRepository;

        public ProductService2(IProductRepository2 productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Product name is required.");
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("Product price cannot be negative.");
            }

            if (product.Stock < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative.");
            }

            return await _productRepository.CreateAsync(product);
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            if (product.Id <= 0)
            {
                throw new ArgumentException("A valid product ID is required.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Product name is required.");
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("Product price cannot be negative.");
            }

            if (product.Stock < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative.");
            }

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("A valid product ID is required.");
            }

            return await _productRepository.DeleteAsync(id);
        }
    }
}
