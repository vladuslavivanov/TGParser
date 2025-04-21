using CryptoPay.Types;

namespace TGParser.API.Services.Interfaces;

public interface ICryptoBotService
{
    /// <summary>
    /// Создать ссылку на платеж.
    /// </summary>
    /// <param name="currencyType">Тип валюты.</param>
    /// <param name="currency">Сокращенное название валюты.</param>
    /// <param name="amount">Цена.</param>
    /// <param name="desctiption">Описание товара.</param>
    /// <param name="hiddenMessage">Сообщение после оплаты.</param>
    /// <param name="paidButtonNames">Тип кнопки, после оплаты для редиректа.</param>
    /// <param name="paidButtonUrl">Ссылка, на которую будет вести кнопка.</param>
    /// <param name="payload">Сообщение, которое будет передано по webhooks.</param>
    /// <param name="expiresIn">Время жизни ссылки.</param>
    /// <returns></returns>
    Task<string> CreateInvoiceLink(CurrencyTypes currencyType, 
        string currency, 
        double amount, 
        string desctiption,
        string hiddenMessage,
        PaidButtonNames paidButtonNames,
        string paidButtonUrl,
        string payload,
        int expiresIn);
}
