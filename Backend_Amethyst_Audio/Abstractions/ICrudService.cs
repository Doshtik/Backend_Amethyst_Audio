namespace Backend_Amethyst_Audio.Abstractions
{
    public interface ICrudService<T> : IReadService<T>
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
