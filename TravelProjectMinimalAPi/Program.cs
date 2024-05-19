using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDbConnection>(_ => new SqliteConnection("Data Source=wwwroot/database/travel.db"));

var app = builder.Build();

app.UseStaticFiles();
app.MapFallbackToFile("/index.html");


app.MapPost("/add-destination", async (HttpContext context, IDbConnection db) =>
{
    var form = await context.Request.ReadFormAsync();
    var file = form.Files.GetFile("file");
    var destinationName = form["destination"];
    var rating = form["rating"];

    Console.WriteLine(rating);
    

    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }
    if (destinationName == "")
    {
        return Results.BadRequest("Destination not Received");
    }

    var fileId = Guid.NewGuid().ToString();
    Directory.CreateDirectory("wwwroot/uploads");
    var filePath = "wwwroot/uploads/" + fileId + Path.GetExtension(file.FileName);
    string relativePath = filePath.Replace("wwwroot/", "");
    using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }

    var sql = "INSERT INTO destinations (Id, Name, Rating, Image) VALUES (@Id, @Name, @Rating, @Image)";
    var rows = await db.ExecuteAsync(sql, new { Id=fileId, Name=destinationName, Rating=rating, Image=relativePath});
    Console.WriteLine("The number of affected rows: " + rows);
    return Results.NoContent();
});

app.MapGet("/get-destinations", async (IDbConnection db) =>
{
    var sql = "SELECT * FROM destinations";
    var destinations = await db.QueryAsync(sql);

    string html = "";
    string starHTML = "<img class=\"star\" src=\"images/star.svg\" alt=\"star\" />";
    string halfStarHTML = "<img class=\"star\" src=\"images/half-star.svg\" alt=\"half-star\" />";
    foreach (var dest in destinations)
    {
        float rating;
        float.TryParse(dest.Rating, out rating);

        StringBuilder starsBuilder = new StringBuilder();
        int wholeStars = (int)rating;
        bool hasHalfStar = (rating - wholeStars) > 0;

        for (int i = 0; i < wholeStars; i++)
        {
            starsBuilder.Append(starHTML);
        }

        if (hasHalfStar)
        {
            starsBuilder.Append(halfStarHTML);
        }

        string stars = starsBuilder.ToString();

        html += $@"
        <div class=""card card-destination"">
            <img src=""{dest.Image}"" class=""card-img-top"" alt=""..."">
            <div class=""card-body"">
                <h5 class=""card-title"">{dest.Name}</h5>
                <p class=""country""></p>
                <span class=""rating"">{stars}</span>
            </div>
        </div>";
    }

    return Results.Content(html);
});

app.MapDelete("/remove-destination/{name}", async (string name, IDbConnection db) =>
{
    var sql = $"SELECT Image FROM destinations WHERE Name=\"{name}\"";
    var destinations = await db.QueryAsync(sql);
    
    foreach(var dest in destinations)
    {
        File.Delete($"{app.Environment.ContentRootPath}/wwwroot/{dest.Image}");
    }

    sql = $"delete from destinations where Name=\"{name}\"";
    var count = await db.ExecuteAsync(sql);

    if (count > 0) { return Results.Ok("destination removed successfully"); }
    return Results.BadRequest("destination doesn't exist");
});


app.Run();


