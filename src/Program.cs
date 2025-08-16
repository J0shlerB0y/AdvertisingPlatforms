using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Builder;
using System.Text;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IAdvertisingPlatformService, AdvertisingPlatformService>();
            var app = builder.Build();

            app.UseHttpsRedirection();


            app.Map("/", async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync("wwwroot/html/index.html");
            });

            app.MapGet("/{**location:minlength(1)}", async (string location, IAdvertisingPlatformService platformService) =>
                {
                    if (string.IsNullOrWhiteSpace(location))
                    {
                        return Results.BadRequest("Location not specified");
                    }
    
                    var fullPath = "/" + location;
    
                    var platforms = platformService.FindPlatformsForLocation(fullPath);
    
                    if (!platforms.Any())
                    {
                        return Results.NotFound($"For location '{fullPath}' no advertising sites found");
                    }
    
                    return Results.Ok(platforms);
                })
                .WithName("GetPlatformsByLocation")
                .WithSummary("Search for advertising platforms for a given location"); 

            app.MapPost("/upload", async (HttpContext context, IAdvertisingPlatformService platformService) =>
                {
                    IFormFile file = context.Request.Form.Files.FirstOrDefault();
                    if (file == null || file.Length == 0)
                    {
                        return Results.BadRequest("File not provided or empty");
                    }

                    try
                    {
                        await platformService.LoadPlatformsAsync(file.OpenReadStream());
                        return Results.Ok("Data updated successfully");
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem($"An error occurred while processing the file: {ex.Message}");
                    }
                })
                .WithName("UploadPlatforms")
                .WithSummary("Loading advertising platforms from a file")
                .DisableAntiforgery();

            app.Run();
        }
    }
}