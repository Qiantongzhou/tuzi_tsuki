using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace OpenAI_hztec.Utilities
{
    public static class QrCodeHelper
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static MemoryStream GerQrCodeStream(string url)
        {
            BitMatrix bitMatrix = new MultiFormatWriter().encode(url, BarcodeFormat.QR_CODE, 300, 300);
            var bw = new ZXing.BarcodeWriterPixelData();

            var pixelData = bw.Write(bitMatrix);
            var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var fileStream = new MemoryStream();
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image   
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            fileStream.Flush();//.net core 必须要加
            fileStream.Position = 0;//.net core 必须要加

            bitmap.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);

            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        /// <summary>
        /// 获取文字图片信息
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MemoryStream GetTextImageStream(string text)
        {
            MemoryStream fileStream = new MemoryStream();
            var fontSize = 14;
            var wordLength = 0;
            for (int i = 0; i < text.Length; i++)
            {
                byte[] bytes = Encoding.Default.GetBytes(text.Substring(i, 1));
                wordLength += bytes.Length > 1 ? 2 : 1;
            }
            using (var bitmap = new System.Drawing.Bitmap(wordLength * fontSize + 20, 14 + 40, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.ResetTransform();//重置图像
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(text, new Font("宋体", fontSize, FontStyle.Bold), Brushes.White, 10, 10);
                    bitmap.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        public static Bitmap GenerateQRCode(string content)
        {
            var qrCodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 500,
                    Width = 500,
                    Margin = 1,
                    ErrorCorrection = ErrorCorrectionLevel.H,
                }
            };

            var pixelData = qrCodeWriter.Write(content);
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            using (var ms = new MemoryStream())
            {
                // Create a bitmap
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    // We assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // Save to stream as PNG
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return new Bitmap(ms);
            }
            }

        public static Bitmap AddIconToQRCode(Bitmap qrCode, Bitmap icon)
        {
            int iconSize = 48;  // Define the icon size
            Bitmap resizedIcon = new Bitmap(icon, new Size(iconSize, iconSize));  // Resize icon to 48x48

            // Create a new bitmap with RGB format based on the original QR code
            Bitmap rgbQrCode = new Bitmap(qrCode.Width, qrCode.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(rgbQrCode))
            {
                // Draw the original QR code onto the new RGB bitmap
                g.DrawImage(qrCode, new Rectangle(0, 0, qrCode.Width, qrCode.Height));

                // Set coordinates for where the icon should be drawn
                int left = (qrCode.Width / 2) - (iconSize / 2);
                int top = (qrCode.Height / 2) - (iconSize / 2);

                // Draw the resized icon onto the QR code
                g.DrawImage(resizedIcon, new Rectangle(left, top, iconSize, iconSize));
            }

            // Dispose original Bitmaps if no longer needed
            qrCode.Dispose();
            icon.Dispose();
            resizedIcon.Dispose();

            return rgbQrCode;
        }

        public static Bitmap AddBackgroundToQRCode(Bitmap qrCode, Bitmap background)
        {
            int iconSize = 500;  // Define the icon size
             background = new Bitmap(background, new Size(iconSize, iconSize));

            // Create a new bitmap for the output to avoid modifying the original qrCode
            Bitmap coloredQR = new Bitmap(qrCode.Width, qrCode.Height);

            // Loop over each pixel in the QR code image
            for (int x = 0; x < qrCode.Width; x++)
            {
                for (int y = 0; y < qrCode.Height; y++)
                {
                    // Get the pixel color from the QR code
                    Color qrColor = qrCode.GetPixel(x, y);
                    //Console.WriteLine(qrColor.ToString());
                    // Check if the pixel is black
                    if (qrColor.Name.Equals("ff000000"))
                    {
                        // It's a black pixel, get the corresponding color from the background
                        Color bgColor = background.GetPixel(x, y);

                        // Set this color in the new image
                        coloredQR.SetPixel(x, y, bgColor);
                    }
                    else
                    {
                       // Console.WriteLine(qrColor.ToString());
                        // If not black, just use the original QR code color (preserves any white or other colors)
                        //coloredQR.SetPixel(x, y, qrColor);
                    }
                }
            }

            return coloredQR;
        }

    }
}