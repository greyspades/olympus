using MongoDB.Driver;
using Article.Model;

namespace Article.Interface
{
    public interface IArticleRepository
    {
        public Task CreateArticle(ArticleModel payload);
        public Task<IEnumerable<ArticleModel>> GetArticles();
        public Task UpdateArticle(ArticleModel payload);
        public Task CreateArticleView(ArticleModel payload);
        public Task<IEnumerable<ArticleModel>> GetArticleViews();
        public Task UpdateArticleView(ArticleModel payload);
        public Task<IEnumerable<ArticleModel>> GetArticleViewById(string Id);
        public Task<IEnumerable<ArticleModel>> GetArticleViewByTitle(string title);
        public Task DeleteArticle(string id);
    }
}
