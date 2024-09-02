namespace BestReg.Models
{
    using BestReg.Data;

    public class SupplierDashboardViewModel
    {
        public IEnumerable<InventoryItem> InventoryItems { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
        public IEnumerable<SupplierOrder> SupplierOrders { get; set; }
        // Add other relevant properties as needed
    }
}
