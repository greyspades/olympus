using MongoDB.Driver;
using Article.Model;

namespace Article.Interface
{
    public interface IArticleRepository
    {
        public Task CreateArticle(ArticleModel payload);
        public Task<IEnumerable<ArticleModel>> GetArticles();
    }
}
