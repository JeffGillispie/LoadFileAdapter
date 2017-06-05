using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LoadFileAdapter.Exporters
{
    /// <summary>
    /// Creates a slipsheet for a <see cref="Document"/>.
    /// </summary>
    public class SlipSheet
    {
        private string key;
        private string text;        
        private int resolution = 300;
        private Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        private HorizontalPlacementOption horizontalTextPlacement = HorizontalPlacementOption.Left;
        private VerticalPlacementOption verticalTextPlacement = VerticalPlacementOption.Top;
                
        private SlipSheet()
        {
            // do nothing here
        }

        /// <summary>
        /// The docid value of the slipsheet.
        /// </summary>
        public string Key { get { return key; } }

        /// <summary>
        /// The horizontal placement option for slipsheet text.
        /// </summary>
        public enum HorizontalPlacementOption
        {
            Center, Left
        }

        /// <summary>
        /// The vertical placement option for slipsheet text.
        /// </summary>
        public enum VerticalPlacementOption
        {
            Center, Top
        }

        /// <summary>
        /// Gets the slip sheet bitmap.
        /// </summary>        
        /// <returns>Returns a bitmap image of a slip sheet.</returns>
        public Bitmap GetImage()
        {
            int sizeFactor = resolution / 100;
            int width = 850 * sizeFactor;
            int height = 1100 * sizeFactor;
            Bitmap image = new Bitmap(width, height);
            Graphics slipsheet = Graphics.FromImage(image);
            slipsheet.FillRectangle(Brushes.White, 0, 0, image.Width, image.Height);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = getStringAlignment(horizontalTextPlacement);
            drawFormat.LineAlignment = getStringAlignment(verticalTextPlacement);
            slipsheet.DrawString(text, font, SystemBrushes.WindowText, 
                new Rectangle(10, 10, width, height), drawFormat);

            return image;
        }

        /// <summary>
        /// Saves the slip sheet as the image/tiff mime type to the target destination.
        /// </summary>        
        /// <param name="destination">The location where the slip sheet should be saved.</param>
        public void SaveImage(string destination)
        {
            Bitmap bmp = GetImage();
            bmp.SetResolution(resolution, resolution);
            Bitmap bitonalBmp = convertToBitonal(bmp);
            ImageCodecInfo codec = getEncoderInfo("image/tiff");
            Encoder encoder = Encoder.Compression;
            EncoderParameters encoderParams = new EncoderParameters(1);
            EncoderParameter encoderParam = new EncoderParameter(encoder, (long)EncoderValue.CompressionCCITT4);
            encoderParams.Param[0] = encoderParam;
            bitonalBmp.Save(destination, codec, encoderParams);
        }

        /// <summary>
        /// Gets the string alignment for positioning text on the slipsheet.
        /// </summary>
        /// <param name="alignment">The target horizontal or vertical XREF placement option.</param>
        /// <returns>Returns center if either the horizontal or vertical placement option is set to 
        /// center. Otherwise returns near.</returns>
        protected StringAlignment getStringAlignment(object alignment)
        {
            StringAlignment stringAlignment = StringAlignment.Near;

            if (alignment != null)
            {
                if ((alignment.GetType().Equals(typeof(HorizontalPlacementOption)) &&
                    (HorizontalPlacementOption)(object)alignment == HorizontalPlacementOption.Center)
                    || 
                    (alignment.GetType().Equals(typeof(VerticalPlacementOption)) &&
                    (VerticalPlacementOption)(object)alignment == VerticalPlacementOption.Center))
                {
                    stringAlignment = StringAlignment.Center;
                }                
            }

            return stringAlignment;
        }

        /// <summary>
        /// Converts a bitmap to a 32 BPP ARGB bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to convert.</param>
        /// <returns>Returns a 32 BPP ARGB bitmap.</returns>
        protected Bitmap convertTo32bppARGB(Bitmap bitmap)
        {
            Bitmap updatedBitmap = bitmap;
                        
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                updatedBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
                updatedBitmap.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                using (Graphics g = Graphics.FromImage(updatedBitmap))
                {
                    g.DrawImageUnscaled(bitmap, 0, 0);
                }
            }
            
            return updatedBitmap;
        }

        /// <summary>
        /// Gets a buffer with the bitonal data created from the supplied bitmap.
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to convert to bitonal.</param>
        /// <param name="sourceBuffer">A buffer containing the data for the original image.</param>
        /// <param name="sourceData">The bitmap data of the original image.</param>
        /// <param name="destinationData">The bitmap data of the bitonal image.</param>
        /// <returns>Returns a buffer of bitonal data to create a bitonal image.</returns>
        protected byte[] getBitonalBuffer(
            Bitmap sourceBitmap, byte[] sourceBuffer, BitmapData sourceData, BitmapData destinationData)
        {
            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;            
            int threshold = 500;
            int imageSize = destinationData.Stride * destinationData.Height;
            byte[] buffer = new byte[imageSize];
            
            // Iterate lines
            for (int y = 0; y < sourceBitmap.Height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;
                // Iterate pixels
                for (int x = 0; x < sourceBitmap.Width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    //                           B                             G                              R
                    pixelTotal = sourceBuffer[sourceIndex] + sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }

                    if (pixelValue == 1)
                    {
                        buffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }

                if (pixelValue != 128)
                {
                    buffer[destinationIndex] = destinationValue;
                }
            }

            return buffer;
        }

        /// <summary>
        /// Converts the supplied bitmap to a bitonal image.
        /// </summary>
        /// <param name="originalBitmap">The bitmap to convert.</param>
        /// <returns>Returns a bitonal bitmap.</returns>
        protected Bitmap convertToBitonal(Bitmap originalBitmap)
        {
            Bitmap sourceBitmap = convertTo32bppARGB(originalBitmap); 
            // Lock source bitmap in memory
            BitmapData sourceData = sourceBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
                ImageLockMode.ReadOnly, 
                PixelFormat.Format32bppArgb);
            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);
            // Unlock source bitmap
            sourceBitmap.UnlockBits(sourceData);
            // Create destination bitmap
            Bitmap destinationBitmap = new Bitmap(
                sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format1bppIndexed);
            destinationBitmap.SetResolution(
                originalBitmap.HorizontalResolution, originalBitmap.VerticalResolution);
            // Lock destination bitmap in memory
            BitmapData destinationData = destinationBitmap.LockBits(
                new Rectangle(0, 0, destinationBitmap.Width, destinationBitmap.Height), 
                ImageLockMode.WriteOnly, 
                PixelFormat.Format1bppIndexed);
            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = getBitonalBuffer(
                sourceBitmap, sourceBuffer, sourceData, destinationData);
            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);
            // Unlock destination bitmap
            destinationBitmap.UnlockBits(destinationData); 
            // Dispose of source if not originally supplied bitmap
            if (sourceBitmap != originalBitmap)
            {
                sourceBitmap.Dispose();
            }
                       
            return destinationBitmap;
        }

        /// <summary>
        /// Gets the specified image codec.
        /// </summary>
        /// <param name="mimeType">The mime type of the codec to obtain.</param>
        /// <returns>Returns the requested image codec or null if not found.</returns>
        protected static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            ImageCodecInfo codec = null;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < encoders.Length; ++i)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    codec = encoders[i];
                    break;
                }
            }

            return codec;
        }

        public class Builder
        {
            private SlipSheet instance;

            private Builder()
            {
                instance = new SlipSheet();
            }

            public static Builder Start(string key, string text)
            {
                Builder builder = new Builder();
                builder.instance.key = key;
                builder.instance.text = text;
                return builder;
            }

            public Builder SetResolution(int value)
            {
                instance.resolution = value;
                return this;
            }

            public Builder SetFont(Font value)
            {
                instance.font = value;
                return this;
            }

            public Builder SetHorizontalTextPlacement(HorizontalPlacementOption value)
            {
                instance.horizontalTextPlacement = value;
                return this;
            }

            public Builder SetVerticalTextPlacement(VerticalPlacementOption value)
            {
                instance.verticalTextPlacement = value;
                return this;
            }

            public SlipSheet Build()
            {
                SlipSheet instance = this.instance;
                this.instance = null;
                return instance;
            }
        }
    }
}
