using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.WebUI.ApiServices;
using Movies.WebUI.Models;

namespace Movies.WebUI.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieApiService _movieApiService;

        public MovieController(IMovieApiService movieApiService)
        {
            _movieApiService = movieApiService;
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        private async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            Console.WriteLine($"Token : {identityToken}");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim type : {claim.Type} / Claim value : {claim.Value}");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OnlyAdmin()
        {
            var userInfo = await _movieApiService.GetUserInfo();
            return View(userInfo);
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            await LogTokenAndClaims();
            return View(await _movieApiService.GetMovies());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var movie = await _movieApiService.GetMovie(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                await _movieApiService.CreateMovie(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var movie = await _movieApiService.GetMovie(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _movieApiService.UpdateMovie(movie);
                }
                catch (Exception)
                {
                    if (!await MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var movie = await _movieApiService.GetMovie(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _movieApiService.GetMovie(id);
            if (movie != null)
            {
                await _movieApiService.DeleteMovie(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MovieExists(int id)
        {
            return await _movieApiService.GetMovie(id) != null ? true : false;
        }
    }
}