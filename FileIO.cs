namespace CourseDB;

public class FileIO
{
    public static bool Write(string buffer, string path)
    {
        try
        {
            new StreamWriter(path).WriteAsync(buffer); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
    
    public static string Read(string path)
    {
        try
        {
            return (new StreamReader(path)).ReadToEndAsync().Result; 
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}   

