using LiteDB;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.Core.Models;

namespace eCommerceOnlineShop.Cart.DAL.Repositories
{
    public class CartRepository : ICartRepository, IDisposable
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<CartEntity> _collection;
        private bool _disposed;

        public CartRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("LiteDb");
            _database = new LiteDatabase(connectionString);
            _collection = _database.GetCollection<CartEntity>("carts");
        }

        public Task<CartEntity?> GetCartAsync(string cartKey)
        {
            ThrowIfDisposed();
            var cart = _collection.FindOne(c => c.CartKey == cartKey);
            return Task.FromResult<CartEntity?>(cart);
        }

        public Task<IEnumerable<CartEntity>> GetAllCartsAsync()
        {
            ThrowIfDisposed();
            var carts = _collection.FindAll();
            return Task.FromResult(carts);
        }

        public Task CreateCartAsync(CartEntity cart)
        {
            ThrowIfDisposed();
            _collection.Insert(cart);
            return Task.CompletedTask;
        }

        public Task UpdateCartAsync(CartEntity cart)
        {
            ThrowIfDisposed();
            _collection.Update(cart);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteCartAsync(string cartKey)
        {
            ThrowIfDisposed();
            var deleted = _collection.DeleteMany(c => c.CartKey == cartKey) > 0;
            return Task.FromResult(deleted);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _database?.Dispose();
                }

                // Set large fields to null
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}
