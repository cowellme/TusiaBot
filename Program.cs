using GalleryBot;

namespace TusiaBot
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                RTelegeram.Start();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}