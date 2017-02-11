using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Drawing.Imaging;
using Patagames.Ocr;
using Patagames.Ocr.Enums;
using Accord.Imaging.Converters;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Distributions.DensityKernels;
using AForge.Imaging;
using AForge.Math.Geometry;
using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Features2D;

namespace Text_From_Video_V1
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Algo.Items.Add("Connected Components Filtering");
            Algo.Items.Add("Manual Rectangle Filtering");
            Algo.Items.Add("Cluster Analysis-Kmeans");
            Algo.Items.Add("Moving Window");
            clspace.Items.Add("RGB");
        }

        private void fileselect_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.ShowDialog();
            string sFileName = openFileDialog.FileName;
            filepath.Text = sFileName;
            VideoFileReader reader = new VideoFileReader();
            reader.Open(sFileName);
            while (true)
                using (var videoFrame = reader.ReadVideoFrame())
                {
                    i++;
                    if (videoFrame == null)
                        break;
                }
            
            reader.Close();
            Framelast.Text = (i - 2).ToString();
            FrameStart.Text = "0";
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int i = Convert.ToInt32(Slivid.Value);
            Frmacurr.Text = i.ToString();
            VideoFileReader reader = new VideoFileReader();
            reader.Open(filepath.Text);
            for (int j = 0; j < i; j++)
                reader.ReadVideoFrame();
            Bitmap videoFrame = reader.ReadVideoFrame();
            MemoryStream ms = new MemoryStream();
            videoFrame.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            Imgshow.Source = bi;
            videoFrame.Save(@"img.bmp");
            reader.Close();
            videoFrame.Dispose();
        }

        private void Gettext_Click(object sender, RoutedEventArgs e)
        {
            if (Algo.SelectedIndex==0)
            {
                string path = @"img.bmp";
                Bitmap image = (Bitmap)Bitmap.FromFile(path);
                image = modifyframecc(image);
                //display transformed image
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Imgshowt.Source = bi;
                Fetchedtext.Text = textframecc();
            }
            if (Algo.SelectedIndex == 1)
            {
                string path = @"img.bmp";
                Bitmap image = (Bitmap)Bitmap.FromFile(path);
                Bitmap cropped = CropImage(image, Convert.ToInt32(xsli.Value), Convert.ToInt32(ysli.Value), Convert.ToInt32(wsli.Value), Convert.ToInt32(hsli.Value));
                if (bcg.IsChecked == true)
                {
                    cropped = bcgadj(cropped);
                }
                if (sharp.IsChecked == true)
                {
                    cropped = Sharpen(cropped);
                }
                //display transformed image
                MemoryStream ms = new MemoryStream();
                cropped.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Imgshowt.Source = bi;
                //display transformed image
                OcrApi.LicenseKey = "52433553494d50032923be84e16cd6ae0bce153446af7918d52303038286fd2bd9870b5498b68a351dbc49304f2b0b58fcb30eafb46874984a592a74682d1aa6b2ee661e27d761db6dcedb10a8547f94a52f34cebc926adfb0f0ad0868fe9cf48e752a40bbdc04e988c11580393ed65a0bf2903c6434e266";
                using (var api = OcrApi.Create())
                {
                    api.Init(Languages.English);
                    using (cropped)
                    {
                        Fetchedtext.Text = api.GetTextFromImage(cropped);
                    }
                }

               
            }
            if (Algo.SelectedIndex == 2)
            {
                string path = @"img.bmp";
                Bitmap image = (Bitmap)Bitmap.FromFile(path);
                Bitmap cluster = runkmeans(image);
                //display transformed image
                MemoryStream ms = new MemoryStream();
                cluster.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Imgshowt.Source = bi;
                //display transformed image
                OcrApi.LicenseKey = "52433553494d50032923be84e16cd6ae0bce153446af7918d52303038286fd2bd9870b5498b68a351dbc49304f2b0b58fcb30eafb46874984a592a74682d1aa6b2ee661e27d761db6dcedb10a8547f94a52f34cebc926adfb0f0ad0868fe9cf48e752a40bbdc04e988c11580393ed65a0bf2903c6434e266";
                using (var api = OcrApi.Create())
                {
                    api.Init(Languages.Dutch);
                    using (cluster)
                    {
                        Fetchedtext.Text = api.GetTextFromImage(cluster);
                    }
                }

            }
            if (Algo.SelectedIndex == 3)
            {
                string path = @"img.bmp";
                Bitmap image = (Bitmap)Bitmap.FromFile(path);
                string imgtext = movwin(image);
                //display transformed image
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                Imgshowt.Source = bi;
                //display transformed image
                Fetchedtext.Text = imgtext;
            }
            
        }
        public Bitmap CropImage(Bitmap source, int x, int y, int width, int height)
        {
            System.Drawing.Rectangle crop = new System.Drawing.Rectangle(x, y, width, height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        public string movwin(Bitmap Source)
        {
            // Retrieve the number of clusters
            int h = Source.Height;
            int w = Source.Width;
            //adjust bcg
            if (bcg.IsChecked == true)
            {
                Source = bcgadj(Source);
            }
            if (sharp.IsChecked == true)
            {
                Source = Sharpen(Source);
            }
            for (int i = 0; i <= h; i++)
            {
                for (int j=0;j)
            }

            string text = "Text: ";
            //for (int i=)
            

            return text;
        }
        public Bitmap runkmeans(Bitmap Source)
        {
            // Retrieve the number of clusters
            int k = (int)cluslidk.Value;

            // Load original image
            Bitmap image = Source;

            // Create converters
            ImageToArray imageToArray = new ImageToArray(min: -1, max: +1);
            ArrayToImage arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            // Transform the image into an array of pixel values
            double[][] pixels; imageToArray.Convert(image, out pixels);


            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            KMeans kmeans = new KMeans(k, Distance.SquareEuclidean)
            {
                Tolerance = 0.05
            };

            // Compute the K-Means algorithm until the difference in
            //  cluster centroids between two iterations is below 0.05
            int[] idx = kmeans.Compute(pixels);


            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);

            // Show resulting image in the picture box
            Bitmap result; arrayToImage.Convert(pixels, out result);

            return result;
        }
        
        public string textframecc()
        {
            string appath = AppDomain.CurrentDomain.BaseDirectory;
            string blobtext, frametext="text=";
            DirectoryInfo dir = new DirectoryInfo(appath);
            FileInfo[] files = dir.GetFiles("result*", SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
                OcrApi.LicenseKey = "52433553494d50032923be84e16cd6ae0bce153446af7918d52303038286fd2bd9870b5498b68a351dbc49304f2b0b58fcb30eafb46874984a592a74682d1aa6b2ee661e27d761db6dcedb10a8547f94a52f34cebc926adfb0f0ad0868fe9cf48e752a40bbdc04e988c11580393ed65a0bf2903c6434e266";
                using (var api = OcrApi.Create())
                {
                    api.Init(Languages.English);
                    using (var bmp = Bitmap.FromFile(item.FullName.ToString()) as Bitmap)
                    {
                        blobtext = api.GetTextFromImage(bmp);
                    }
                }
                frametext = frametext + " " + blobtext;
            }
            return frametext;
        }
        public Bitmap modifyframecc(Bitmap source)
        {
            string appath = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(appath);
            int i = 0;
            Bitmap image = source;
            //ApplyAnimationClock grayscale filtering
            Grayscale filter1 = new Grayscale(grey1.Value / 1000, grey2.Value / 1000, grey3.Value / 1000);
            // apply the filter
            image = filter1.Apply(image);
            // edge detection filtering
            DifferenceEdgeDetector filter2 = new DifferenceEdgeDetector();
            // apply the filter
            image = filter2.Apply(image);
            // create filter
            Dilatation filter3 = new Dilatation();
            // apply the filter
            image = filter3.Apply(image);
            // create filter
            Threshold filter4 = new Threshold(Convert.ToInt32(thresh1.Value));
            // apply the filter
            image = filter4.Apply(image);
            //adjust bcg
            if (bcg.IsChecked==true)
            {
                image=bcgadj(image);
            }
            if (sharp.IsChecked==true)
            {
                image=Sharpen(image);
            }
            // create filter
            ConnectedComponentsLabeling filter = new ConnectedComponentsLabeling();
            // apply the filter
            image = filter.Apply(image);
            // locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 1;
            blobCounter.MinWidth = 1;
            blobCounter.ProcessImage(image);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            //remove all files from base directory
            FileInfo[] files = dir.GetFiles("result*", SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
                System.IO.File.Delete(item.FullName);
            }
            foreach (var blob in blobs)
            {
                List<AForge.IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                try
                {
                    Bitmap tempBitmap = CropImage(source, blob.Rectangle.X, blob.Rectangle.Y, blob.Rectangle.Width, blob.Rectangle.Height);
                    i = i + 1;
                    tempBitmap.Save("result" + i.ToString() + ".bmp");
                }
                catch (ArgumentException err1)
                {
                    continue;
                }
            }
            return image;
        }

        private void Showcontent_Click(object sender, RoutedEventArgs e)
        {
            if (Algo.SelectedIndex==0)
            {
                crl.Visibility = Visibility.Visible;
                cbl.Visibility = Visibility.Visible;
                cgl.Visibility = Visibility.Visible;
                thresh1.Visibility = Visibility.Visible;
                grey1.Visibility = Visibility.Visible;
                grey2.Visibility = Visibility.Visible;
                grey3.Visibility = Visibility.Visible;
                tl.Visibility = Visibility.Visible;
                Gettext.Visibility = Visibility.Visible;
                bril.Visibility = Visibility.Visible;
                conl.Visibility = Visibility.Visible;
                Gammal.Visibility = Visibility.Visible;
                brisli.Visibility = Visibility.Visible;
                consli.Visibility = Visibility.Visible;
                gamsli.Visibility = Visibility.Visible;
                clspace.Visibility = Visibility.Hidden;
                cluslidk.Visibility = Visibility.Hidden;
                Clusnumkmeans.Visibility = Visibility.Hidden;
                colorl.Visibility = Visibility.Visible;
                Xl.Visibility = Visibility.Hidden;
                Yl.Visibility = Visibility.Hidden;
                heightl.Visibility = Visibility.Hidden;
                Wl.Visibility = Visibility.Hidden;
                xsli.Visibility = Visibility.Hidden;
                ysli.Visibility = Visibility.Hidden;
                hsli.Visibility = Visibility.Hidden;
                wsli.Visibility = Visibility.Hidden;
                Selview.Visibility = Visibility.Hidden;
                heightt.Visibility = Visibility.Hidden;
                widtht.Visibility = Visibility.Hidden;
                heightl_Copy.Visibility = Visibility.Hidden;
                Wl_Copy.Visibility = Visibility.Hidden;

            }
            if (Algo.SelectedIndex == 1)
            {
                Xl.Visibility = Visibility.Visible;
                Yl.Visibility = Visibility.Visible;
                heightl.Visibility = Visibility.Visible;
                Wl.Visibility = Visibility.Visible;
                xsli.Visibility = Visibility.Visible;
                ysli.Visibility = Visibility.Visible;
                hsli.Visibility = Visibility.Visible;
                wsli.Visibility = Visibility.Visible;
                Gettext.Visibility = Visibility.Visible;
                Selview.Visibility = Visibility.Visible;
                heightt.Visibility = Visibility.Visible;
                widtht.Visibility = Visibility.Visible;
                heightl_Copy.Visibility = Visibility.Visible;
                Wl_Copy.Visibility = Visibility.Visible;
                bril.Visibility = Visibility.Visible;
                conl.Visibility = Visibility.Visible;
                Gammal.Visibility = Visibility.Visible;
                brisli.Visibility = Visibility.Visible;
                consli.Visibility = Visibility.Visible;
                gamsli.Visibility = Visibility.Visible;
                clspace.Visibility = Visibility.Hidden;
                cluslidk.Visibility = Visibility.Hidden;
                Clusnumkmeans.Visibility = Visibility.Hidden;
                colorl.Visibility = Visibility.Visible;
                crl.Visibility = Visibility.Hidden;
                cbl.Visibility = Visibility.Hidden;
                cgl.Visibility = Visibility.Hidden;
                thresh1.Visibility = Visibility.Hidden;
                grey1.Visibility = Visibility.Hidden;
                grey2.Visibility = Visibility.Hidden;
                grey3.Visibility = Visibility.Hidden;
                tl.Visibility = Visibility.Hidden;
                
            }
            if (Algo.SelectedIndex == 2)
            {
                Xl.Visibility = Visibility.Hidden;
                Yl.Visibility = Visibility.Hidden;
                heightl.Visibility = Visibility.Hidden;
                Wl.Visibility = Visibility.Hidden;
                xsli.Visibility = Visibility.Hidden;
                ysli.Visibility = Visibility.Hidden;
                hsli.Visibility = Visibility.Hidden;
                wsli.Visibility = Visibility.Hidden;
                Gettext.Visibility = Visibility.Visible;
                Selview.Visibility = Visibility.Hidden;
                heightt.Visibility = Visibility.Hidden;
                widtht.Visibility = Visibility.Hidden;
                heightl_Copy.Visibility = Visibility.Hidden;
                Wl_Copy.Visibility = Visibility.Hidden;
                bril.Visibility = Visibility.Visible;
                conl.Visibility = Visibility.Visible;
                Gammal.Visibility = Visibility.Visible;
                brisli.Visibility = Visibility.Visible;
                consli.Visibility = Visibility.Visible;
                gamsli.Visibility = Visibility.Visible;
                clspace.Visibility = Visibility.Visible;
                cluslidk.Visibility = Visibility.Visible;
                Clusnumkmeans.Visibility = Visibility.Visible;
                colorl.Visibility = Visibility.Visible;
                crl.Visibility = Visibility.Hidden;
                cbl.Visibility = Visibility.Hidden;
                cgl.Visibility = Visibility.Hidden;
                thresh1.Visibility = Visibility.Hidden;
                grey1.Visibility = Visibility.Hidden;
                grey2.Visibility = Visibility.Hidden;
                grey3.Visibility = Visibility.Hidden;
                tl.Visibility = Visibility.Hidden;

            }
            if (Algo.SelectedIndex == 3)
            {
                Xl.Visibility = Visibility.Hidden;
                Yl.Visibility = Visibility.Hidden;
                heightl.Visibility = Visibility.Hidden;
                Wl.Visibility = Visibility.Hidden;
                xsli.Visibility = Visibility.Hidden;
                ysli.Visibility = Visibility.Hidden;
                hsli.Visibility = Visibility.Hidden;
                wsli.Visibility = Visibility.Hidden;
                Gettext.Visibility = Visibility.Visible;
                Selview.Visibility = Visibility.Hidden;
                heightt.Visibility = Visibility.Hidden;
                widtht.Visibility = Visibility.Hidden;
                heightl_Copy.Visibility = Visibility.Hidden;
                Wl_Copy.Visibility = Visibility.Hidden;
                bril.Visibility = Visibility.Hidden;
                conl.Visibility = Visibility.Hidden;
                Gammal.Visibility = Visibility.Hidden;
                brisli.Visibility = Visibility.Hidden;
                consli.Visibility = Visibility.Hidden;
                gamsli.Visibility = Visibility.Hidden;
                clspace.Visibility = Visibility.Hidden;
                cluslidk.Visibility = Visibility.Hidden;
                Clusnumkmeans.Visibility = Visibility.Hidden;
                colorl.Visibility = Visibility.Hidden;
                crl.Visibility = Visibility.Hidden;
                cbl.Visibility = Visibility.Hidden;
                cgl.Visibility = Visibility.Hidden;
                thresh1.Visibility = Visibility.Hidden;
                grey1.Visibility = Visibility.Hidden;
                grey2.Visibility = Visibility.Hidden;
                grey3.Visibility = Visibility.Hidden;
                tl.Visibility = Visibility.Hidden;

            }
        }

        private void Selview_Click(object sender, RoutedEventArgs e)
        {
            string path = @"img.bmp";
            Bitmap image = (Bitmap)Bitmap.FromFile(path);
            heightt.Text = image.Height.ToString();
            widtht.Text = image.Width.ToString();
        }
        private void Adjbcg_Click(object sender, RoutedEventArgs e)
        {
            string path = @"img.bmp";
            Bitmap Source = (Bitmap)Bitmap.FromFile(path);
            Bitmap adjustedImage = bcgadj(Source);
            //display transformed image
            MemoryStream ms = new MemoryStream();
            adjustedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            BCGimage.Source = bi;
        }

        public Bitmap bcgadj(Bitmap Source)
        {
            Bitmap adjustedImage = Source;
            float brightness = Convert.ToSingle(brisli.Value); // no change in brightness
            float contrast = Convert.ToSingle(consli.Value); // twice the contrast
            float gamma = Convert.ToSingle(gamsli.Value); // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
                new float[] {contrast, 0, 0, 0, 0}, // scale red
                new float[] {0, contrast, 0, 0, 0}, // scale green
                new float[] {0, 0, contrast, 0, 0}, // scale blue
                new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(Source, new System.Drawing.Rectangle(0, 0, Source.Width, Source.Height)
                , 0, 0, Source.Width, Source.Height,
                GraphicsUnit.Pixel, imageAttributes);

            //dispose the Graphics object
            g.Dispose();
            return adjustedImage;


        }
        //public Bitmap mserocr(Bitmap Source)
        //{
        //    Image<Bgr, Byte> image = new Image<Bgr, Byte>(Source);
        //    MSERDetector mserdetect = new MSERDetector();
        //    var keypts = mserdetect.Detect(image);
        //    Image<Bgr, byte> Result = null;
        //    Result = image.Clone();

        //    foreach (MKeyPoint keypt in keypts)
        //    {
        //        PointF pt = keypt.Point;
        //        Result.Draw(new CircleF(pt, 5), new Bgr(255, 255, 0), 5);
        //    }
        //    Bitmap result = Result.ToBitmap();
        //    return result;
        //}
        public static Bitmap Sharpen(Bitmap image)
        {
            Bitmap sharpenImage = (Bitmap)image.Clone();

            int filterWidth = 3;
            int filterHeight = 3;
            int width = image.Width;
            int height = image.Height;

            // Create sharpening filter.
            double[,] filter = new double[filterWidth, filterHeight];
            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0;
            double bias = 0.0;

            System.Drawing.Color[,] result = new System.Drawing.Color[image.Width, image.Height];

            // Lock image bits for read/write.
            BitmapData pbits = sharpenImage.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Declare an array to hold the bytes of the bitmap.
            int bytes = pbits.Stride * height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

            int rgb;
            // Fill the color array with the new sharpened color values.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + width) % width;
                            int imageY = (y - filterHeight / 2 + filterY + height) % height;

                            rgb = imageY * pbits.Stride + 3 * imageX;

                            red += rgbValues[rgb + 2] * filter[filterX, filterY];
                            green += rgbValues[rgb + 1] * filter[filterX, filterY];
                            blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = System.Drawing.Color.FromArgb(r, g, b);
                    }
                }
            }

            // Update the image with the sharpened pixels.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rgb = y * pbits.Stride + 3 * x;

                    rgbValues[rgb + 2] = result[x, y].R;
                    rgbValues[rgb + 1] = result[x, y].G;
                    rgbValues[rgb + 0] = result[x, y].B;
                }
            }

            // Copy the RGB values back to the bitmap.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
            // Release image bits.
            sharpenImage.UnlockBits(pbits);

            return sharpenImage;
        }

        private void Procvid_Click(object sender, RoutedEventArgs e)
        {
            string sFileName = filepath.Text;
            string vidtext="Text";
            VideoFileReader reader = new VideoFileReader();
            int framesskip = Convert.ToInt32(framescroll.Text);
            int framecount=0;
            reader.Open(sFileName);
            while (true)
            {
                using (var videoFrame = reader.ReadVideoFrame())
                {
                    framecount++;
                    if (videoFrame == null)
                        break;
                }
            }
            Bitmap Source, crop;
            for (int j=1;j<(framecount-1);j++)
            {
                reader.ReadVideoFrame();
                if (j%framesskip==0)
                {
                    Source = reader.ReadVideoFrame();
                    crop = CropImage(Source, Convert.ToInt32(xsli.Value), Convert.ToInt32(ysli.Value), Convert.ToInt32(wsli.Value), Convert.ToInt32(hsli.Value));
                    if (Algo.SelectedIndex == 1)
                    {
                        if (bcg.IsChecked == true)
                        {
                            crop = bcgadj(crop);
                        }
                        if (sharp.IsChecked == true)
                        {
                            crop = Sharpen(crop);
                        }
                        //display transformed image
                        OcrApi.LicenseKey = "52433553494d50032923be84e16cd6ae0bce153446af7918d52303038286fd2bd9870b5498b68a351dbc49304f2b0b58fcb30eafb46874984a592a74682d1aa6b2ee661e27d761db6dcedb10a8547f94a52f34cebc926adfb0f0ad0868fe9cf48e752a40bbdc04e988c11580393ed65a0bf2903c6434e266";
                        using (var api = OcrApi.Create())
                        {
                            api.Init(Languages.English);
                            using (crop)
                            {
                                vidtext = vidtext + " " + api.GetTextFromImage(crop);
                            }
                        }

                    }

                }
            }
            Fetchedtext.Text = vidtext;
        }
    }
}