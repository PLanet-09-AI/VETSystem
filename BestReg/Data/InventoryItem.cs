namespace BestReg.Data
{
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
        public InventoryItem Item { get; set; }
        public int QuantityOrdered { get; set; }

    }
    public class SupplierOrder
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
    }

}
