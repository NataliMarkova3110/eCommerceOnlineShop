using eCommerceOnlineShop.Cart.BLL.Services;
using eCommerceOnlineShop.Cart.Core.Interfaces;
using eCommerceOnlineShop.Cart.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Cart services
builder.Services.AddSingleton<ICartRepository>(sp => new CartRepository(builder.Configuration));
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
