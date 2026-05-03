using Microsoft.EntityFrameworkCore;
using WexCardApi.DB;
using WexCardApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddHttpClient<ITreasuryClient, TreasuryClient>();

var app = builder.Build();

// Setup db
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}


app.MapControllers();

app.UseHttpsRedirection();

app.Run();
