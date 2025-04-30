namespace Filter
{
    public class Filter
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static Bitmap Execute(Bitmap source)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            float[,] kernel = { { 0, +1, 0 }, { -1, 0, +1 }, { 0, -1, 0 } };
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    int radiusX = kernel.GetLength(0) / 2;
                    int radiusY = kernel.GetLength(1) / 2;
                    float resultR = 0;
                    float resultG = 0;
                    float resultB = 0;
                    for (int l = -radiusY; l <= radiusY; l++)
                    {
                        for (int k = -radiusX; k <= radiusX; k++)
                        {
                            int idX = Clamp(i + k, 0, source.Width - 1);
                            int idY = Clamp(j + l, 0, source.Height - 1);
                            Color neighborColor = source.GetPixel(idX, idY);
                            resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                            resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                            resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(
                        Clamp((int)resultR, 0, 255),
                        Clamp((int)resultG, 0, 255),
                        Clamp((int)resultB, 0, 255)));
                }
            }
            return result;
        }
    }
}