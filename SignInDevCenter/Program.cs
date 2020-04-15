using System;

namespace SignInDevCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Microsoft!");

            SignInDevCenter demo = new SignInDevCenter();
            demo.Run().Wait();
        }
    }
}
