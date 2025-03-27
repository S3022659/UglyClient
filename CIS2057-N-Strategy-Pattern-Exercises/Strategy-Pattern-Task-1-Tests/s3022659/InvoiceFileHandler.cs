public class InvoiceFileUtility {
    public static string? ReadFile(string filePath, TextWriter outputErrorTo)
    {
        try {
            using(StreamReader sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
        catch (Exception ex) {
            outputErrorTo.WriteLine($"Error! Unable to open the file {filePath} for reading");
            outputErrorTo.WriteLine(ex.Message);
            return null;
        }
    }

    public static void WriteFile(string filePath, string content, TextWriter outputErrorTo)
    {
        try
        {
            using(StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(content);
            }
        }
        catch(Exception ex)
        {
            outputErrorTo.WriteLine($"Error! Unable to open the file {filePath} for writing");
        }
    }
}
