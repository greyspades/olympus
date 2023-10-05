using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Slider.Interface;
using Slider.Model;

namespace SliderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ISliderRepository _repo;
        Guid guid = Guid.NewGuid();
        public SliderController(IConfiguration config, ISliderRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult> GetSlides()
        {
            try
            {
                var data = await _repo.GetAllSlides();

                foreach (SlideModel slide in data)
                {
                    var imagePath = @$"images/slides/{slide.Id}.jpg"; // Replace with your image path.
                    var contentType = "image/jpeg"; // Set the appropriate content type.

                    byte[] imageData;
                    using (FileStream imageStream = new(imagePath, FileMode.Open, FileAccess.Read))
                    using (MemoryStream memoryStream = new())
                    {
                        imageStream.CopyTo(memoryStream);
                        slide.ImageBytes = memoryStream.ToArray();
                    }
                    // slide.ActionBtn = JsonSerializer.Deserialize<ActionButtonModel>(slide.ActionString);
                    // var test = JsonSerializer.Deserialize<ActionButtonModel>(slide.ActionString);
                    // Console.WriteLine(test.Display);
                    // Console.WriteLine(test.Display);
                }

                var response = new
                {
                    code = 200,
                    message = "success",
                    status = true,
                    data
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, response);
            }
        }
        // [DisableCors]
        [HttpPost]
        public async Task<ActionResult> CreateSlide([FromForm] SlideModel payload)
        {
            try
            {
                payload.Id = guid.ToString();

                payload.CreationDate = DateTime.Now;
                payload.BgType = "Image";
                payload.Animation = "40s para infinite linear";
                payload.Position = "unset";
                payload.Banner = "Remove";
                payload.SideBar = "None";

                await _repo.CreateSlide(payload);

                var response = new
                {
                    code = 200,
                    status = true,
                    message = "Successfully created new Slide"
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateSlide([FromForm] SlideModel payload)
        {
            try
            {
                await _repo.UpdateSlide(payload);

                var response = new
                {
                    code = 200,
                    message = "Slide updated successfully",
                    status = true
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to process your request"
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("delete")]
        public async Task<ActionResult> DeleteSlide(DeleteDto payload)
        {
            try
            {
                await _repo.DeleteSlide(payload.Id);

                var response = new
                {
                    code = 200,
                    message = "Slide deleted successfully",
                    status = true
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                var response = new
                {
                    code = 500,
                    message = "Unnable to process your request",
                    status = false
                };

                return StatusCode(500, response);
            }
        }
        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetImage(string id) {
            try {
                var filePath = @$"images/slides/{id}.jpg";

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                return File(fileStream, "image/jpeg");
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
