namespace Video.Model;

public class VideoModel {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Path { get; set; }
    public IFormFile? Video { get; set; }
    public string? Section { get; set; }
    public bool? Active { get; set; }
}