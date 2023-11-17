using System.Collections;
using MongoDB.Driver;
using Slider.Interface;
using Slider.Model;
using Mongo.client;
using MongoDB.Bson;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using CredentialsHandler;
using System.Text.Json;
using AES;
using System.Text;
using Newtonsoft.Json.Linq;
using Credentials.Models;
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













    // public async Task<dynamic> AdminAuth(AdminDto payload)
    // {

    //     HttpClient client = new();

    //     var cred = new CredHandler(_config);

    //     var credData = await cred.MakeContract();

    //     // Console.Write(credData?[0]);

    //     var token = new
    //     {
    //         tk = credData?[0],
    //         src = "AS-IN-D659B-e3M",
    //     };

    //     var body = new
    //     {
    //         UsN = payload.Id,
    //         Pwd = payload.Password,
    //         xAppSource = "AS-IN-D659B-e3M"
    //     };

    //     var jsonBody = JsonSerializer.Serialize(body);

    //     var encryptedBody = AEShandler.Encrypt(jsonBody, credData?[1], credData?[2]);

    //     byte[] bodyBytes = Convert.FromBase64String(encryptedBody);

    //     string bodyHexString = BitConverter.ToString(bodyBytes).Replace("-", "").ToLower();

    //     var jsonHeader = JsonSerializer.Serialize(token);

    //     var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

    //     byte[] bytes = Convert.FromBase64String(encryptedHeader);

    //     string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

    //     using StringContent jsonContent = new(
    //     bodyHexString,
    //     Encoding.UTF8,
    //     "application/json");

    //     client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

    //     using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("E360:Signin_url"), jsonContent);

    //     var resData = await response.Content.ReadAsStringAsync();

    //     var jsonData = JObject.Parse(resData);

    //     if (jsonData.Value<string>("status") == "200")
    //     {
    //         byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

    //         string bytes64 = Convert.ToBase64String(stringBytes);

    //         var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

    //         var data = JsonSerializer.Deserialize<dynamic>(decrypted);

    //         var res = new
    //         {
    //             code = 200,
    //             message = "Successful",
    //             data
    //         };

    //         return res;
    //     }
    //     else if (jsonData.Value<string>("status") != "200")
    //     {
    //         // Console.WriteLine("something went wrong");
    //         var res = new
    //         {
    //             code = 400,
    //             message = jsonData.Value<string>("message_description"),
    //         };
    //         return res;
    //     }

    //     return Array.Empty<IEnumerable<dynamic>>();
    // }
}