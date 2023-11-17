using Article.Model;
using Credentials.Models;

namespace User.Interface
{
    public interface IUserRepository
    {
        public Task<dynamic> AdminAuth(AdminDto payload);
        public Task CreateAdmin(UserData payload);
        public Task<IEnumerable<UserData>> GetAdmin(string Id);
    }
}
