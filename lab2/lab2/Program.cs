using System;

public class FileHandle
{
    private static int INVALID_INDEX = -1;

    private int index;

    public int Index { get { return index; } }
    public FileHandle()
    {
        index = INVALID_INDEX;
    }

    public FileHandle(int index)
    {
        this.index = index;
    }

    public bool IsValid()
    {
        return index != INVALID_INDEX;
    }
}

public class FileManager
{
    private static int counter = 0;

    private SortedDictionary<int, string> savedFiles;

    public SortedDictionary<int, string> SavedFiles { get { return savedFiles; } }
    public FileManager()
    {
        savedFiles = new SortedDictionary<int, string>();
    }

    public bool IsHandleValidInThisManager(FileHandle handle)
    {
        if(handle == null)
        {
            return false;
        }
        if(!handle.IsValid())
        {
            return false;
        }
        if(savedFiles.ContainsKey(handle.Index))
        {
            return true;
        }
        return false;
    }

    public FileHandle AddFile(string path)
    {
        if(File.Exists(Path.GetFullPath(path)))
        {
            savedFiles.Add(counter, Path.GetFullPath(path));
            return new FileHandle(counter++);
        }
        return new FileHandle();
    }

    public bool RemoveFile(FileHandle handle)
    {
        if(IsHandleValidInThisManager(handle))
        {
            savedFiles.Remove(handle.Index);
            return true;
        }
        return false;
    }

    public StreamReader? CreateReader(FileHandle handle)
    {
        if(IsHandleValidInThisManager(handle))
        {
            return File.OpenText(savedFiles[handle.Index]); 
        }
        return null;
    }

    public StreamWriter? CreateWriter(FileHandle handle)
    {
        if (IsHandleValidInThisManager(handle))
        {
            return File.CreateText(savedFiles[handle.Index]);
        }
        return null;
    }
}

public class Platforms
{ 
    public List<int> Heights { get; set; }

    public Platforms()
    {
        Heights = new List<int>();
    }

    public int DistanceBetweenPlatforms(int index1, int index2)
    {
        if(Heights.Count > index1 && Heights.Count > index2 && index1 >= 0 && index2 >= 0)
        {
            return Math.Abs(Heights[index2] - Heights[index1]);
        }
        return -1;
    }

    public int SuperSkillBetween(int index1, int index2)
    {
        if (Heights.Count > index1 && Heights.Count > index2 && index1 >= 0 && index2 >= 0)
        {
            return 3 * Math.Abs(Heights[index2] - Heights[index1]);
        }
        return -1;
    }

    /*public int FindMinimalRecursive(int index)
    {
        int sum = 0;
        int minimal = 0;
        if (index + 2 < Heights.Count)
        {
            minimal = DistanceBetweenPlatforms(index, index + 1);
            if (DistanceBetweenPlatforms(index, index + 1) + DistanceBetweenPlatforms(index + 1, index + 2) > SuperSkillBetween(index, index+2))
            {
                int v1min = FindMinimalRecursive(index + 1);
                int v2min = FindMinimalRecursive(index + 2);
                sum += v1min > v2min ? v1min : v2min;
            }
            else
            {
                sum += FindMinimalRecursive(index + 1);
            }
        }
        else if(index + 1 < Heights.Count)
        {
            return DistanceBetweenPlatforms(index, index + 1);
        }
        else
        {
            return 0;
        }
        
    }*/

    public int CalculateMinimalValue()
    {
        int sumOfPoints = 0;
        int minimal = 0;
        for(int i = 0; i < Heights.Count - 1; i++)
        {
            if(i + 2 < Heights.Count)
            {
                minimal = DistanceBetweenPlatforms(i, i + 1) + DistanceBetweenPlatforms(i + 1, i + 2);
                if(minimal > SuperSkillBetween(i, i+2))
                {
                    minimal = SuperSkillBetween(i, i+2);
                }
                sumOfPoints += minimal;
                i++;
            }
            else if(i + 1 < Heights.Count)
            {
                sumOfPoints += DistanceBetweenPlatforms(i, i + 1);
            }
        }

        return sumOfPoints;
    }
}


public class Lab2
{
    public static void Main()
    {
        string path = new string("../../../files/");
        string inputFileName = new string("input.txt");
        string outputFileName = new string("output.txt");
        string delimeter = new string(" ");

        Platforms platforms = new Platforms();

        FileManager fileManager = new FileManager();
        FileHandle inputHandle = fileManager.AddFile(path + inputFileName);
        FileHandle outputHandle = fileManager.AddFile(path + outputFileName);
        if(!inputHandle.IsValid() || !outputHandle.IsValid())
        {
            Console.WriteLine("One of handles are not valid!");
            return;
        }

        StreamReader? sr = fileManager.CreateReader(inputHandle);
        StreamWriter? writer = fileManager.CreateWriter(outputHandle);
        if(sr == null || writer == null)
        {
            Console.WriteLine("Cant create reader for " + inputFileName + " or writer for " + outputFileName);
            return;
        }

        int numberOfPlatforms = 0;
        if(!Int32.TryParse(sr.ReadLine(), out numberOfPlatforms))
        {
            Console.WriteLine("Cant parse first line of input file to int");
            return;
        }
        if(numberOfPlatforms < 0)
        {
            Console.WriteLine("numberOfPlatforms cant be a negative number");
            return;
        }
        string? dataPlatforms = sr.ReadLine();
        if(dataPlatforms == null)
        {
            Console.WriteLine("Second line of input is null");
            return;
        }
        string[] words = dataPlatforms.Split(delimeter);
        foreach(string word in words)
        {
            if(Int32.TryParse(word, out int height)) {
                platforms.Heights.Add(height);
            } 
            else
            {
                Console.WriteLine("Cant convert data from second line to ints with delimeter: " + delimeter);
                return;
            }
        }

        writer.WriteLine(platforms.CalculateMinimalValue());
        Console.WriteLine("Success!");

        sr.Close();
        writer.Close();
    }
};
