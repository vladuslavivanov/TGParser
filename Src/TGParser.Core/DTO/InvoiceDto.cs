namespace TGParser.Core.DTO;

/// <summary>
/// Модель оплаты.
/// </summary>
/// <param name="Currency">Валюта.</param>
/// <param name="Amount">Оплаченная сумма.</param>
/// <param name="PaidAt">Дата оплаты.</param>
/// <param name="QuantityDays">Количество купленных дней.</param>
/// <param name="UserId">Идентификатор пользователя, который совершил оплату.</param>
public record InvoiceDto(string Currency, double Amount, DateTime PaidAt, int QuantityDays, long UserId);