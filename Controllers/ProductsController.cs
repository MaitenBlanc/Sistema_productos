using Microsoft.AspNetCore.Mvc;
using prueba_addaccion.Data;
using prueba_addaccion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace prueba_addaccion.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Listado de productos
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // Formulario para crear un producto
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.ProductCategories
                .Select(c => new { c.CategoryProductId, c.CategoryDescription })
                .ToListAsync();

            ViewBag.CategoryList = new SelectList(categories, "CategoryProductId", "CategoryDescription");

            //ViewData["CategoryList"] = new SelectList(categories, "CategoryProductId", "CategoryDescription");

            if (categories == null || !categories.Any())
            {
                Console.WriteLine("No se encontraron categorías en la base de datos.");
            }

            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, bool HaveECDiscountCheckbox, bool IsActiveCheckbox)
        {
            Console.WriteLine($"HaveECDiscountCheckbox: {HaveECDiscountCheckbox}");
            Console.WriteLine($"IsActiveCheckbox: {IsActiveCheckbox}");
            Console.WriteLine($"CategoryProductId: {product.CategoryProductId}");

            // Mapear los valores de los checkbox
            product.HaveECDiscount = HaveECDiscountCheckbox ? "S" : "N";
            product.IsActive = IsActiveCheckbox ? "S" : "N";

            if (product.CategoryProductId == 0 || product.CategoryProductId == null) 
            {
                ModelState.AddModelError("CategoryProductId", "La categoría es obligatoria.");
            }

            //Verificar que la categoría haya sido seleccionada
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState no es válido.");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en {state.Key}: {error.ErrorMessage}");
                    }
                }

                // Si no es válido, volver a cargar las categorías
                var categorias = await _context.ProductCategories.ToListAsync();
                ViewData["CategoryList"] = new SelectList(categorias, "CategoryProductId", "CategoryDescription");
                return View(product);
            }

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                Console.WriteLine("Producto guardado exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar en la base de datos: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "Ocurrió un error al intentar guardar el producto.");
            }



            //Si llegamos aquí, hay errores, volvemos a cargar las categorías
            var categories = await _context.ProductCategories.ToListAsync();
            ViewData["CategoryList"] = new SelectList(categories, "CategoryProductId", "CategoryDescription");

            Console.WriteLine($"CategoryProductId: {product.CategoryProductId}");

            return View(product);
        }

        // Formulario para editar un producto
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Mapear valores de base de datos para checkbox
            product.HaveECDiscountCheckbox = product.HaveECDiscount == "S";
            product.IsActiveCheckbox = product.IsActive == "S";

            var categories = await _context.ProductCategories.ToListAsync();
            ViewData["CategoryList"] = new SelectList(categories, "CategoryProductId", "CategoryDescription", product.CategoryProductId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Product product, bool HaveECDiscountCheckbox, bool IsActiveCheckbox)
        {
            // Verifica que el ID del producto coincida con el parámetro de la ruta
            if (id != product.ProductId)
            {
                Console.WriteLine($"El ID del producto no coincide: {id} != {product.ProductId}");
                return NotFound();
            }

            Console.WriteLine($"Editando producto con ID: {id}");
            Console.WriteLine($"HaveECDiscountCheckbox: {HaveECDiscountCheckbox}");
            Console.WriteLine($"IsActiveCheckbox: {IsActiveCheckbox}");

            // Mapear los valores de los checkbox
            product.HaveECDiscount = HaveECDiscountCheckbox ? "S" : "N";
            product.IsActive = IsActiveCheckbox ? "S" : "N";

            if (product.CategoryProductId == 0 || product.CategoryProductId == null)
            {
                ModelState.AddModelError("CategoryProductId", "La categoría es obligatoria.");
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState no es válido.");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en {state.Key}: {error.ErrorMessage}");
                    }
                }

                var categories = await _context.ProductCategories.ToListAsync();
                ViewData["CategoryList"] = new SelectList(categories, "CategoryProductId", "CategoryDescription", product.CategoryProductId);
                return View(product);
            }

            try
            {
                Console.WriteLine("Actualizando producto...");
                _context.Update(product);
                await _context.SaveChangesAsync();
                Console.WriteLine("Producto actualizado exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar en la base de datos: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "Ocurrió un error al intentar actualizar el producto.");
            }

            var categoriesAgain = await _context.ProductCategories.ToListAsync();
            ViewData["CategoryList"] = new SelectList(categoriesAgain, "CategoryProductId", "CategoryDescription", product.CategoryProductId);

            return View(product);
        }

        // Eliminación
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // Procesar eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al intentar eliminar el producto.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
