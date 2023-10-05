namespace Article.Model;

public enum Sections {SEC1, SEC2, SEC3}
public class ArticleModel {
    public Sections Section { get; set; }
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Paragraph { get; set; }
    public string? Header { get; set; }
    public DateTime? CreationDate { get; set; }
    public IFormFile? Image { get; set; }
    public byte[]? ImageBytes { get; set; }
    public bool? IsActive { get; set; }
}

public class ArticleDto {
    public string? Id { get; set; }
    public ArticleModel? Section1 { get; set; }
    public ArticleModel? Section2 { get; set; }
    public ArticleModel? Section3 { get; set; }
    public bool Active { get; set; }
}