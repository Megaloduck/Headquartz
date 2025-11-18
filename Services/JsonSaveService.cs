using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Headquartz.Models;

namespace Headquartz.Services
{
    public class JsonSaveService : ISaveService
    {
        private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };


        public async Task SaveAsync(GameState state, string path, CancellationToken ct = default)
        {
            using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, state, _opts, ct);
        }


        public async Task<GameState?> LoadAsync(string path, CancellationToken ct = default)
        {
            if (!File.Exists(path)) return null;
            using var fs = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<GameState>(fs, _opts, ct);
        }
    }
}
