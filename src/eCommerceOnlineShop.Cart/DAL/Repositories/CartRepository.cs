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

        public Task<CartEntity?> GetCartAsync(string cartKey)
        {
            var cart = _collection.FindOne(c => c.CartKey == cartKey);
            return Task.FromResult<CartEntity?>(cart);
        }

        public Task<IEnumerable<CartEntity>> GetAllCartsAsync()
        {
            var carts = _collection.FindAll();
            return Task.FromResult(carts);
        }

        public Task CreateCartAsync(CartEntity cart)
        {
            _collection.Insert(cart);
            return Task.CompletedTask;
        }

        public Task UpdateCartAsync(CartEntity cart)
        {
            _collection.Update(cart);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteCartAsync(string cartKey)
        {
            var deleted = _collection.DeleteMany(c => c.CartKey == cartKey) > 0;
            return Task.FromResult(deleted);
        }

        public void Dispose()
        {
            _database?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
