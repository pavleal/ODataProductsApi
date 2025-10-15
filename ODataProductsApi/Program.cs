using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using ODataProductsApi.Data;
using ODataProductsApi.Dtos;
using ODataProductsApi.Models;
using ODataProductsApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("ProductsDb"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Controller + OData
builder.Services.AddControllers().AddOData(opt =>
{
    var edm = new ODataConventionModelBuilder();
    edm.EntitySet<ProductDto>("Products");
    edm.EntitySet<CategoryDto>("Categories");

    opt.AddRouteComponents("odata", edm.GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .SetMaxTop(100)
        .Count();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();