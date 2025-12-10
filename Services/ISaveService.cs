using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Services
{
    public interface ISaveService
    {
        Task SaveAsync(GameState state, string path);
        Task<GameState> LoadAsync(string path);
    }
}
