using Newtonsoft.Json;
using System.IO;

namespace FlexiTools.Model
{
    public class Funcionario
    {
        public string? Nome { get; set; }
        public string? Departamento { get; set; }
        public string? Hash { get; set; }
        public string? Gerente { get; set; }
        public string? Email { get; set; }

        public static async Task<List<Funcionario>> GetFuncionarios(string filePath)
        {

            if (!File.Exists(filePath))
            {
                return new List<Funcionario>();
            }

            using (var reader = new StreamReader(filePath))
            {
                string json = await reader.ReadToEndAsync();
                var funcionarios = JsonConvert.DeserializeObject<List<Funcionario>>(json);
                return funcionarios ?? new List<Funcionario>();
            }

        }

        public static async Task SetFuncionarios(string filePath, List<Funcionario> funcionarios)
        {
            string json = JsonConvert.SerializeObject(funcionarios, Formatting.Indented);
            using (var writer = new StreamWriter(filePath))
            {
                await writer.WriteLineAsync(json);
            }
        }
    }
}
