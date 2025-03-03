using backend.Models;

namespace backend.Repositories
{
    public interface ILevelRepository
    {
        Task<List<Level>> getAllLevel();
    }
}
