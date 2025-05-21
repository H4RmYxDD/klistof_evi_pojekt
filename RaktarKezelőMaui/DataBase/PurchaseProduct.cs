using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    [PrimaryKey(nameof(ProductId), nameof(PurchaseId))]

    public class PurchaseProduct
    {
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
