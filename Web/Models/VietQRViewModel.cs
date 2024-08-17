namespace Web.Controllers
{
    public class VietQRViewModel
    {
        public string BankId { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
    public static class MyBank
    {
        public static readonly VietQRViewModel Bank = new VietQRViewModel
        {
            BankId = "MB",
            AccountNo = "0000227065289"
        };
    }
}
