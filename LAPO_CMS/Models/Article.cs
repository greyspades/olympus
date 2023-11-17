namespace Article.Model;

public enum Sections {SEC1, SEC2, SEC3}
public class ArticleModel {
    public string? Section { get; set; }
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Paragraph { get; set; }
    public string? Header { get; set; }
    public DateTime? CreationDate { get; set; }
    public IFormFile? Image { get; set; }
    public IFormFile? Avatar { get; set; }
    public string? Author { get; set; }
    public string? Uname { get; set; }
    // public byte[]? ImageBytes { get; set; }
    // public bool? IsActive { get; set; }
}

public class ArticleDto {
    public ArticleModel? Section1 { get; set; }
    public ArticleModel? Section2 { get; set; }
    public ArticleModel? Section3 { get; set; }
}

public class ArticleViewDto {
    // public ArticleModel? Article { get; set; }
    public int Page { get; set; }
    public int Take { get; set; }
}

public class GetArticleDto {
    public string? Id { get; set; }
    public string? Title { get; set; }
}

public class UserData {
    public IFormFile? Avatar { get; set; }
    public string? Id { get; set; }
    public string? StaffId { get; set; }
    public string? Role { get; set; }
    public string? Name { get; set; }
}