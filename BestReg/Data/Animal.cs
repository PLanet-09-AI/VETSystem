namespace BestReg.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public DateTime DateOfBirth { get; set; }
        // Initialize these collections in the constructor
        public List<DiagnosisRecord> DiagnosisRecords { get; set; } = new List<DiagnosisRecord>();
        public List<VaccinationSchedule> VaccinationSchedules { get; set; } = new List<VaccinationSchedule>();
        public List<FeedingPlan> FeedingPlans { get; set; } = new List<FeedingPlan>();
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }

   

    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PricePerUnit { get; set; }
        public int QuantityInStock { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public InventoryItem Item { get; set; }
        public int QuantityOrdered { get; set; }
        public int SupplierOrderId { get; set; }
    }

    public class SupplierOrder
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}
