namespace TGParser.DAL.Models;

public class Invoice
{
    /// <summary>
    /// Идентификатор платежа.
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Валюта платежа.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Сумма платежа.
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Дата совершения оплаты.
    /// </summary>
    public DateTime PaidAt { get; set; }

    /// <summary>
    /// Количество купленных дней.
    /// </summary>
    public int QuantityDays { get; set; }
    
    /// <summary>
    /// Идентификатор пользователя совершившего оплату.
    /// </summary>
    public long UserId { get; set; }

    #region Nav Props

    public User User { get; set; }

    #endregion
}
