using LiteDB;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.DAL.Repositories
{
    public class CartRepository : ICartRepository, IDisposable
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<CartEntity> _collection;

        public CartRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LiteDb");
            _database = new LiteDatabase(connectionString);
            _collection = _database.GetCollection<CartEntity>("carts");
        }

        public Task<CartEntity> GetCartAsync(Guid cartId)
        {
            var cart = _collection.FindOne(c => c.Id == cartId);
            return Task.FromResult(cart);
        }

        public Task<CartEntity> CreateCartAsync(CartEntity cart)
        {
            _collection.Insert(cart);
            return Task.FromResult(cart);
        }

        public Task<CartEntity> UpdateCartAsync(CartEntity cart)
        {
            _collection.Update(cart);
            return Task.FromResult(cart);
        }

        public Task<bool> DeleteCartAsync(Guid cartId)
        {
            var deleted = _collection.DeleteMany(c => c.Id == cartId) > 0;
            return Task.FromResult(deleted);
        }

        public void Dispose()
        {
            _database?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}