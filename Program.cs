using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Skii.Binders;
using Skii.Data;
using Skii.Extensions;
namespace Skii;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("Db"));
        builder.Services.AddValidation();
        builder.Services.AddCustomServices();
        builder.Services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new CreateAnswerBinderProvider());
            options.ModelBinderProviders.Insert(0, new UpdateAnswerBinderProvider());
        });
        builder.Services.AddAuth(builder.Configuration);

        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        builder.Services.AddDocs();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Logging.AddConsole();
        
        var app = builder.Build();

       
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors("AllowAll");
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Code Vault API")
                    .WithTheme(ScalarTheme.Default) 
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                

            });
            app.UseStaticFiles();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

   
}