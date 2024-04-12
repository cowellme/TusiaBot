using System.Text;

namespace GalleryBot
{
    public class Hasher
    {
        public static string NewHash(int i)
        {
            var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Remove(i)));
            return hash;
        }
    }
}
