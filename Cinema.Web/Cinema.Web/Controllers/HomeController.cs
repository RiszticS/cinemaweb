using System.Diagnostics;
using AutoMapper;
using Cinema.DataAccess.Services;
using Cinema.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMoviesService _moviesService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public HomeController(IMoviesService moviesService, IMapper mapper, IConfiguration configuration)
    {
        _moviesService = moviesService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var lastestMovies = await _moviesService.GetLatestMoviesAsync(int.Parse(_configuration["NewMovieCount"]!));
        var lastestMoviesViewModels = _mapper.Map<List<MovieViewModel>>(lastestMovies);

        var homePageViewModel = new HomePageViewModel()
        {
            LatestMovies = lastestMoviesViewModels
        };

        return View(homePageViewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}