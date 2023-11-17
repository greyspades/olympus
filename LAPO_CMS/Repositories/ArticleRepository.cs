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
     public async Task<IEnumerable<ArticleModel>> GetArticleViews() {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<ArticleModel>("Get_article_views", commandType: CommandType.StoredProcedure);
        return data;
    }
    public async Task<IEnumerable<ArticleModel>> GetArticleViewById(string id) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<ArticleModel>("Get_article_by_id", new { Id = id }, commandType: CommandType.StoredProcedure);
        return data;
    }
    public async Task<IEnumerable<ArticleModel>> GetArticleViewByTitle(string title) {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var data = await connection.QueryAsync<ArticleModel>("Get_article_view_by_type", new { Title = title }, commandType: CommandType.StoredProcedure);
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

        await connection.ExecuteAsync("Create_article", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph, CreationDate = payload.CreationDate, Section = payload.Section}, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateArticle(ArticleModel payload)
    {
        using SqlConnection? connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        if (payload.Image != null)
        {
            Console.WriteLine("has image");
            var extension = Path.GetExtension(payload.Image!.FileName);

            var path = @$"images/articles/{payload.Id}.jpg";
            if (File.Exists(path))
            {
                Console.WriteLine("it exists");
                File.Delete(path);
            }
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await payload.Image.CopyToAsync(stream);
            }
        }
        await connection.ExecuteAsync("Update_article", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph, CreationDate = payload.CreationDate, Section = payload.Section}, commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateArticleView(ArticleModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        if (payload.Image != null)
        {
            var extension = Path.GetExtension(payload.Image!.FileName);

            var path = @$"images/articleViews/{payload.Id}.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await payload.Image.CopyToAsync(stream);
            }
        }
        await connection.ExecuteAsync("Update_article_view", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph}, commandType: CommandType.StoredProcedure);
    }
    public async Task DeleteArticle(string id) {
        try {
            var filePath = @$"images/articleViews/{id}.jpg";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                Console.WriteLine("File not found.");
            }

            using SqlConnection? connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Delete_article_view", new {Id = id}, commandType: CommandType.StoredProcedure);
        } catch(Exception e) {
            Console.WriteLine(e.Message);
        }
    }
    public async Task CreateArticleView(ArticleModel payload) {
        
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var extension = Path.GetExtension(payload.Image!.FileName);

        var path = @$"images/articleViews/{payload.Id}.jpg";

        if(payload.Avatar != null) {
            var avatarExtension = Path.GetExtension(payload.Avatar!.FileName);

            var avatarPath = @$"images/avatars/{payload.Id}.jpg";

            using (var stream = new FileStream(avatarPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await payload.Avatar.CopyToAsync(stream);
            }
        }

        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            await payload.Image.CopyToAsync(stream);
        }

        await connection.ExecuteAsync("Create_article_view", new {Id = payload.Id, Title = payload.Title, Header = payload.Header, Paragraph = payload.Paragraph, CreationDate = payload.CreationDate, Author = payload.Author }, commandType: CommandType.StoredProcedure);
    }
}
