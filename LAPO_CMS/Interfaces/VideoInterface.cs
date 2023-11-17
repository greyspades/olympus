using MongoDB.Driver;
using Video.Model;

namespace Video.Interface
{
    public interface IVideoRepository
    {
        public Task CreateVideo(VideoModel payload);
        public Task<IEnumerable<VideoModel>> GetVideos();
        public Task UpdateVideo(VideoModel payload);
    }
}
