using System.Reflection;
using System.Text;


public class FileWriter
{
    private readonly string directoryPath;

    public FileWriter()
    {
        directoryPath = Directory.GetParent(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location)).Parent.Parent.FullName;

        directoryPath = Path.Combine(directoryPath, "Data");
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine("Создана директория Data");
        }
    }

    public async Task WriteToFileAsync(string fileName, string data)
    {
        string filePath = Path.Combine(directoryPath, fileName);

        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            await Console.Out.WriteLineAsync("Запись данных в файл...");
            await writer.WriteLineAsync(data);
        }

        await Console.Out.WriteLineAsync("Запись завершена.");
    }
}
