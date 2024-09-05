using System.Reflection;

namespace NEGOCIO
{
    public static class GeneraLog
    {
        public static string LogError(string message, string capa)
        {
            string logFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string file = $"{capa}_{timestamp}_" + Guid.NewGuid().ToString()[..4] + ".txt";
            string routeFile = Path.Combine(logFolder, file);

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            using StreamWriter writer = new(routeFile);
            writer.WriteLine($"Error: {message}");

            return file;
        }
    }
}
