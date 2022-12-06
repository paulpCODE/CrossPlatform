using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace LabLib;

class Pair
{
    public int first, second;

    public Pair(int first, int second)
    {
        this.first = first;
        this.second = second;
    }
}

class Node
{
    public int vertexNumber;

    public List<Pair> children;

    public Node(int vertexNumber)
    {
        this.vertexNumber = vertexNumber;
        children = new List<Pair>();
    }
    public void Add_child(int vNumber, int length)
    {
        Pair p = new Pair(vNumber, length);
        children.Add(p);
    }
}



public static class Program
{
    static int[] dijkstraDist(List<Node> g, int s, int[] path)
    {
        int[] dist = new int[g.Count];

        bool[] visited = new bool[g.Count];
        for (int i = 0; i < g.Count; i++)
        {
            visited[i] = false;
            path[i] = -1;
            dist[i] = Int32.MaxValue;
        }
        dist[s] = 0;
        path[s] = -1;
        int current = s;


        HashSet<int> sett = new HashSet<int>();

        while (true)
        {
            visited[current] = true;
            for (int i = 0;
                    i < g[current].children.Count;
                    i++)
            {
                int v = g[current].children[i].first;
                if (visited[v])
                    continue;

                sett.Add(v);
                int alt = dist[current] +
                         g[current].children[i].second;

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    path[v] = current;
                }
            }
            sett.Remove(current);

            if (sett.Count == 0)
                break;

            int minDist = Int32.MaxValue;
            int index = 0;

            foreach (int a in sett)
            {
                if (dist[a] < minDist)
                {
                    minDist = dist[a];
                    index = a;
                }
            }
            current = index;
        }
        return dist;
    }

    public static void Main()
    {
        string input = "input.txt";
        string output = "output.txt";

        string[] lines;
        string result = "";
        try
        {
            lines = File.ReadAllLines(input);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Could not find file: " + e.FileName);
            return;
        }

        var nodesnum = Int32.Parse(lines[0].Split(" ")[0]);
        var edgesnum = Int32.Parse(lines[0].Split(" ")[1]);
        if (lines.Length != edgesnum + 1)
        {
            throw new ArgumentException($"You must have {edgesnum + 1} lines in input file");
        }

        if (nodesnum < 1 || nodesnum > 100)
        {
            throw new ArgumentException($"Number of nodes must be in range [1, 1000]");
        }

        if (edgesnum < 0 || edgesnum > 10000)
        {
            throw new ArgumentException($"Number of edges must be in range [1, 1000]");
        }



        List<Node> v = new List<Node>();
        int n = 4, s = 0;

        for (int i = 0; i < n; i++)
        {
            Node a = new Node(i);
            v.Add(a);
        }

        foreach (var edge in lines.Skip(1))
        {
            var temp = edge.Split(" ");
            if (temp.Length != 3)
            {
                throw new ArgumentException($"You must have 3 numbers in each line");
            }
            var from = Int32.Parse(temp[0]);
            var to = Int32.Parse(temp[1]);
            var weight = Int32.Parse(temp[2]);
            if (weight < -100 || weight > 100)
            {
                throw new ArgumentException($"Weight of edge must be in range [-100, 100]");
            }

            v[from-1].Add_child(to-1, weight);
        }

        int[] path = new int[v.Count];
        int[] dist = dijkstraDist(v, s, path);

        for (int i = 0; i < dist.Length; i++)
        {
            if (dist[i] == Int32.MaxValue)
            {
                result += "30000" + " ";
                continue;
            }
            result += dist[i] + " ";

        }

        try
        {
            File.WriteAllText(output, result);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Could not find file: " + e.FileName);
            return;
        }
        Console.WriteLine(result);
    }
}
