using System.Collections;
using MongoDB.Driver;
using Slider.Interface;
using Slider.Model;
using Mongo.client;
using MongoDB.Bson;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace Slider.Repository;

public class SliderRepository : ISliderRepository
{
    private readonly IConfiguration _config;
    private readonly MongoConnection _connection;

    public SliderRepository(IConfiguration config, MongoConnection connection)
    {
        this._config = config;
        this._connection = connection;
    }

    public async Task<IEnumerable<SlideModel>> GetAllSlides()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<SlideModel>("Get_all_slides", commandType: CommandType.StoredProcedure);

        // var connection = _connection.MakeConnection<SlideModel>("slider");

        // var filter = Builders<SlideModel>.Filter.Eq("_id", new ObjectId("64f5f5e1c0ace9233d07c4fc"));

        // var data = connection.Find((x) => x.Image != null).ToList();

        return data;
    }

    public async Task CreateSlide(SlideModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var extension = Path.GetExtension(payload.Image!.FileName);

        var path = @$"images/slides/{payload.Id}.jpg";

        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await payload.Image.CopyToAsync(stream);
        }

        await connection.ExecuteAsync("Create_slide", new { PText = payload.PText, H1text = payload.H1Text, Id = payload.Id, Creationdate = payload.CreationDate, Banner = payload.Banner, Animation = payload.Animation, BgType = payload.BgType, SideBar = payload.SideBar, Position = payload.Position, Url = payload.Url, Display = payload.Display, IsActionBtn = payload.IsActionBtn, Active = payload.Active }, commandType: CommandType.StoredProcedure);

        // var extension = Path.GetExtension(payload.File!.FileName);
        // var path = @$"images/{payload.ImageId}{extension}";
        // var fileData = new
        // {
        //     extension,
        //     id = payload.ImageId
        // };

        // using var memoryStream = new MemoryStream();
        // await payload.File.CopyToAsync(memoryStream);

        // // Save the image data to MongoDB
        // payload.Image = memoryStream.ToArray();
        // payload.File = null;

        // await connection.InsertOneAsync(payload);
    }
    public async Task UpdateSlide(SlideModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        if (payload.Image != null)
        {
            var extension = Path.GetExtension(payload.Image!.FileName);

            var path = @$"images/slides/{payload.Id}.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await payload.Image.CopyToAsync(stream);
            }
        }
        await connection.ExecuteAsync("Update_slide", new { PText = payload.PText, H1text = payload.H1Text, Id = payload.Id, Url = payload.Url, Display = payload.Display, Active = payload.Active }, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteSlide(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync("Delete_slide", new { Id }, commandType: CommandType.StoredProcedure);
    }
}