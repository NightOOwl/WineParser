using System.Reflection;
using System.Text;


public class FileWriter
{
    private readonly string directoryPath;

    public FileWriter()
    {
        directoryPath = Directory.GetParent(Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location)).Parent.Parent.FullName;

        // Обновляем директорию, если это необходимо
        directoryPath = Path.Combine(directoryPath, "Data");

        // Создаем директорию, если она не существует
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); 
            Console.WriteLine("Создана директория Data");
        }       
    }

    public async Task WriteToFileAsync(string fileName, IEnumerable<string> data)
    {
        string filePath = Path.Combine(directoryPath, fileName);

        
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            await Console.Out.WriteLineAsync("Запись данных в файл...");
            
            foreach (var line in data)
            {
                await writer.WriteLineAsync(line);
            }
        }
        await Console.Out.WriteLineAsync("Запись завершена.");
    }
}
