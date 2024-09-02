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
        public InventoryItem Item { get; set; } // Reference to InventoryItem
        public int QuantityOrdered { get; set; } // Quantity of this item ordered
    }

    public class SupplierOrder
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } // Name of the supplier
        public DateTime OrderDate { get; set; } // Date the order was placed
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); // List of items in the order, initialized to avoid null references
    }
}
