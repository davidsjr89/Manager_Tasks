using Api.Models;
using Api.Token;
using AutoMapper;
using Domain.Interfaces;
using Domain.Interfaces.Generics;
using Domain.Interfaces.InterfaceServices;
using Domain.Services;
using Entities.Model;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generics;
using Infrastructure.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Manager Tasks",
                        Version = "v1",
                        Description = "Faz o gerenciamento de tarefas, podendo (adicionar, remover, atualizar e listar). Conseguimos adicionar usuários e gerenciar seu acessos",
                        Contact = new OpenApiContact 
                        {
                            Name = "David da Silva Júnior",
                            Email = "david.sjr89@gmail.com",
                            Url = new Uri("https://github.com/davidsjr89"),
                        }
                    }
            );

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"Primeiro passo você precisa fazer o login e copiar o token.
                        JWT Cabeçalho de autorização usando o esquema Bearer.
                        Entre 'Bearer'[espaço] digite seu token na entrada de texto abaixo.
                        Exemplo: Bearer 12345abcdef",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
});

//Configure Services
builder.Services.AddDbContext<ContextBase>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ContextBase>();

// Interface and Repositories
builder.Services.AddSingleton(typeof(IGeneric<>), typeof(RepositoryGenerics<>));
builder.Services.AddSingleton<ITasks, RepositoryTasks>();

//Services domain
builder.Services.AddSingleton<IServiceTasks, ServiceTasks>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "Manager_Tasks.Security.Bearer",
            ValidAudience = "Manager_Tasks.Security.Bearer",
            IssuerSigningKey = JwtSecurityKey.Create("eX|Isq-G|F8f(4_+QA;p8uAzQn>&W/ijbSPx7}JR&4<X9O,V01s==2;(UB;IJ`9")
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                return Task.CompletedTask;
            }
        };
    });

var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<TasksAddViewModel, Tasks>().ReverseMap();
    cfg.CreateMap<TasksViewModel, Tasks>().ReverseMap();
});

IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
