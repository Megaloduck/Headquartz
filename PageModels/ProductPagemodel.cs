using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headquartz.PageModels
{
    internal class ProductPagemodel : BasePageModel
    {
        private readonly Product _model;

        public string Name => _model.Name;
        public int Quantity => _model.Quantity;

        public string StockColor =>
            Quantity < 10 ? "Red" :
            Quantity < 30 ? "Orange" : "White";

        public ProductPageModel(Product model)
        {
            _model = model;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(StockColor));
        }
    }
}
