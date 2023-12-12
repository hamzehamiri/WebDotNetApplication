using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication1.DB;
using WebApplication1.DB.Entities;
using WebApplication1.Exceptions;
using WebApplication1.Security;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;
    var env = builder.Environment;

    if (env.IsProduction())
    {
        services.AddDbContext<DataContext>();
    }
    else
    {
        services.AddDbContext<DataContext, SqliteDataContext>();
    }

    services.AddCors();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        //.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options))
        //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => builder.Configuration.Bind("CookieSettings", options));
    services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id=JwtBearerDefaults.AuthenticationScheme
                    }
                },
                new string[]{}
            }
        });
    });
    //services.AddSwaggerGen(c =>
    //{
    //    var jwtSecurityScheme = new OpenApiSecurityScheme
    //    {
    //        BearerFormat = "JWT",
    //        Name = "JWT Authentication",
    //        In = ParameterLocation.Header,
    //        Type = SecuritySchemeType.Http,
    //        Scheme = JwtBearerDefaults.AuthenticationScheme,
    //        Description = "Put Token In This Box",

    //        Reference = new OpenApiReference
    //        {
    //            Id = JwtBearerDefaults.AuthenticationScheme,
    //            Type = ReferenceType.SecurityScheme
    //        }
    //    };
    //    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //    {
    //        { jwtSecurityScheme, Array.Empty<string>() }
    //    });
    //});

    services.AddAutoMapper(typeof(Program));
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();

}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}

{
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseMiddleware<JwtMiddleware>();
    app.MapControllers();
    app.UseAuthorization();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

{
    //var testusers = new list<user>
    //{
    //    new() { id = 1, firstname = "admin", lastname = "user", username = "admin", passwordhash = bcryptnet.hashpassword("admin"), role = role.admin },
    //    new() { id = 2, firstname = "normal", lastname = "user", username = "user", passwordhash = bcryptnet.hashpassword("user"), role = role.user }
    //};

    //using var scope = app.services.createscope();
    //var datacontext = scope.serviceprovider.getrequiredservice<datacontext>();
    //datacontext.users.addrange(testusers);
    //datacontext.savechanges();
}


app.Run();
