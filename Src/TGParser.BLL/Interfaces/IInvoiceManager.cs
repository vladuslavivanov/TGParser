using TGParser.Core.DTO;
using TGParser.DAL.Models;

namespace TGParser.BLL.Interfaces;

public interface IInvoiceManager
{
    Task AddInvoice(InvoiceDto invoice);
}