using System;

namespace Rextester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            float originalFahrenheit;
            
            Console.Write("Enter temperature (Fahrenheit): ");            
            originalFahrenheit = float.Parse(Console.ReadLine());
			
			float celsius = ((originalFahrenheit - 32) / 9) * 5;			
			float fahrenheit = ((celsius * 9) / 5) + 32;
			
			Console.WriteLine(originalFahrenheit + " degrees Fahrenheit is " + celsius + " in Celsius");
			Console.WriteLine(celsius + " degrees Celsius is " + fahrenheit + " in Fahrenheit");
            
        }
    }
}
