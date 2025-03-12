using Cinema.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DataAccess.Services
{
    public interface IMoviesService
    {
        Task<IReadOnlyCollection<Movie>> GetLatestMoviesAsync(int? count = null);

        Task<Movie> GetByIdAsync(int id);
    }
}
