namespace BestReg.Data
{
    public class NutritionStock
    {
        public int Id { get; set; }
        public string FoodType { get; set; }
        public float Quantity { get; set; }
        public string Supplier
        {
            get; set;
        }

    }
}
