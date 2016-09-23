namespace Northwind.DAL.Models
{
  public class OrderDetail
  {
    public Order Order { get; set; }
    public Product Product { get; set; }
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public float Discount { get; set; }
  }
}