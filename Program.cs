using System;

namespace WreckingBall
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new WreckingBall())
                game.Run();
        }
    }
}
