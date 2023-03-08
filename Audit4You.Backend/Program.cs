using Audit4You.Backend.Ports.Components;
using Audit4You.Backend.Ports.Services;
using Audit4You.Backend.Repositories;
using Audit4You.Backend.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader();
                      });
});
// Add services to the container.
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IXlsxToWholeClient, XlsxToWholeClientEPPlus>();
builder.Services.AddScoped<IMonetaryUnitSampling, MonetaryUnitSampling>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseMiddleware<ErrorHandling>();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();