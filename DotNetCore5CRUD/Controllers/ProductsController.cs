
using DotNetCore5CRUD.Models;
using DotNetCore5CRUD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5CRUD.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public ProductsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }


        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.OrderByDescending(m => m.Price).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductFormViewModel
            {
                Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync()
            };

            return View("ProductForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                return View("ProductForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("ProductForm", model);
            }

            var poster = files.FirstOrDefault();

            if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                return View("ProductForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View("ProductForm", model);
            }

            using var dataStream = new MemoryStream();

            await poster.CopyToAsync(dataStream);

            var movies = new Product
            {
                Name = model.Name,
                TagId = model.TagId,
                Description = model.Description,
                Price = model.Price,
                InStock = model.InStock,
                Image = dataStream.ToArray()
            };

            _context.Products.Add(movies);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Product created successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            var viewModel = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                TagId = product.TagId,
                Price = product.Price,
                InStock = product.InStock,
                Description = product.Description,
                Poster = product.Image,
                Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync()
            };

            return View("ProductForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                return View("ProductForm", model);
            }

            var product = await _context.Products.FindAsync(model.Id);

            if (product == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("ProductForm", model);
                }

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Tags = await _context.Tags.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("ProductForm", model);
                }

                product.Image = model.Poster;
            }

            product.Name = model.Name;
            product.TagId = model.TagId;
            product.Description = model.Description;
            product.Price = model.Price;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Product updated successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _context.Products.Include(m => m.Tag).SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Products.FindAsync(id);

            if (movie == null)
                return NotFound();

            _context.Products.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }
    }
}