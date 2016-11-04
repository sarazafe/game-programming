
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rextester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int age = 29;
            
            Console.WriteLine(age);
            
            int MaxScore = 100;
            int score = 50;
            
            float percent = (float)score / MaxScore;
            
            Console.WriteLine(percent);
        }
    }
}
