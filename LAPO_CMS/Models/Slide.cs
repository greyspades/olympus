using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Slider.Model;

public class ActionButtonModel {
    public string? Display { get; set; }
    public string? Url { get; set; }
}
public class FileData
{
    public string? FileName { get; set; }
    public byte[]? Content { get; set; }
}
public class SlideModel {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? BgType { get; set; }
    public string? Animation { get; set; }
    public string? Position { get; set; }
    public string? KtLnGnt { get; set; }
    public string? LnGnt { get; set; }
    [BsonElement("H1text")]
    public string? H1Text { get; set; }
    [BsonElement("Ptext")]
    public string? PText { get; set; }
    public string? SideBar { get; set; }
    [BsonElement("IsActionbtn")]
    public bool? IsActionBtn { get; set; }
    public string? Banner { get; set; }
    [BsonElement("Actionbtn")]
    // public IFormFile? File { get; set; }
    public IFormFile? Image { get; set; }
    public byte[]? ImageBytes { get; set; }
    public DateTime? CreationDate { get; set; }
    // public string? ActionString { get; set; }
    public string? Url { get; set; }
    public string? Display { get; set; }
    public bool? Active { get; set; }
    // public ActionButtonModel? ActionBtn { get; set; }
}

public class DeleteDto {
    public string? Id { get; set; }
}
