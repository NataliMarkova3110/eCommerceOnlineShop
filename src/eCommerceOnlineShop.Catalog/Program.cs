using eCommerceOnlineShop.Catalog.BLL.Mapping;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.AddProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.DeleteProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProduct;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProducts;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.GetProductsByCategory;
using eCommerceOnlineShop.Catalog.BLL.UseCases.Products.UpdateProduct;
using eCommerceOnlineShop.Catalog.DAL.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddProductCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(AddProductCommand).Assembly);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();