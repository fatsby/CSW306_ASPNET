using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Lab3 API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers().AddJsonOptions(options => {
    // tells the serializer to stop when it detects a loop.
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("ActiveUserOnly", policy =>
        policy.RequireClaim("IsActive", "True")
    );

    options.AddPolicy("AdminOrLibrarian", policy =>
        policy.RequireRole("Admin", "Librarian")
    );

    options.AddPolicy("MinimumMembership", policy =>
        policy.RequireAssertion(context => {
            var dateClaim = context.User.FindFirst("CreatedDate");

            if (dateClaim == null) return false;

            if (DateTime.TryParse(dateClaim.Value, out DateTime createdDate)) {
                return createdDate <= DateTime.Now.AddDays(-30);
            }

            return false;
        }));

    options.AddPolicy("ManageActiveCategories", policy =>
        policy.RequireClaim("CanManageCategories", "True")
            .RequireRole("Admin")
    );

    options.AddPolicy("VerifiedEmailOnly", policy =>
        policy.RequireClaim("EmailConfirmed", "True")
    );
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy => {
                          // "http://127.0.0.1:5500" is the default for Live Server
                          // "http://localhost:3000" is common for React
                          policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
