using Invoice.Data;
using Invoice.Models;
using Invoice.Services;
using Invoice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Controllers
{
    public class BillInvoiceController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TaxCalculatorService _taxCalculatorService;
        private readonly UserManager<IdentityUser> _userManager;

        public BillInvoiceController(ApplicationDbContext dbContext, TaxCalculatorService taxCalculatorService, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _taxCalculatorService = taxCalculatorService;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new BillInvoiceViewModel();
            model.CreatorUserName = User.Identity?.Name;
            model.InvoiceItems = new List<InvoiceItemViewModel>();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(BillInvoiceViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && user != null)
            {
                var faktura = new BillInvoice
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CreationDate = DateTime.Now,
                    DueDate = model.DueDate,
                    CreatorUserId = user.Id,
                    RecipientName = model.RecipientName,
                    InvoiceItems = new List<InvoiceItem>()
                };

                if (model.InvoiceItems != null)
                {
                    foreach (var item in model.InvoiceItems)
                    {
                        var invoiceItem = new InvoiceItem
                        {
                            Description = item.Description,
                            Quantity = item.Quantity,
                            UnitPriceExcludingTax = item.UnitPriceExcludingTax,
                            TotalPriceExcludingTax = item.Quantity * item.UnitPriceExcludingTax
                        };

                        faktura.InvoiceItems.Add(invoiceItem);
                    }
                }

                var porezCalculator = _taxCalculatorService.GetTaxCalculator(model.TaxType);
                if (porezCalculator != null)
                {
                    faktura.TotalAmountExcludingTax = faktura.InvoiceItems.Sum(s => s.TotalPriceExcludingTax);
                    faktura.TotalAmountIncludingTax = faktura.TotalAmountExcludingTax + porezCalculator.CalculateTax(faktura.TotalAmountExcludingTax);
                }

                _dbContext.BillInvoices.Add(faktura);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult _InvoiceItemRow()
        {
            var invoiceItem = new InvoiceItemViewModel();

            return PartialView("_InvoiceItemRow", invoiceItem);
        }


        public ActionResult Index()
        {
            var invoices = _dbContext.BillInvoices.ToList();
            var invoiceViewModels = invoices.Select(i => new BillInvoiceViewModel
            {
                InvoiceNumber = i.InvoiceNumber,
                DueDate = i.DueDate,
                CreatorUserName = GetCreatorName(i.CreatorUserId),
                RecipientName = i.RecipientName,
                CanDelete = CanDeleteInvoice(i.CreationDate)
            }).ToList();

            return View(invoiceViewModels);
        }

        public ActionResult Details(string invoiceNumber)
        {
            var invoice = _dbContext.BillInvoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);

            if (invoice == null)
            {
                return View("Error"); // or handle the error in another way
            }

            var invoiceViewModel = new BillInvoiceViewModel
            {
                InvoiceNumber = invoice.InvoiceNumber,
                DueDate = invoice.DueDate,
                CreatorUserName = GetCreatorName(invoice.CreatorUserId),
                RecipientName = invoice.RecipientName,
                InvoiceItems = invoice.InvoiceItems.Select(item => new InvoiceItemViewModel
                {
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPriceExcludingTax = item.UnitPriceExcludingTax
                }).ToList()
            };

            return View(invoiceViewModel);
        }


        [HttpPost]
        public ActionResult Delete(string invoiceNumber)
        {
            var invoice = _dbContext.BillInvoices.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
            if (invoice == null)
            {
                return View("Error");
            }

            if (!CanDeleteInvoice(invoice.CreationDate))
            {
                return View("Error");
            }

            _dbContext.BillInvoices.Remove(invoice);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool CanDeleteInvoice(DateTime creationDate)
        {
            var currentTime = DateTime.Now;
            var timeElapsed = currentTime - creationDate;
            return timeElapsed.TotalMinutes <= 10;
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _dbContext.BillInvoices.OrderByDescending(i => i.Id).FirstOrDefault();

            if (lastInvoice != null)
            {
                var lastInvoiceNumber = int.Parse(lastInvoice.InvoiceNumber, NumberStyles.None);
                var nextInvoiceNumber = (lastInvoiceNumber + 1).ToString("D6");
                return nextInvoiceNumber;
            }
            else
            {
                return "000001";
            }
        }
        private string GetCreatorName(string creatorUserId)
        {
            var creator = _dbContext.Users.FirstOrDefault(u => u.Id == creatorUserId);
            return creator?.UserName;
        }
    }

}
