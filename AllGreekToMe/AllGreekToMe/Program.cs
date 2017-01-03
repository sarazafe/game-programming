using System;
using System.Globalization;

namespace AllGreekToMe
{
    /// <summary>
    /// Programming Assigment 1: program that calculates the distance and the angle (in degrees) between two points
    /// </summary>
    class Program
    {
        /// <summary>
        /// Programming Assigment 1: program that calculates the distance and the angle (in degrees) between two points
        /// </summary>
        /// <param name="args">command-line args</param>
        static void Main(string[] args)
        {
            // Welcome message
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|                                                                                                    |");
            Console.WriteLine("| Welcome to 'It's All Greek to Me!'. Let's calculate the distance and the angle between two points! |");
            Console.WriteLine("|                                                                                                    |");
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
            Console.WriteLine("");

            // Getting values of Point 1
            Console.WriteLine("Introduce the value for X of Point 1: ");
            float point1X = float.Parse(Console.ReadLine());
            Console.WriteLine("Introduce the value for Y of Point 1: ");
            float point1Y = float.Parse(Console.ReadLine());

            // Getting values of Point 2
            Console.WriteLine("Introduce the value for X of Point 2: ");
            float point2X = float.Parse(Console.ReadLine());
            Console.WriteLine("Introduce the value for Y of Point 2: ");
            float point2Y = float.Parse(Console.ReadLine());

            // Calculating deltas values
            float deltaX = point2X - point1X;
            float deltaY = point2Y - point1Y;

            // Calculating distance between 2 points (Pythagorean Theorem)
            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            // Calculating the angle from Point 1 to Point 2
            double angle = Math.Atan2(deltaY, deltaX);

            // Angle in degrees
            double angleInDegrees = (angle * (180.0 / Math.PI));

            // Printing the results
            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|\t* The distance between Point 1 (" + point1X + ", " + point1Y + ") and Point 2 (" + point2X + ", " + point2Y + ") is: " + distance.ToString("F3",
                        CultureInfo.InvariantCulture));
            Console.WriteLine("|");
            Console.WriteLine("|\t* The angle (in degrees) between Point 1 (" + point1X + ", " + point1Y + ") and Point 2 (" + point2X + ", " + point2Y + ") is: " + angleInDegrees.ToString("F3",
                  CultureInfo.InvariantCulture));
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
        }
    }
}
