using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Article.Interface;
using Article.Model;

namespace Article.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IArticleRepository _repo;
        Guid guid = Guid.NewGuid();
        public ArticleController(IConfiguration config, IArticleRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetArticles() {
            try {
                var data = await _repo.GetArticles();

                foreach (ArticleModel article in data)
                {
                    var imagePath = @$"images/articles/{article.Id}.jpg"; // Replace with your image path.
                    var contentType = "image/jpeg"; // Set the appropriate content type.

                    byte[] imageData;
                    using (FileStream imageStream = new(imagePath, FileMode.Open, FileAccess.Read))
                    using (MemoryStream memoryStream = new())
                    {
                        imageStream.CopyTo(memoryStream);
                        article.ImageBytes = memoryStream.ToArray();
                    }
                }

                var response = new {
                    code = 200,
                    status = true,
                    data
                };

                return Ok(response);

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                var response = new {
                    code = 500, 
                    message = "Unsuccessful",
                    status = false
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateArticle([FromForm] ArticleDto payload) {
            try {
                // sets a single gui gor the articles
                var file = HttpContext.Request.Form;
                // Console.WriteLine(file.FileName);

                string currentGuid = guid.ToString();
                payload.Id = currentGuid;
                Console.WriteLine(payload.Section1.Title);
                // Console.WriteLine(payload.Section2.Header);
                // Console.WriteLine(payload.Section3.Paragraph);
                payload.Section1.Id = currentGuid;
                payload.Section2.Id = currentGuid;
                payload.Section3.Id = currentGuid;
                payload.Section1.CreationDate = DateTime.Now;
                payload.Section2.CreationDate = DateTime.Now;
                payload.Section3.CreationDate = DateTime.Now;
                payload.Section1.Image = file.Files[0];
                payload.Section2.Image = file.Files[1];
                payload.Section3.Image = file.Files[2];

                await _repo.CreateArticle(payload.Section1);
                await _repo.CreateArticle(payload.Section2);
                await _repo.CreateArticle(payload.Section3);
                
                // if(payload.Section1.Image != null) {
                // string currentGuid = guid.ToString();
                // payload.Id = currentGuid;
                // Console.WriteLine(payload.Section1.Title);
                // // Console.WriteLine(payload.Section2.Header);
                // // Console.WriteLine(payload.Section3.Paragraph);
                // payload.Section1.Id = currentGuid;
                // payload.Section2.Id = currentGuid;
                // payload.Section3.Id = currentGuid;

                // // Console.WriteLine(payload.Section1.Id);

                // await _repo.CreateArticle(payload.Section1);
                // } else {
                //     Console.WriteLine("no image");
                // }

                // creates articles
                // foreach(ArticleModel article in articles) {
                //     article.Id = currentGuid;
                //     article.CreationDate = DateTime.Now;
                    
                //     await _repo.CreateArticle(article);
                // }
                
                var response = new {
                    code = 200,
                    status = true,
                    message = "Article created successfully"
                };

                return Ok(response);

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                var response = new {
                    code = 500, 
                    message = "Unsuccessful",
                    status = false
                };

                return StatusCode(500, response);
            }
        }
}
}
