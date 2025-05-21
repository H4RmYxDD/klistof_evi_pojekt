using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Purchase
    {
        public int Id { get; set; }
        public List<Product> Products { get; set; }
        public string BuyerName { get; set; }
        public DateTime BuyingTime { get; set; }
        public Status PurchaseStatus { get; set; } 

        public enum Status
        {
            New,
            Delivered,
            Canceled
        }
    }
}
