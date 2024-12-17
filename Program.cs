using ApiPeliculas.Data;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiPeliculas.PeliculasMappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ApiPeliculas.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext < AplicationDBContext > (options => 
{

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));




});

//Soporte Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>().AddEntityFrameworkStores<AplicationDBContext>();

//Cache
builder.Services.AddResponseCaching();

//Repositorios
builder.Services.AddScoped<ICategoriaRepository,CategoriaRepository>();
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Mapper

builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Configuracion Autenticacion
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer( x=>

        {
            x.RequireHttpsMetadata= false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            { 
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
               ValidateIssuer = false,
               ValidateAudience = false
            };
        } );
 

builder.Services.AddControllers(option =>
    {
        option.CacheProfiles.Add("PerfilVeinteSeg", new CacheProfile() { Duration=20});
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
        "Ejemplo: \"Bearer tkdknkdllskd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//Cors
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build =>
    {

        build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();

    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Soporte CORS
app.UseCors("PolicyCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
