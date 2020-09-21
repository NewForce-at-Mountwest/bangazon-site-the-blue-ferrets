﻿using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var applicationDbContext = _context.Product.Include(p => p.ProductType).Include(p => p.User).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(p => p.Title.Contains(searchString));
            }
            if(applicationDbContext.Count() < 1 )
            {
                return NotFound();
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {


            ProductCreateViewModel ViewModel = new ProductCreateViewModel();

            ViewModel.productTypes = _context.ProductType.Select(c => new SelectListItem
            {
                Text = c.Label,
                Value = c.ProductTypeId.ToString()
            }
          ).ToList();

            ViewModel.productTypes.Insert(0, new SelectListItem() { Value = "0", Text = "--Select Product Category--" });

            //ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label");
            //ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View(ViewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            //should I be getting the true user id, because that's what i am getting??  
            ModelState.Remove("product.User");
            ModelState.Remove("product.UserId");

            ProductCreateViewModel ViewModel = new ProductCreateViewModel();
            if (ModelState.IsValid)
            {
                //User checks id
                var user = await GetCurrentUserAsync();
                product.UserId = user.Id;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = product.ProductId });
            }

            ViewModel.productTypes = _context.ProductType.Select(c => new SelectListItem
            {
                Text = c.Label,
                Value = c.ProductTypeId.ToString()
            }).ToList();



            //ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            //ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(ViewModel);
        }

        //ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
        //ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);

        //    return View(product);

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await GetCurrentUserAsync();
                    product.UserId = user.Id;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            // Get the current user
            var user = await GetCurrentUserAsync();

            // Find the product requested and subtract 1 from the quantity available
            Product productToAdd = await _context.Product.SingleOrDefaultAsync(p => p.ProductId == id);

            // See if the user has an open order
            var openOrder = await _context.Order.SingleOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);

            // If no order, create one, else add to existing order
            if (openOrder != null)
            {
                OrderProduct orderProduct = new OrderProduct()
                {
                    OrderId = openOrder.OrderId,
                    ProductId = id
                };
                if (ModelState.IsValid)
                {
                    _context.Add(orderProduct);
                    productToAdd.Quantity = productToAdd.Quantity - 1;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                Order neworder = new Order()
                {
                    UserId = user.Id,
                    User = user
                };
                if (ModelState.IsValid)
                {
                    _context.Add(neworder);
                    await _context.SaveChangesAsync();
                    openOrder = await _context.Order.SingleOrDefaultAsync(o => o.User == user && o.PaymentTypeId == null);

                    OrderProduct orderProduct = new OrderProduct()
                    {
                        OrderId = openOrder.OrderId,
                        ProductId = id
                    };
                    _context.Add(orderProduct);
                    productToAdd.Quantity = productToAdd.Quantity - 1;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return RedirectToAction("Types", "ProductTypes", new { id = id });
        }
    }
}

