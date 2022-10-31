// See https://aka.ms/new-console-template for more information

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
        if (handle == null)
        {
            return false;
        }
        if (!handle.IsValid())
        {
            return false;
        }
        if (savedFiles.ContainsKey(handle.Index))
        {
            return true;
        }
        return false;
    }

    public FileHandle AddFile(string path)
    {
        if (File.Exists(Path.GetFullPath(path)))
        {
            savedFiles.Add(counter, Path.GetFullPath(path));
            return new FileHandle(counter++);
        }
        return new FileHandle();
    }

    public bool RemoveFile(FileHandle handle)
    {
        if (IsHandleValidInThisManager(handle))
        {
            savedFiles.Remove(handle.Index);
            return true;
        }
        return false;
    }

    public StreamReader? CreateReader(FileHandle handle)
    {
        if (IsHandleValidInThisManager(handle))
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
class SectionValue
{
    private int _value;
    private string _name;

    public int Get => _value;
    public string GetName => _name;
    
    public SectionValue(int value)
    {
        _value = value;
        _name = value.ToString();
    }

    public SectionValue(int value, string name)
    {
        _value = value;
        _name = name;
    }

    public override string ToString()
    {
        return _name;
    }
}

class WinCombination
{
    private List<SectionValue> _winCombination;

    public WinCombination(List<SectionValue> winComb)
    {
        _winCombination = new List<SectionValue>(winComb);
    }
    public override string ToString()
    {
        string winComb = new string("");
        foreach(SectionValue value in _winCombination)
        {
            winComb = winComb + value.ToString() + " ";
        }
        return winComb;
    }
}

class Darts
{
    List<SectionValue> _sectionValues;

    bool isAll;

    public Darts()
    {
        _sectionValues = new List<SectionValue>();

        for (int i = 1; i <= 20; i++)
        {
            _sectionValues.Add(new SectionValue(i));
            _sectionValues.Add(new SectionValue(i * 2, "D" + i.ToString()));
            _sectionValues.Add(new SectionValue(i * 3, "T" + i.ToString()));
        }

        _sectionValues.Add(new SectionValue(25));
        _sectionValues.Add(new SectionValue(50, "Bull"));
        isAll = false;
    }

    public List<WinCombination> FindWinCombinationsCount(int targetCount, int targetPoints)
    {
        List<WinCombination> result = new List<WinCombination>();
        List<SectionValue> partial = new List<SectionValue>();

        for (int i = 0; i < _sectionValues.Count; i++)
        {
            if (isAll) { break; }
            partial.Add(_sectionValues[i]);
            checkForWin(targetPoints, targetCount, ref partial, ref result);
            if (isAll) { break; }
            partial.Add(_sectionValues[i]);
            checkForWin(targetPoints, targetCount, ref partial, ref result);
            if (isAll) { break; }
            partial.Add(_sectionValues[i]);
            checkForWin(targetPoints, targetCount, ref partial, ref result);
            partial.Clear();

            for (int j = i + 1; j < _sectionValues.Count; j++)
            {
                if (isAll) { break; }
                partial.Add(_sectionValues[i]);
                partial.Add(_sectionValues[j]);
                checkForWin(targetPoints, targetCount, ref partial, ref result);
                if (isAll) { break; }
                partial.Add(_sectionValues[i]);
                checkForWin(targetPoints, targetCount, ref partial, ref result);
                if (isAll) { break; }
                partial[partial.Count - 1] = _sectionValues[j];
                checkForWin(targetPoints, targetCount, ref partial, ref result);
                partial.Clear();

                for (int k = j + 1; k < _sectionValues.Count; k++)
                {
                    if (isAll) { break; }
                    partial.Add(_sectionValues[i]);
                    partial.Add(_sectionValues[j]);
                    partial.Add(_sectionValues[k]);
                    checkForWin(targetPoints, targetCount, ref partial, ref result);
                    partial.Clear();
                }
            }
        }

        if (targetCount == result.Count)
        {
            Console.WriteLine("All elements found");

        }
        else if (targetCount > result.Count)
        {
            Console.WriteLine("Found count: " + result.Count);
        }

        isAll = false;
        return result;
    }

    private void checkForWin(int targetSum, int targetCount, ref List<SectionValue> partial, ref List<WinCombination> result)
    {
        int Sum = partial[0].Get;
        if (partial.Count > 1)
        {
            for(int i = 1; i < partial.Count; i++)
            {
                Sum += partial[i].Get;
            }
        }
        if(Sum == targetSum)
        {
            result.Add(new WinCombination(partial));
            if (targetCount == result.Count)
            {
                isAll = true;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        string path = new string("../../../files/");
        string inputFileName = new string("input.txt");
        string outputFileName = new string("output.txt");

        FileManager fileManager = new FileManager();
        FileHandle inputHandle = fileManager.AddFile(path + inputFileName);
        FileHandle outputHandle = fileManager.AddFile(path + outputFileName);
        if (!inputHandle.IsValid() || !outputHandle.IsValid())
        {
            Console.WriteLine("One of handles are not valid!");
            return;
        }

        StreamReader? sr = fileManager.CreateReader(inputHandle);
        StreamWriter? writer = fileManager.CreateWriter(outputHandle);
        if (sr == null || writer == null)
        {
            Console.WriteLine("Cant create reader for " + inputFileName + " or writer for " + outputFileName);
            return;
        }

        int remainingPoints = 0;
        if (!Int32.TryParse(sr.ReadLine(), out remainingPoints))
        {
            Console.WriteLine("Cant parse first line of input file to int");
            return;
        }

        int countOfWinCombinationsToFound = 0;
        if (!Int32.TryParse(sr.ReadLine(), out countOfWinCombinationsToFound))
        {
            Console.WriteLine("Cant parse second line of input file to int");
            return;
        }

        Darts darts = new Darts();

        List<WinCombination> FoundCombinations = new List<WinCombination>();
        FoundCombinations = darts.FindWinCombinationsCount(countOfWinCombinationsToFound, remainingPoints);

        foreach(WinCombination comb in FoundCombinations)
        {
            Console.WriteLine(comb.ToString());
            writer.WriteLine(comb.ToString());
        }

        sr.Close();
        writer.Close();
        
    }
}

