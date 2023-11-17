using System.Collections;
using MongoDB.Driver;
using Video.Interface;
using Video.Model;
using Mongo.client;
using MongoDB.Bson;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace Video.Repository;

public class VideoRepository : IVideoRepository
{
    private readonly IConfiguration _config;
    private readonly MongoConnection _connection;

    public VideoRepository(IConfiguration config, MongoConnection connection)
    {
        this._config = config;
        this._connection = connection;
    }
    public async Task<IEnumerable<VideoModel>> GetVideos() {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<VideoModel>("Get_videos", commandType: CommandType.StoredProcedure);
        return data;
    }
    public async Task CreateVideo(VideoModel payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var extension = Path.GetExtension(payload.Video!.FileName);

        var path = @$"videos/home/{payload.Id}.jpg";

        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await payload.Video.CopyToAsync(stream);
        }

        await connection.ExecuteAsync("Create_video", new {Id = payload.Id, Name = payload.Name, Path = payload.Path, CreationDate = payload.CreationDate, Section = payload.Section, Active = payload.Active }, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateVideo(VideoModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        if (payload.Video != null)
        {
            var extension = Path.GetExtension(payload.Video!.FileName);

            var path = @$"images/articles/{payload.Id}.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await payload.Video.CopyToAsync(stream);
            }
        }
        await connection.ExecuteAsync("Edit_Video", new { Id = payload.Id, Name = payload.Name, Active = payload.Active}, commandType: CommandType.StoredProcedure);
    }
}
