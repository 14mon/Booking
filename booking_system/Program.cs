


using booking_system.Data;
using booking_system.Models;
using booking_system.Redis;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<User>("Users");
    builder.EntitySet<Class>("Classes");
    builder.EntitySet<ClassBooking>("ClassBookings");
    builder.EntitySet<Country>("Countries");
    builder.EntitySet<GatewayRawEvent>("GatewayRawEvents");
    builder.EntitySet<Package>("Packages");
    builder.EntitySet<PaymentGateway>("PaymentGateways");
    builder.EntitySet<Refund>("Refunds");
    builder.EntitySet<Transaction>("Transactions");
    builder.EntitySet<UserCreditHistory>("UserCreditHistories");


    return builder.GetEdmModel();
}
var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(
        builder.Environment.IsDevelopment()
            ? "appsettings.Development.json"
            : "appsettings.Production.json",
        optional: false,
        reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

string? redisReadConnectionString = Environment.GetEnvironmentVariable("REDIS_READ_CONNECTION_STRING");
string? redisWriteConnectionString = Environment.GetEnvironmentVariable("REDIS_WRITE_CONNECTION_STRING");
string? redisInstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME");

builder.Services.AddSingleton(sp => new RedisService(redisReadConnectionString!, redisWriteConnectionString!, redisInstanceName!));

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase.json"),
});


builder.Services.AddControllers().AddOData(opt => opt
                                            .AddRouteComponents("v1", GetEdmModel())
                                            .Filter()
                                            .Count()
                                            .OrderBy()
                                            .Select()
                                            .Expand()
                                            .SetMaxTop(100)
                                           );

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.Run();


