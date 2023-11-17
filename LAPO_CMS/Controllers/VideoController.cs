using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Video.Interface;
using Video.Model;

namespace Video.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IVideoRepository _repo;
        Guid guid = Guid.NewGuid();
        public VideoController(IConfiguration config, IVideoRepository repo)
        {
            _config = config;
            _repo = repo;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetArticles() {
            try {
                var data = await _repo.GetVideos();

                var response = new {
                    code = 200,
                    status = true,
                    data
                };

                return Ok(response);

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500, 
                    message = "Unsuccessful",
                    status = false
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateArticle([FromForm] VideoModel payload) {
            try {
                var file = HttpContext.Request.Form;
                
                payload.Id = guid.ToString();

                await _repo.CreateVideo(payload);
                
                var response = new {
                    code = 200,
                    status = true,
                    message = "Article created successfully"
                };

                return Ok(response);

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500, 
                    message = "Unsuccessful",
                    status = false
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateArticle([FromForm] VideoModel payload) {
            try {

                payload.CreationDate = DateTime.Now;

                await _repo.UpdateVideo(payload);
                
                var response = new {
                    code = 200,
                    status = true,
                    message = "Article updated successfully"
                };

                return Ok(response);

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500, 
                    message = "Unsuccessful",
                    status = false
                };

                return StatusCode(500, response);
            }
        }

        [HttpGet("video/{id}")]
        public async Task<ActionResult> GetImage(string id) {
            try {
                var filePath = @$"videos/home/{id}.mp4";

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                return File(fileStream, "video/mp4");
                
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }
}
}
