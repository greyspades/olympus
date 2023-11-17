using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Article.Interface;
using Article.Model;
using Microsoft.AspNetCore.Authorization;

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
        // [Authorize]
        public async Task<ActionResult> GetArticles() {
            try {
                ArticleDto? data = new()
                {
                    Section1 = null,
                    Section2 = null,
                    Section3 = null
                };
                var articles = await _repo.GetArticles();
                
                foreach(ArticleModel article in articles) {
                    if(article.Section == "SEC1") {
                        data.Section1 = article;
                    } else if(article.Section == "SEC2") {
                        data.Section2 = article;
                    } else if(article.Section == "SEC3") {
                        data.Section3 = article;
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
        // [Authorize]
        [HttpPost("view")]
        public async Task<ActionResult> GetArticleViewById(GetArticleDto payload) {
            try {
                var data = await _repo.GetArticleViewById(payload.Id);

                return Ok(new {
                    code = 200,
                    status = true,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(500, new {
                    code = 500,
                    message = "Unnable to process your request"
                });
            }
        }

        [HttpPost("delete")]
        [Authorize]
        public async Task<ActionResult> DeleteArticleView(GetArticleDto payload) {
            try {
                Console.WriteLine(payload.Id);
                await _repo.DeleteArticle(payload.Id);

                return Ok(new {
                    code = 200,
                    status = true,
                    message = "Successfully deleted article"
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                return StatusCode(500, new {
                    code = 500,
                    message = "Unnable to process your request"
                });
            }
        }

         [HttpPost("view/type")]
        public async Task<ActionResult> GetArticleViewByType(GetArticleDto payload) {
            try {
                var data = await _repo.GetArticleViewByTitle(payload.Title);

                return Ok(new {
                    code = 200,
                    status = true,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                return StatusCode(500, new {
                    code = 500,
                    message = "Unnable to process your request"
                });
            }
        }

        [HttpGet("all")]
        // [Authorize]
        public async Task<ActionResult> GetAllArticleViews() {
            try {
                var data = await _repo.GetArticleViews();

                return Ok(new {
                    code = 200,
                    status = true,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(500, new {
                    code = 500,
                    message = "Unnable to process your request"
                });
            }
        }
        [HttpPost("pages")]
        // [Authorize]
        public async Task<ActionResult> GetArticleViews(ArticleViewDto payload) {
            try {
                var data = await _repo.GetArticleViews();

                List<ArticleModel> dataList = (List<ArticleModel>)data;

                var count = payload.Page * 10;

                var slicedList = data.Skip(count).Take(10);

                return Ok(new {
                    code = 200,
                    status = true,
                    data
                });
            } catch (Exception e){
                Console.WriteLine(e.Message);
                return StatusCode(500, new {
                    code = 500,
                    message = "Unnable to process your request"
                });
            }
        }
        [HttpPost("page")]
        // [Authorize]
        public async Task<ActionResult> CreateArticleView([FromForm] ArticleModel payload) {
            try {
                payload.Id = Guid.NewGuid().ToString();
                payload.CreationDate = DateTime.Now;

                await _repo.CreateArticleView(payload);
                
                return Ok(new {
                    status = "success",
                    code = 200,
                    message = "Successfully created new Article View"
                });
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
        [Authorize]
        public async Task<ActionResult> CreateArticle([FromForm] ArticleDto payload) {
            try {
                var articles = await _repo.GetArticles();
                if(articles.Any() == false) {
                    var file = HttpContext.Request.Form;

                payload.Section1.Id = Guid.NewGuid().ToString();
                payload.Section2.Id = Guid.NewGuid().ToString();
                payload.Section3.Id = Guid.NewGuid().ToString();
                payload.Section1.CreationDate = DateTime.Now;
                payload.Section2.CreationDate = DateTime.Now;
                payload.Section3.CreationDate = DateTime.Now;
                payload.Section1.Image = file.Files[0];
                payload.Section2.Image = file.Files[1];
                payload.Section3.Image = file.Files[2];

                // Console.WriteLine(payload.Section1.Paragraph);
                // Console.WriteLine(payload.Section1.Header);

                await _repo.CreateArticle(payload.Section1);
                await _repo.CreateArticle(payload.Section2);
                await _repo.CreateArticle(payload.Section3);
                
                var response = new {
                    code = 200,
                    status = true,
                    message = "Article created successfully"
                };

                return Ok(response);
                } else {
                    return Ok(new {
                        code = 401,
                        status = false,
                        message = "Articles have aleady been created. please modify existing articles"
                    });
                }

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
        [Authorize]
        public async Task<ActionResult> UpdateArticle([FromForm] ArticleModel payload) {
            try {
                // var file = HttpContext.Request.Form;

                // payload.Section1.Id = Guid.NewGuid().ToString();
                // payload.Section2.Id = Guid.NewGuid().ToString();
                // payload.Section3.Id = Guid.NewGuid().ToString();
                // payload.Section1.CreationDate = DateTime.Now;
                // payload.Section2.CreationDate = DateTime.Now;
                // payload.Section3.CreationDate = DateTime.Now;
                // payload.Section1.Image = file.Files[0];
                // payload.Section2.Image = file.Files[1];
                // payload.Section3.Image = file.Files[2];

                // payload.CreationDate = DateTime.Now;

                await _repo.UpdateArticle(payload);
                
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

        [HttpPost("update/page")]
        [Authorize]
        public async Task<ActionResult> UpdateArticleView([FromForm] ArticleModel payload) {
            try {
                await _repo.UpdateArticleView(payload);
                
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

        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetImage(string id) {
            try {
                var filePath = @$"images/articles/{id}.jpg";

                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/jpeg");
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return NotFound("File not found");
                }
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

        [HttpGet("image/views/{id}")]
        public async Task<ActionResult> GetArticleViewImage(string id) {
            try {
                var filePath = @$"images/articleViews/{id}.jpg";

                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/jpeg");
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return NotFound("File not found");
                }
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

        [HttpGet("image/avatar/{id}")]
        public async Task<ActionResult> GetAvatar(string id) {
            try {
                var filePath = @$"images/avatars/{id}.jpg";
                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/jpeg");
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return NotFound("File not found");
                }

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
