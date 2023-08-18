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
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService service,
            UserManager<IdentityUser> userManager) : base(context, service, userManager)
        {
        }

        public IList<Invoice> Invoice { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (Context.Invoice != null)
            {
                var invoice = from i in Context.Invoice
                              select i;
                var isManager = User.IsInRole(Constants.InvoiceManagerRole);  // for checking role is manager or not
                var currentUserId = UserManager.GetUserId(User);  // for checking is invoice owner or not

                if (!isManager)
                {
                    invoice = invoice.Where(i => i.CreatorId == currentUserId);
                }
                Invoice = await invoice.ToListAsync();
            }
        }
    }
}
