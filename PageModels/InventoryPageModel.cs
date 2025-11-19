using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Headquartz.Models;

namespace Headquartz.PageModels
{
    public class InventoryPageModel : BasePageModel
    {
        private readonly GameState _state;
        private readonly ISimulationEngine _engine;

        public ObservableCollection<ProductPageModel> Products { get; set; }

        public ICommand RestockCommand { get; }

        public InventoryPageModel(GameState state, ISimulationEngine engine)
        {
            _state = state;
            _engine = engine;

            Products = new ObservableCollection<ProductPageModel>(
                _state.Inventory.Products.Select(p => new ProductPageModel(p)));

            RestockCommand = new Command<string>(Restock);

            // update UI every tick
            _engine.OnTicked += _ => Refresh();
        }

        private void Restock(string productName)
        {
            var p = _state.Inventory.Products.First(x => x.Name == productName);
            p.Quantity += 10;
            Refresh();
        }

        private void Refresh()
        {
            foreach (var p in Products)
                p.Refresh();
        }
    }
}
