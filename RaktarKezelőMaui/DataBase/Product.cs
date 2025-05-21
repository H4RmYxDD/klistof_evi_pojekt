using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Category> Categories { get; set; }
        public double PirceEur { get; set; }
        public double PriceHuf { get; set; }
        public MeasureUnit ProductMeasureUnit { get; set; }
        public enum MeasureUnit 
        {
            Piece, 
            Kilogram,
            Liter
        }
        public double Quantity { get; set; }
    }
}
