namespace BestReg.Data
{
    public class CheckInRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; }
      
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}
