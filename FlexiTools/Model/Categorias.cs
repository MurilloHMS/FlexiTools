using Newtonsoft.Json;
using System.IO;

namespace FlexiTools.Model
{
    public class Categorias
    {
        public string? Nome { get; set; }

        public static async Task<List<Categorias>> GetCategoriasAsync(string filePath)
        {

            if (!File.Exists(filePath))
            {
                return new List<Categorias>();
            }

            using (var reader = new StreamReader(filePath))
            {
                string json = await reader.ReadToEndAsync();
                var categorias = JsonConvert.DeserializeObject<List<Categorias>>(json);
                return categorias ?? new List<Categorias>();
            }

        }

        public static async Task SetCategoriasAsync(string filePath, List<Categorias> categorias)
        {
            string json = JsonConvert.SerializeObject(categorias, Formatting.Indented);
            using (var writer = new StreamWriter(filePath))
            {
                await writer.WriteLineAsync(json);
            }
        }
    }
}
