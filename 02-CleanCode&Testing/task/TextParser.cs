using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace parse
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: parse <filename> <n>");
                Console.WriteLine("  print <n>-th column of space separated file <filename>");
                return;
            }
            // Задание 1.
            // Дайте переменным в этом методе правильные имена. 
            // Обязательно обсудите выбор имен в паре.
            // Переименовать что-то — F2
            var first = args[0];
            var second = int.Parse(args[1]);
            var ss = File.ReadAllLines(first);
            var p = new Parser('"', '\'');
            foreach (var s in ss)
            {
                var fff = p.SplitToFields(s);
                var tmp = (fff.Length < second ? "" : fff[second - 1]);
                Console.WriteLine(tmp);
            }
        }
    }

    public class Parser
    {
        private char[] quoteChars;

        public Parser(params char[] quoteChars)
        {
            this.quoteChars = quoteChars;
        }

        public string[] SplitToFields(string line)
        {
            var pos = 0;
            var res = new List<string>();
            while (pos < line.Length)
            {
                while (pos < line.Length && line[pos] == ' ')
                    pos++;
                if (pos < line.Length)
                {
                    var token = ReadField(line, pos);
                    res.Add(token);
                    pos += token.Length;
                }
            }
            return res.ToArray();
        }

        public string ReadField(string line, int startPos)
        {
            // Задание 2.
            // Выделите в отдельные методы блоки кода, ответственные за чтение закавыченного и простого полей.
            // Выделение метода — Ctrl+R+M (VS hotkey) или Ctrl+Alt+M (Resharper hotkey)

            if (quoteChars.Contains(line[startPos]))
            {
                var q = line[startPos];
                var pos = startPos + 1;
                while (pos < line.Length)
                {
                    if (line[pos] == q)
                        return line.Substring(startPos, pos - startPos + 1);
                    if (line[pos] == '\\')
                        pos++;
                    pos++;
                }
                return line.Substring(startPos);
            }
            else
            {
                var pos = startPos;
                while (pos < line.Length && line[pos] != ' ') pos++;
                return line.Substring(startPos, pos - startPos);
            }
        }
    }
}
