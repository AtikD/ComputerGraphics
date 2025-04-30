using System.IO;

namespace TomogramVisualizer
{
    internal class Bin
    {
        public static int X, Y, Z;
        public static short[] array;
        public Bin() { }

        public void readBin(string path)
        {   

            if (File.Exists(path))
            {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                Z = reader.ReadInt32();

                int arraysize = X * Y * Z;
                array = new short[arraysize];
                for (int i = 0; i < arraysize; i++)
                {
                    array[i] = reader.ReadInt16();
                }
            }
        }
    }
}
