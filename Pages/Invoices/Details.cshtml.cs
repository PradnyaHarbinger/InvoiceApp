using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Data;
using InvoiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InvoiceApp.Authorization;

namespace InvoiceApp.Pages.Invoices
{
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService service,
            UserManager<IdentityUser> userManager
            ) : base( context, service, userManager )
        {
        }

      public Invoice Invoice { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }
            else 
            {
                Invoice = invoice;
            }

            var isCreator = await AuthorizationService.AuthorizeAsync(
                User, Invoice, InvoiceOperations.Read);

            var isManager = User.IsInRole(Constants.InvoiceManagerRole);

            if(!isCreator.Succeeded && !isManager)
            {
                return Forbid();
            }




            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, InvoiceStatus status)
        {
            Invoice = await Context.Invoice.FindAsync(id);

            if(Invoice == null)
                return NotFound();

            var invoiceOperation = status == InvoiceStatus.Approved ? InvoiceOperations.Approved : InvoiceOperations.Rejected;

            var isAuthorize = await AuthorizationService.AuthorizeAsync(
                User, Invoice, invoiceOperation);

            if(!isAuthorize.Succeeded)
            {
                return Forbid();
            }

            Invoice.Status = status;
            Context.Invoice.Update(Invoice);

            await Context.SaveChangesAsync();

            return RedirectToAction("./Index");
        }
    }
}
