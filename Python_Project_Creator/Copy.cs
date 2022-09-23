using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopySpace
{
    public class Copy
    {
        public static void CopyDirectory(string directory, string target)
        {
            Directory.CreateDirectory(target);

            //Get files ad copy them to the target
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                string[] file_array = file.Split("\\");
                File.Copy(file, Path.Combine(target, file_array[file_array.Length - 1]));
            }

            //Get directoriesstring
            string[] dirs = Directory.GetDirectories(directory);
            foreach (string dir in dirs)
            {
                string[] directory_array = dir.Split("\\");
                CopyDirectory(Path.Combine(directory, directory_array[directory_array.Length - 1]), Path.Combine(target, directory_array[directory_array.Length - 1]));
            }

        }

    }
}
