namespace Microsoft.eShopWeb.ApplicationCore.Entities.WishlistAggregate
{
    public class WishlistItem : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CatalogItemId { get; set; }
        public int WishlistId { get; private set; }
    }
}