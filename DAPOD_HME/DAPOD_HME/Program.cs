using System;

namespace DAPOD_HME
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (DAPOD_STARTER game = new DAPOD_STARTER())
            {
                game.Run();
            }
        }
    }
#endif
}

