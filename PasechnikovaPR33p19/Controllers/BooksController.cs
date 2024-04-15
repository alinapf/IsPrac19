using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasechnikovaPR33p19.Domain.Services;
using System.Reflection.PortableExecutable;

namespace PasechnikovaPR33p19.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksReader reader;

        public BooksController(IBooksReader reader)
        {
            this.reader = reader;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await reader.GetAllBooksAsync());
        }
    }
}
