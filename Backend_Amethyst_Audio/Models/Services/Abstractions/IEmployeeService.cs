using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.Services.Abstractions;

public interface IEmployeeService
{
    Task<Employee> GetByIdAsync(long id);
    Task<List<Employee>> GetAllAsync();
    
    Task CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(long idEmployee);

    Task<Employee> GetEmployeeByEmail(string email, string password);
}