using System;

namespace XNA_TPS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TPSGame game = new TPSGame())
            {
                game.Run();
            }
        }
    }
}

