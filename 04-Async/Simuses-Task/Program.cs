using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int max = 5000000;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine(sw.Elapsed + " start calculating sinuses");
            var sins = string.Join("\r\n", 
                Enumerable.Range(0, max)
                .AsParallel()
                .Select(x => new {x, sinX=Math.Sin(x)})
                .Where(pair => pair.sinX > 0)
                .OrderBy(pair => pair.sinX)
                .Select(pair => pair.ToString()));
            byte[] data = Encoding.ASCII.GetBytes(sins);
            Console.WriteLine(sw.Elapsed + " start writing sinuses");
            var fs = new FileStream("sins.txt", 
                FileMode.Create, FileAccess.Write, FileShare.None, 4096, 
                FileOptions.WriteThrough 
                | FileOptions.Asynchronous
                );
//            fs.Write(data, 0, data.Length);
//            Console.WriteLine(sw.Elapsed + " sinuses written!");
            fs.BeginWrite(data, 0, data.Length, res =>
            {
                Console.WriteLine(sw.Elapsed + " sinuses written!");
                fs.EndWrite(res);
                fs.Dispose();
                data = null;
            }, null);
//            Console.WriteLine(sw.Elapsed + " start calculating cosinuses");
//            var coses = Enumerable.Range(0, max)
//                .AsParallel()
//                .Select(x => new { x, cosX = Math.Cos(x) })
//                .Where(pair => pair.cosX > 0)
//                .OrderBy(pair => pair.cosX)
//                .Select(pair => pair.ToString());
//            Console.WriteLine(sw.Elapsed + " start writing cosinuses");
//            var cosinuses = string.Join("\r\n", coses);
//            File.WriteAllText("cosinuses.txt", cosinuses);
//            Console.WriteLine(sw.Elapsed);
        }
    }
}
