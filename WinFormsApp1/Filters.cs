using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WinFormsApp1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap source, int x, int y);

        public virtual Bitmap processImage(Bitmap source, BackgroundWorker worker)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Width; i++)
            {
                worker.ReportProgress((int)((float)i / result.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < source.Height; j++)
                {
                    Color resultColor = calculateNewPixelColor(source, i, j);
                    result.SetPixel(i, j, resultColor);
                }
            }
            return result;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }

    class PerfectReflectorFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            // Этот метод будет перекрыт полной реализацией processImage
            return source.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap source, BackgroundWorker worker)
        {
            int width = source.Width;
            int height = source.Height;
            int maxR = 0, maxG = 0, maxB = 0;

            // Находим максимальные значения каналов
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress((int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    maxR = Math.Max(maxR, pixel.R);
                    maxG = Math.Max(maxG, pixel.G);
                    maxB = Math.Max(maxB, pixel.B);
                }
            }

            // Проверка на случай, если maxR, maxG или maxB равны 0
            maxR = Math.Max(1, maxR);
            maxG = Math.Max(1, maxG);
            maxB = Math.Max(1, maxB);

            Bitmap result = new Bitmap(width, height);

            // Применяем фильтр "Идеальный отражатель"
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress(50 + (int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    int newR = (pixel.R * 255) / maxR;
                    int newG = (pixel.G * 255) / maxG;
                    int newB = (pixel.B * 255) / maxB;

                    result.SetPixel(i, j, Color.FromArgb(
                        Clamp(newR, 0, 255),
                        Clamp(newG, 0, 255),
                        Clamp(newB, 0, 255)
                    ));
                }
            }

            return result;
        }
    }

    class DilationFilter : Filters
    {
        private int[,] structElem =
        {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 }
        };

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            bool shouldExpand = false;
            Color maxColor = source.GetPixel(x, y);

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int newX = x + i;
                    int newY = y + j;

                    if (newX >= 0 && newX < source.Width &&
                        newY >= 0 && newY < source.Height &&
                        structElem[j + 1, i + 1] == 1)
                    {
                        Color pixel = source.GetPixel(newX, newY);
                        if (pixel.GetBrightness() > maxColor.GetBrightness())
                        {
                            maxColor = pixel;
                            shouldExpand = true;
                        }
                    }
                }
            }

            return shouldExpand ? maxColor : source.GetPixel(x, y);
        }
    }

    class ErosionFilter : Filters
    {
        private int[,] structElem =
        {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 }
        };

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            Color minColor = source.GetPixel(x, y);
            bool shouldErode = false;

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int newX = x + i;
                    int newY = y + j;
                    if (newX >= 0 && newX < source.Width &&
                        newY >= 0 && newY < source.Height &&
                        structElem[j + 1, i + 1] == 1)
                    {
                        Color pixel = source.GetPixel(newX, newY);
                        if (pixel.GetBrightness() < minColor.GetBrightness())
                        {
                            minColor = pixel;
                            shouldErode = true;
                        }
                    }
                }
            }

            return shouldErode ? minColor : source.GetPixel(x, y);
        }
    }

    class MedianFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            List<int> reds = new List<int>();
            List<int> greens = new List<int>();
            List<int> blues = new List<int>();

            // Собираем пиксели в окно 3x3
            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int neighborX = Clamp(x + i, 0, source.Width - 1);
                    int neighborY = Clamp(y + j, 0, source.Height - 1);
                    Color pixel = source.GetPixel(neighborX, neighborY);
                    reds.Add(pixel.R);
                    greens.Add(pixel.G);
                    blues.Add(pixel.B);
                }
            }

            // Берём медиану по каждому цветовому каналу
            int medianR = GetMedian(reds);
            int medianG = GetMedian(greens);
            int medianB = GetMedian(blues);

            return Color.FromArgb(medianR, medianG, medianB);
        }

        private int GetMedian(List<int> values)
        {
            values.Sort();
            return values[values.Count / 2];
        }
    }

    class SobelFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            float[,] kernelX = new float[3, 3]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            float[,] kernelY = new float[3, 3]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            float gradientXR = 0, gradientYR = 0;
            float gradientXG = 0, gradientYG = 0;
            float gradientXB = 0, gradientYB = 0;

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int neighborX = Clamp(x + i, 0, source.Width - 1);
                    int neighborY = Clamp(y + j, 0, source.Height - 1);
                    Color pixel = source.GetPixel(neighborX, neighborY);

                    gradientXR += pixel.R * kernelX[j + 1, i + 1];
                    gradientYR += pixel.R * kernelY[j + 1, i + 1];
                    gradientXG += pixel.G * kernelX[j + 1, i + 1];
                    gradientYG += pixel.G * kernelY[j + 1, i + 1];
                    gradientXB += pixel.B * kernelX[j + 1, i + 1];
                    gradientYB += pixel.B * kernelY[j + 1, i + 1];
                }
            }

            int gradientR = (int)Math.Sqrt(gradientXR * gradientXR + gradientYR * gradientYR);
            int gradientG = (int)Math.Sqrt(gradientXG * gradientXG + gradientYG * gradientYG);
            int gradientB = (int)Math.Sqrt(gradientXB * gradientXB + gradientYB * gradientYB);

            return Color.FromArgb(
                Clamp(gradientR, 0, 255),
                Clamp(gradientG, 0, 255),
                Clamp(gradientB, 0, 255)
            );
        }
    }

    class SharrFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            float[,] kernelX = new float[3, 3]
            {
                { -3, 0, 3 },
                { -10, 0, 10 },
                { -3, 0, 3 }
            };

            float[,] kernelY = new float[3, 3]
            {
                { -3, -10, -3 },
                { 0, 0, 0 },
                { 3, 10, 3 }
            };

            float resultRx = 0, resultGx = 0, resultBx = 0;
            float resultRy = 0, resultGy = 0, resultBy = 0;

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    int neighborX = Clamp(x + i, 0, source.Width - 1);
                    int neighborY = Clamp(y + j, 0, source.Height - 1);
                    Color pixel = source.GetPixel(neighborX, neighborY);

                    resultRx += pixel.R * kernelX[j + 1, i + 1];
                    resultGx += pixel.G * kernelX[j + 1, i + 1];
                    resultBx += pixel.B * kernelX[j + 1, i + 1];
                    resultRy += pixel.R * kernelY[j + 1, i + 1];
                    resultGy += pixel.G * kernelY[j + 1, i + 1];
                    resultBy += pixel.B * kernelY[j + 1, i + 1];
                }
            }

            int gradientR = (int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy);
            int gradientG = (int)Math.Sqrt(resultGx * resultGx + resultGy * resultGy);
            int gradientB = (int)Math.Sqrt(resultBx * resultBx + resultBy * resultBy);

            return Color.FromArgb(
                Clamp(gradientR, 0, 255),
                Clamp(gradientG, 0, 255),
                Clamp(gradientB, 0, 255)
            );
        }
    }

    class LinearStretchFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            // Этот метод будет перекрыт полной реализацией processImage
            return source.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap source, BackgroundWorker worker)
        {
            int width = source.Width;
            int height = source.Height;
            int minR = 255, maxR = 0;
            int minG = 255, maxG = 0;
            int minB = 255, maxB = 0;

            // Находим минимальные и максимальные значения яркости
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress((int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    minR = Math.Min(minR, pixel.R);
                    maxR = Math.Max(maxR, pixel.R);
                    minG = Math.Min(minG, pixel.G);
                    maxG = Math.Max(maxG, pixel.G);
                    minB = Math.Min(minB, pixel.B);
                    maxB = Math.Max(maxB, pixel.B);
                }
            }

            // Проверка на случай, если min и max совпадают
            if (maxR == minR) maxR = minR + 1;
            if (maxG == minG) maxG = minG + 1;
            if (maxB == minB) maxB = minB + 1;

            Bitmap result = new Bitmap(width, height);

            // Применяем линейное растяжение
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress(50 + (int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    int newR = (pixel.R - minR) * 255 / (maxR - minR);
                    int newG = (pixel.G - minG) * 255 / (maxG - minG);
                    int newB = (pixel.B - minB) * 255 / (maxB - minB);

                    result.SetPixel(i, j, Color.FromArgb(
                        Clamp(newR, 0, 255),
                        Clamp(newG, 0, 255),
                        Clamp(newB, 0, 255)
                    ));
                }
            }

            return result;
        }
    }

    class GrayWorldFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            // Этот метод будет перекрыт полной реализацией processImage
            return source.GetPixel(x, y);
        }

        public override Bitmap processImage(Bitmap source, BackgroundWorker worker)
        {
            int width = source.Width;
            int height = source.Height;
            long sumR = 0, sumG = 0, sumB = 0;
            int totalPixels = width * height;

            // Подсчитываем средние значения каналов
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress((int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    sumR += pixel.R;
                    sumG += pixel.G;
                    sumB += pixel.B;
                }
            }

            float avgR = sumR / (float)totalPixels;
            float avgG = sumG / (float)totalPixels;
            float avgB = sumB / (float)totalPixels;
            float avgGray = (avgR + avgG + avgB) / 3.0f;

            Bitmap result = new Bitmap(width, height);

            // Применяем коррекцию серого мира
            for (int i = 0; i < width; i++)
            {
                worker.ReportProgress(50 + (int)((float)i / width * 50));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < height; j++)
                {
                    Color pixel = source.GetPixel(i, j);
                    int newR = (int)(pixel.R * (avgGray / avgR));
                    int newG = (int)(pixel.G * (avgGray / avgG));
                    int newB = (int)(pixel.B * (avgGray / avgB));

                    result.SetPixel(i, j, Color.FromArgb(
                        Clamp(newR, 0, 255),
                        Clamp(newG, 0, 255),
                        Clamp(newB, 0, 255)
                    ));
                }
            }

            return result;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            Color sourceColor = source.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                                                255 - sourceColor.G,
                                                255 - sourceColor.B);
            return resultColor;
        }
    }

    class OffsetFilter : Filters
    {
        private int _offsetX;
        private int _offsetY;

        public OffsetFilter(int offsetX = 50, int offsetY = 0)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            if (x - _offsetX < 0 || x - _offsetX >= source.Width ||
                y - _offsetY < 0 || y - _offsetY >= source.Height)
            {
                return Color.White;
            }

            return source.GetPixel(x - _offsetX, y - _offsetY);
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            Color sourceColor = source.GetPixel(x, y);
            int intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Color resultColor = Color.FromArgb(intensity, intensity, intensity);
            return resultColor;
        }
    }

    class BrightnessFilter : Filters
    {
        private int _brightness;

        public BrightnessFilter(int brightness = 20)
        {
            _brightness = brightness;
        }

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            Color sourceColor = source.GetPixel(x, y);

            // Увеличиваем значения RGB на величину яркости
            int R = sourceColor.R + _brightness;
            int G = sourceColor.G + _brightness;
            int B = sourceColor.B + _brightness;

            // Ограничиваем значения в диапазоне [0,255]
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            Color sourceColor = source.GetPixel(x, y);
            int k = 20;
            int intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            int R = intensity + 2 * k;
            int G = intensity + (int)(0.5 * k);
            int B = intensity - 1 * k;
            return Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
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
                    int idX = Clamp(x + k, 0, source.Width - 1);
                    int idY = Clamp(y + l, 0, source.Height - 1);
                    Color neighborColor = source.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }

    class GaussuanFilter : MatrixFilter
    {
        public GaussuanFilter()
        {
            createGaussianKernel(3, 2);
        }

        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }

    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter()
        {
            kernel = new float[9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    kernel[i, j] = 0;

            for (int i = 0; i < 9; i++)
                kernel[i, i] = 1.0f / 9.0f;
        }
    }

    class EmbossFilter : MatrixFilter
    {
        public EmbossFilter()
        {
            // Матрица для эффекта тиснения
            kernel = new float[3, 3]
            {
                {  0, +1,  0 },
                { -1,  0, +1 },
                {  0, -1,  0 }
            };
        }

        public override Bitmap processImage(Bitmap source, BackgroundWorker worker)
        {
            // Сначала применяем матричный фильтр для создания эффекта тиснения
            Bitmap result = new Bitmap(source.Width, source.Height);

            // Применяем матрицу тиснения
            for (int i = 0; i < source.Width; i++)
            {
                worker.ReportProgress((int)((float)i / source.Width * 33));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < source.Height; j++)
                {
                    Color resultColor = calculateNewPixelColor(source, i, j);
                    result.SetPixel(i, j, resultColor);
                }
            }

            // Преобразуем в оттенки серого
            Bitmap grayResult = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Width; i++)
            {
                worker.ReportProgress(33 + (int)((float)i / source.Width * 33));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < source.Height; j++)
                {
                    Color pixel = result.GetPixel(i, j);
                    int intensity = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    grayResult.SetPixel(i, j, Color.FromArgb(intensity, intensity, intensity));
                }
            }

            // Увеличиваем яркость
            Bitmap brightResult = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Width; i++)
            {
                worker.ReportProgress(66 + (int)((float)i / source.Width * 34));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < source.Height; j++)
                {
                    Color pixel = grayResult.GetPixel(i, j);
                    int newR = pixel.R + 100;
                    int newG = pixel.G + 100;
                    int newB = pixel.B + 100;

                    brightResult.SetPixel(i, j, Color.FromArgb(
                        Clamp(newR, 0, 255),
                        Clamp(newG, 0, 255),
                        Clamp(newB, 0, 255)
                    ));
                }
            }

            result.Dispose();
            grayResult.Dispose();
            return brightResult;
        }
    }
}