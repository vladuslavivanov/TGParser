using CryptoPay;
using CryptoPay.Types;
using TGParser.API.Services.Interfaces;

namespace TGParser.API.Services.Implementations;

public class CryptoBotService(ICryptoPayClient cryptoPayClient) : ICryptoBotService
{
    public async Task<string> CreateInvoiceLink(CurrencyTypes currencyType, string currency, double amount, string desctiption, string hiddenMessage, PaidButtonNames paidButtonName, string paidButtonUrl, string payload, int expiresIn)
    {
        var invoice = await cryptoPayClient.CreateInvoiceAsync(amount,
            currencyType: currencyType,
            asset: currencyType == CurrencyTypes.crypto ? currency : null,
            fiats: currencyType == CurrencyTypes.fiat ? currency : null,
            description: desctiption,
            hiddenMessage: hiddenMessage,
            paidBtnName: paidButtonName,
            paidBtnUrl: paidButtonUrl,
            payload: payload,
            expiresIn: expiresIn);

        return invoice.BotInvoiceUrl;
    }
}
