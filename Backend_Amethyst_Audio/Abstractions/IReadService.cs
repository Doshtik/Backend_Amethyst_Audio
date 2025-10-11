namespace Backend_Amethyst_Audio.Abstractions
{
    public interface IReadService<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetAllAsync();
    }
}
