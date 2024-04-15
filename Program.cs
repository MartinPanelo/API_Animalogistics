using API_Animalogistics.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.WebHost.UseUrls("http://192.168.0.*:5014","http://localhost:5015");

// Add services to the container.
builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddAuthentication().AddJwtBearer(options =>//la api web valida con token
    {
		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["TokenAuthentication:Issuer"],
			ValidAudience = configuration["TokenAuthentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
				configuration["TokenAuthentication:SecretKey"])),
		};});

builder.Services.AddDbContext<DataContext>(
	options => options.UseMySql(
		configuration["ConnectionDB:MySql"],
		ServerVersion.AutoDetect(configuration["ConnectionDB:MySql"])
	)
);

builder.Services.AddAuthorization(options =>
{
	//options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
	options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
	options.AddPolicy("Eventos", policy => policy.RequireRole("Eventos"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Data")),
    RequestPath = "/Data"
});
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



app.Run();

