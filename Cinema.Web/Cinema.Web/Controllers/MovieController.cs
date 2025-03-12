using AutoMapper;
using Cinema.DataAccess.Services;
using Cinema.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly IMapper _mapper;

        public MovieController(IMoviesService moviesService, IMapper mapper)
        {
            _moviesService = moviesService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var movies = _mapper.Map<List<MovieViewModel>>(await _moviesService.GetLatestMoviesAsync());
            return View(movies);
        }

        public async Task<IActionResult> Details(int movieId)
        {
            var movie = await _moviesService.GetByIdAsync(movieId);
            var movieViewModel = _mapper.Map<MovieViewModel>(movie);

            return View(new MovieDetailViewModel()
            {
                Movie = movieViewModel
            });
        }
    }
}
