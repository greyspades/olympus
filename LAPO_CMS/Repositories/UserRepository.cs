using System.Collections;
using MongoDB.Driver;
using User.Interface;
using Article.Model;
using Mongo.client;
using MongoDB.Bson;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Credentials.Models;
using CredentialsHandler;
using System.Text.Json;
using AES;
using System.Text;
using Newtonsoft.Json.Linq;

namespace User.Repository;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _config;
    private readonly MongoConnection _connection;

    public UserRepository(IConfiguration config, MongoConnection connection)
    {
        this._config = config;
        this._connection = connection;
    }
    public async Task<dynamic> AdminAuth(AdminDto payload)
    {
        HttpClient client = new();

        var cred = new CredHandler(_config);

        var credData = await cred.MakeContract();

        var token = new
        {
            tk = credData?[0],
            src = "AS-IN-D659B-e3M",
        };

        var body = new
        {
            UsN = payload.Id,
            Pwd = payload.Password,
            xAppSource = "AS-IN-D659B-e3M"
        };

        var jsonBody = JsonSerializer.Serialize(body);

        var encryptedBody = AEShandler.Encrypt(jsonBody, credData?[1], credData?[2]);

        byte[] bodyBytes = Convert.FromBase64String(encryptedBody);

        string bodyHexString = BitConverter.ToString(bodyBytes).Replace("-", "").ToLower();

        var jsonHeader = JsonSerializer.Serialize(token);

        var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

        byte[] bytes = Convert.FromBase64String(encryptedHeader);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        using StringContent jsonContent = new(
        bodyHexString,
        Encoding.UTF8,
        "application/json");

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

        using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("E360:Signin_url"), jsonContent);

        var resData = await response.Content.ReadAsStringAsync();

        var jsonData = JObject.Parse(resData);

        // Console.WriteLine(jsonData);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

            var data = JsonSerializer.Deserialize<dynamic>(decrypted);

            var res = new
            {
                code = 200,
                message = "Successful",
                data
            };

            return res;
        }
        else if (jsonData.Value<string>("status") != "200")
        {
            // Console.WriteLine("something went wrong");
            var res = new
            {
                code = 400,
                message = jsonData.Value<string>("message_description"),
            };
            return res;
        }

        return Array.Empty<IEnumerable<dynamic>>();
    }

    public async Task<IEnumerable<UserData>> GetAdmin(string Id) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<UserData>("Get_admin", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task CreateAdmin(UserData payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_user", new {Id = payload.Id, StaffId = payload.StaffId, Name = payload.Name, Role = payload.Role }, commandType: CommandType.StoredProcedure);
    }

    //   public async Task CreateArticle(UserData payload) {
    //     using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    //     var extension = Path.GetExtension(payload.Image!.FileName);

    //     var path = @$"images/articles/{payload.Id}.jpg";

    //     using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
    //     {
    //         await payload.Image.CopyToAsync(stream);
    //     }

    //     await connection.ExecuteAsync("Create_article", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph, CreationDate = payload.CreationDate, Section = payload.Section}, commandType: CommandType.StoredProcedure);
    // }

}
