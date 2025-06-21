using System.Data.Entity.Migrations.Model;
using TGParser.BLL.Interfaces;
using TGParser.Core.DTO;
using TGParser.DAL;

namespace TGParser.BLL.Implementations;

public class InvoiceManager(DataContext dataContext) :  IInvoiceManager
{
    public async Task AddInvoice(InvoiceDto invoice)
    {
        using var transaction = await dataContext.Database.BeginTransactionAsync();

        try
        {
            await dataContext.Invoices.AddAsync(new()
            {
                Currency = invoice.Currency,
                Amount = invoice.Amount,
                PaidAt = invoice.PaidAt,
                QuantityDays = invoice.QuantityDays,
                UserId = invoice.UserId,
            });

            var user = dataContext.Users.First(u => u.UserId == invoice.UserId);

            user.SubscriptionEndDate = user.SubscriptionEndDate.AddDays(invoice.QuantityDays);
            
            await dataContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }

        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
}