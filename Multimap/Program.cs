﻿using System;

namespace Multimap
{
    class Program
    {
        static void Main(string[] args)
        {
            var v1 = new MyClass("car");
            var v2 = new MyClass("plane");
            var v3 = new MyClass("NPC2");
            var test = new MultiMap<string, MyClass>();

            test.Add("vehicle", v1);
            test.Add("vehicle", v2);
            test.Add("npc", v3);
            test.Add("npc", v3);
            test.Add("npc", v3);

            Console.WriteLine("Keys: {0}", String.Join(", ", test.Keys));
            Console.WriteLine("Values: {0}", String.Join(", ", test.Values));
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["pc"]), "pc");
            Console.WriteLine("Contains {1}->{2}: {0}", test.ContainsValue("vehicle", v1), "vehicle", v1);
            Console.WriteLine("Contains {1}->{2}: {0}", test.ContainsValue("vehicles", v1), "vehicles", v1);

            test.Remove("npc", v3);
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            test.Remove("npc", v3);
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            test.Remove("npc", v3);
            Console.WriteLine("Contains {1}: {0}", test.ContainsKey("npc"), "npc");

            Console.ReadLine();
        }
    }
}