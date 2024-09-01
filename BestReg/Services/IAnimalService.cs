using System.Collections.Generic;
using System.Threading.Tasks;
using BestReg.Data;

namespace BestReg.Services
{
    public interface IAnimalService
    {
        Task<List<Animal>> GetAllAnimalsAsync();
        Task<Animal> GetAnimalByIdAsync(int id);
        Task AddAnimalAsync(Animal animal);
        Task UpdateAnimalAsync(Animal animal);
        Task DeleteAnimalAsync(int id);
    }
}
