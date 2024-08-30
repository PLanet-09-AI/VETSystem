namespace BestReg.Data
{
    public class FeedingPlan
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public DateTime FeedingTime { get; set; }
        public float Quantity { get; set; }
        public string FoodType { get; set; }
        public Animal Animal { get; set; }
    }
}
