using BlogLab.Identity;
using BlogLab.Models.Account;
using BlogLab.Models.Settings;
using BlogLab.Repository;
using BlogLab.Services;
using BlogLab.Web.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("CloudinaryOptions"));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<IBlogRepository,BlogRepository>();
builder.Services.AddScoped<IBlogCommentRepository,BlogCommentRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();

builder.Services.AddIdentityCore<ApplicationUserIdentity>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
})
    .AddUserStore<UserStore>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<ApplicationUserIdentity>>();

builder.Services.AddControllers();
builder.Services.AddCors();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer
    (
      options =>
      {
          options.RequireHttpsMetadata = false;
          options.SaveToken = true;
          options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = builder.Configuration["Jwt:Issuer"],
              ValidAudience = builder.Configuration["Jwt:Issuer"],
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
              ClockSkew = TimeSpan.Zero
          };
      }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.ConfigureExceptionHandler();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}
else
{
    app.UseCors();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.MapRazorPages();

app.Run();
