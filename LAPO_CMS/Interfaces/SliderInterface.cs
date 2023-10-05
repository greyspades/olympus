using MongoDB.Driver;
using Slider.Model;

namespace Slider.Interface
{
    public interface ISliderRepository
    {
        public Task<IEnumerable<SlideModel>> GetAllSlides();
        public Task CreateSlide(SlideModel payload);
        public Task UpdateSlide(SlideModel payload);
        public Task DeleteSlide(string Id);
    }
}
