using System.Collections;
using MongoDB.Driver;
using Article.Interface;
using Article.Model;
using Mongo.client;
using MongoDB.Bson;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace Article.Repository;

public class ArticleRepository : IArticleRepository
{
    private readonly IConfiguration _config;
    private readonly MongoConnection _connection;

    public ArticleRepository(IConfiguration config, MongoConnection connection)
    {
        this._config = config;
        this._connection = connection;
    }
    public async Task<IEnumerable<ArticleModel>> GetArticles() {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<ArticleModel>("Get_articles", commandType: CommandType.StoredProcedure);

        return data;
    }

    public async Task CreateArticle(ArticleModel payload) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var extension = Path.GetExtension(payload.Image!.FileName);

        var path = @$"images/articles/{payload.Id}.jpg";

        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await payload.Image.CopyToAsync(stream);
        }

        await connection.ExecuteAsync("Create_article", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph, CreationDate = payload.CreationDate, IsActive = payload.IsActive, Section = payload.Section}, commandType: CommandType.StoredProcedure);
    }
}