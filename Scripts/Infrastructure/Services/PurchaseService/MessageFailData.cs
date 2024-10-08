namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public class MessageFailData : IFailPurchaseData
    {
        public FailPurchaseType FailType => FailPurchaseType.Message;
        public string Message { get; }
 
        public MessageFailData(string message)
        {
            Message = message;
        }
    }
}