using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace LinqAdditionalTask
{
    class Program
    {
        static void Main(string[] args)
        {
            GetAllNamesAsString(" ");

            LengthGreaterThanIndex();

            SortPhrase(@"Это что же получается: ходишь, ходишь в школу, а потом бац - вторая смена");

            BlindBook(@"This dog eats too much vegetables after lunch", 3);
        }

        public static void GetAllNamesAsString(string delimeter)
        {
            var classWithPropertyName = new List<ClassWithPropertyName>
            {
                new ClassWithPropertyName { Name = "first" },
                new ClassWithPropertyName { Name = "second" },
                new ClassWithPropertyName { Name = "third" },
                new ClassWithPropertyName { Name = "fourth" },
                new ClassWithPropertyName { Name = "fifth" }
            };

            Console.WriteLine(classWithPropertyName.Select(query => query.Name).Skip(3).Aggregate((a, b) => a + delimeter + b));

        }

        public static void LengthGreaterThanIndex()
        {
            var strings = new List<string>
            {
                "ss",
                "s",
                "sss"
            };

            foreach (var s in strings.Select((v, i) => new { Index = i, _Length = v.Length, Data = v }).Where(s => s._Length > s.Index).Select(q => q.Data))
            {
                Console.WriteLine(s);
            }
        }

        public static void SortPhrase(string phrase)
        {
            char[] punctuation = {'.', ',', ':', '-', ' ', '!', '?', ';'};
            int i = 1;
            var s = phrase.Split(' ').
                Select(q => q.Trim(punctuation)).
                Where(q => q.Length > 0).
                GroupBy(q => q.Length,
                (length, words) => new
                {
                    GroupNumber = i++, 
                    Length = length, 
                    Count = words.Count(),
                    Words = words.Aggregate((a, b) => a + "\n" + b)
                }
                ).OrderByDescending(q => q.Count);

            foreach (var result in s)
            {
                Console.Write("\nГруппа " + result.GroupNumber);
                Console.Write(". Длина " + result.Length);
                Console.Write(". Количество " + result.Count + ".\n");
                Console.WriteLine(result.Words);
            }
        }

        public static void BlindBook(string text, int n)
        {
            var engRuDictionary = new Dictionary<string, string>
            {
                { "this", "эта" },
                { "dog", "собака" },
                { "eats", "ест" },
                { "too", "слишком" },
                { "much", "много" },
                { "vegetables", "овощи" },
                { "after", "после" },
                { "lunch", "ланч" }
            };

            var result = text.Split(' ').ToList()
                .Select((v, i) => new {Index = i, Value = engRuDictionary[v.ToLower()].ToUpper()})
                .GroupBy(q => q.Index / n, (f, words) => new 
                {
                    bookString = words.Select(q=> q.Value).Aggregate((a, b) => a + " " + b) + "\n"
                });

            foreach (var v in result)
            {
                Console.Write(v.bookString);
            }
        }
    }

    class ClassWithPropertyName
    {
        public string Name { get; set; }
    }
}
