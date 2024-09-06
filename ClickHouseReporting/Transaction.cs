using System.Text.Json.Serialization;

namespace ClickHouseReporting;

public class Transaction
{
    [JsonPropertyName("siteid")]
    public int SiteId { get; set; }

    [JsonPropertyName("paymentsystemid")]
    public int PaymentSystemId { get; set; }

    [JsonPropertyName("paymentsystemtransactionid")]
    public string PaymentSystemTransactionId { get; set; }

    [JsonPropertyName("merchanttransactionid")]
    public string MerchantTransactionId { get; set; }

    [JsonPropertyName("creationdate")]
    public string CreationDate { get; set; }

    [JsonPropertyName("lastupdatedate")]
    public string LastUpdateDate { get; set; }

    [JsonPropertyName("operationtype")]
    public int OperationType { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("initamount")]
    public decimal InitAmount { get; set; }

    [JsonPropertyName("currency")]
    public int Currency { get; set; }

    [JsonPropertyName("paymentmethod")]
    public int PaymentMethod { get; set; }

    [JsonPropertyName("isblockedtransaction")]
    public bool IsBlockedTransaction { get; set; } = false;

    [JsonPropertyName("language")]
    public string Language { get; set; }
}