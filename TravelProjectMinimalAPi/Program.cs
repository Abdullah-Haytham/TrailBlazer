using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseAntiforgery();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");

app.MapGet("/get-token", (IAntiforgery antiforgery, HttpContext context) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var tokenInput = $@"<input name=""{token.FormFieldName}"" type=""hidden"" value=""{token.RequestToken}"" />";
    return Results.Content(tokenInput, "text/html");
});

app.MapPost("/add-destination", async ([FromForm] string destination, IAntiforgery antiforgery, HttpContext context) =>
{
    await antiforgery.ValidateRequestAsync(context);
    var form = await context.Request.ReadFormAsync();
    var file = form.Files.GetFile("file");

    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }
    if (destination == "")
    {
        return Results.BadRequest("Destination not Received");
    }

    var fileId = Guid.NewGuid().ToString();
    Directory.CreateDirectory("uploads");
    var filePath = Path.Combine("uploads", fileId + Path.GetExtension(file.FileName));
    using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }
    return Results.NoContent();
});


app.Run();

