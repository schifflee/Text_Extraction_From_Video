using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_From_Video_V1
{
    class Test
    {
        //if (Algo.SelectedIndex == 2)
        //{
        //    string path = @"img.bmp";
        //    int shape=shapesel.SelectedIndex;
        //    Bitmap image = (Bitmap)Bitmap.FromFile(path);
        //    image = Shapeidemcv(image,shape);
        //    //display transformed image
        //    MemoryStream ms = new MemoryStream();
        //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //    ms.Position = 0;
        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.StreamSource = ms;
        //    bi.EndInit();
        //    Imgshowt.Source = bi;
        //}
        //if (Algo.SelectedIndex == 3)
        //{
        //    string path = @"img.bmp";
        //    int shape = shapesel.SelectedIndex;
        //    Bitmap image = (Bitmap)Bitmap.FromFile(path);
        //    image = mserocr(image);
        //    //display transformed image
        //    MemoryStream ms = new MemoryStream();
        //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //    ms.Position = 0;
        //    BitmapImage bi = new BitmapImage();
        //    bi.BeginInit();
        //    bi.StreamSource = ms;
        //    bi.EndInit();
        //    Imgshowt.Source = bi;
        //}
        //public Bitmap Shapeidemcv(Bitmap Source,int shape)
        //{
        //    Bitmap image = Source;
        //    if (bcg.IsChecked == true)
        //    {
        //        image = bcgadj(image);
        //    }
        //    if (sharp.IsChecked == true)
        //    {
        //        image = Sharpen(image);
        //    }

        //    //Load the image from file and resize it for display
        //    Image<Bgr, Byte> img = new Image<Bgr, byte>(image).Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

        //    //Convert the image to grayscale and filter out the noise
        //    UMat uimage = new UMat();
        //    CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

        //    //use image pyr to remove noise
        //    UMat pyrDown = new UMat();
        //    CvInvoke.PyrDown(uimage, pyrDown);
        //    CvInvoke.PyrUp(pyrDown, uimage);

        //    //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

        //    double cannyThreshold = 180.0;
        //    double circleAccumulatorThreshold = 120;
        //    CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

        //    double cannyThresholdLinking = 120.0;
        //    UMat cannyEdges = new UMat();
        //    CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

        //    LineSegment2D[] lines = CvInvoke.HoughLinesP(
        //       cannyEdges,
        //       1, //Distance resolution in pixel-related units
        //       Math.PI / 45.0, //Angle resolution measured in radians.
        //       20, //threshold
        //       30, //min Line width
        //       10); //gap between lines


        //    List<Triangle2DF> triangleList = new List<Triangle2DF>();
        //    List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

        //    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
        //    {
        //        CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
        //        int count = contours.Size;
        //        for (int i = 0; i < count; i++)
        //        {
        //            using (VectorOfPoint contour = contours[i])
        //            using (VectorOfPoint approxContour = new VectorOfPoint())
        //            {
        //                CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
        //                if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
        //                {
        //                    if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
        //                    {
        //                        System. Drawing.Point[] pts = approxContour.ToArray();
        //                        triangleList.Add(new Triangle2DF(
        //                           pts[0],
        //                           pts[1],
        //                           pts[2]
        //                           ));
        //                    }
        //                    else if (approxContour.Size == 4) //The contour has 4 vertices.
        //                    {
        //                        bool isRectangle = true;
        //                        System.Drawing.Point[] pts = approxContour.ToArray();
        //                        LineSegment2D[] edges = Emgu.CV.PointCollection.PolyLine(pts, true);

        //                        for (int j = 0; j < edges.Length; j++)
        //                        {
        //                            double angle = Math.Abs(
        //                               edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
        //                            if (angle < 80 || angle > 100)
        //                            {
        //                                isRectangle = false;
        //                                break;
        //                            }
        //                        }

        //                        if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
        //                    }
        //                }
        //            }
        //        }
        //    }



        //    Image<Bgr, Byte> triangleRectangleImage = img.CopyBlank();
        //    foreach (Triangle2DF triangle in triangleList)
        //        triangleRectangleImage.Draw(triangle, new Bgr(System.Drawing.Color.DarkBlue), 2);
        //    foreach (RotatedRect box in boxList)
        //        triangleRectangleImage.Draw(box, new Bgr(System.Drawing.Color.DarkOrange), 2);


        //    Image<Bgr, Byte> circleImage = img.CopyBlank();
        //    foreach (CircleF circle in circles)
        //        circleImage.Draw(circle, new Bgr(System.Drawing.Color.Brown), 2);

        //    Image<Bgr, Byte> lineImage = img.CopyBlank();
        //    foreach (LineSegment2D line in lines)
        //        lineImage.Draw(line, new Bgr(System.Drawing.Color.Green), 2);
        //    //return values
        //    Bitmap BmpInput = triangleRectangleImage.ToBitmap();
        //    if (shape==0)
        //    {
        //        BmpInput = triangleRectangleImage.ToBitmap();
        //    }
        //    if (shape == 1)
        //    {
        //        BmpInput = circleImage.ToBitmap();
        //    }
        //    if (shape == 2)
        //    {
        //        BmpInput = lineImage.ToBitmap();
        //    }
        //    return BmpInput;

        //}
    }
}
