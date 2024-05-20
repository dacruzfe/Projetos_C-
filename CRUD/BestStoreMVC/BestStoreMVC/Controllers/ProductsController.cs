using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    /// <summary>
    /// Controller é a classe onde eu crio os métodos e funções de CRUD
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext Context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDBContext context, IWebHostEnvironment environment) 
        { 
          this.Context = context;
            this.environment = environment;
        }

        /// <summary>
        /// Funções para Criação, leitura, Atualização e Exclusão dos produtos no banco de dados
        /// </summary>

        #region Create 
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }


            // Salva o arquivo de Imagem
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.OpenWrite(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // Salva o novo produto no banco de dados
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            Context.Products.Add(product);
            Context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        #endregion

        #region Read
        public IActionResult Index()
        {
            var products = Context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        #endregion

        #region Update
        public IActionResult Edit(int id) 
        {
            var product = Context.Products.Find(id);

            if (product == null) 
            {
                return RedirectToAction("Index", "Products");
            }

            // Cria ProdutoDTo para Produto
            var productDto = new ProductDto()
            {
                Name= product.Name,
                Brand= product.Brand,   
                Category = product.Category,    
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["imageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = Context.Products.Find(id);

            if (product == null) 
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid) 
            {
                ViewData["ProductId"] = product.Id;
                ViewData["imageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // Atualiza o arquivo de imagem se houver um novo arquivo de imagem
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath)) 
                {
                   productDto.ImageFile.CopyTo(stream);
                }

                // Deleta a imagem anterior
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            // Atualiza o produto no banco de dados
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            Context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        #endregion

        #region Delete

        public IActionResult Delete(int id)
        {
            var product = Context.Products.Find(id);
            if (product == null) 
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            Context.Products.Remove(product);
            Context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }

        #endregion

    }
}
