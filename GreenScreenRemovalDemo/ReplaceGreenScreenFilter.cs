using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GreenScreenRemovalDemo
{
    class ReplaceGreenScreenFilter : IDisposable
    {
        private byte _greenScale = 30;
        public byte GreenScale
        {
            get
            {
                return this._greenScale;
            }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("GreenScale should >=0 and <=255");
                }
                this._greenScale = value;
            }
        }

        private double _minBlockPercent = 0.01;
        public double MinBlockPercent
        {
            get
            {
                return this._minBlockPercent;
            }
            set
            {
                if (value <= 0 || value >= 1)
                {
                    throw new ArgumentOutOfRangeException("GreenScale should >0 and <1");
                }
                this._minBlockPercent = value;
            }
        }

        private Mat _backgroundImage;

        public void SetBackgroundImage(Mat backgroundImage)
        {
            this._backgroundImage = backgroundImage.Clone();
        }

        public ReplaceGreenScreenFilter(Mat backgroundImage)
        {
            SetBackgroundImage(backgroundImage);
        }

        /// <summary>
        /// Render matMask by src, every green pixel on src will cause a black pixel rendered on the same position on matMask,
        /// otherwise, a white pixel will be rendered on matMask
        /// </summary>
        /// <param name="src"></param>
        /// <param name="matMask"></param>
        private unsafe void RenderGreenScreenMask(Mat src, Mat matMask)
        {
            if (src.Channels() < 3)
            {
                throw new ArgumentException("src.Channels() should >=3");
            }
            if (matMask.Channels() != 1)
            {
                throw new ArgumentException("matMask.Channels() should == 1");
            }
            if (src.Size() != matMask.Size())
            {
                throw new System.ArgumentException("src.Size()!=matMask.Size()");
            }
            //https://codereview.stackexchange.com/questions/184044/processing-an-image-to-extract-green-screen-mask
            int rows = src.Rows;
            int cols = src.Cols;
            for (int x = 0; x < rows; x++)
            {
                Vec3b* srcRow = (Vec3b*)src.Ptr(x);
                byte* maskRow = (byte*)matMask.Ptr(x);
                for (int y = 0; y < cols; y++)
                {
                    var pData = srcRow + y;
                    byte blue = pData->Item0;
                    byte green = pData->Item1;
                    byte red = pData->Item2;
                    byte max1 = blue > green ? blue : green;
                    byte max = max1>red?max1:red;
                    //if this pixel is some green, render the pixel with the same position on matMask as black
                    if (green == max && green > this._greenScale)
                    {
                        *(maskRow + y) = 0;
                    }
                    else
                    {
                        *(maskRow + y) = 255;//render as white
                    }
                }
            }
        }

        private Dictionary<double, Mat> dictGammaCorrectionLUT = new Dictionary<double, Mat>();


        //https://www.cnblogs.com/sdu20112013/p/11597171.html
        private void GammaCorrect(Mat src,double gamma)
        {
            Mat lookUpTable = dictGammaCorrectionLUT.GetValueOrDefault(gamma);
            if(lookUpTable==null)
            {
                lookUpTable = new Mat(new Size(1, 256), MatType.CV_8UC1, new Scalar(0));
                for (int i = 0; i <= 255; i++)
                {
                    double value = Math.Pow(i / 255.0, gamma) * 255.0;
                    lookUpTable.Set<byte>(0, i, np.clip((byte)value, 0, 255));
                }
                dictGammaCorrectionLUT[gamma] = lookUpTable;
            }
            Cv2.LUT(src, lookUpTable, src);
        }


        public void Apply(Mat src)
        {
            using (ResourceTracker t = new ResourceTracker())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Size srcSize = src.Size();

                //todo: 只对脸部进行亮肤https://blog.csdn.net/cnbloger/article/details/77949949
                //https://blog.csdn.net/skyqsdyy/article/details/89467143
                //todo: 对src进行美颜、瘦脸、亮肤等。只要控制在一帧50ms之内即可。
                Mat matMask = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0));
                RenderGreenScreenMask(src, matMask);
                //the area is by integer instead of double, so that it can improve the performance of comparision of areas
                int minBlockArea = (int)(srcSize.Width * srcSize.Height * this.MinBlockPercent);
                var contoursExternalForeground = Cv2.FindContoursAsArray(matMask, RetrievalModes.External, ContourApproximationModes.ApproxNone)
                    .Select(c => new { contour = c, Area = (int)Cv2.ContourArea(c) })
                    .Where(c => c.Area >= minBlockArea)
                    .OrderByDescending(c => c.Area).Take(5).Select(c => c.contour);
                //contoursExternalForeground = contoursExternalForeground.Select(c=>Cv2.ApproxPolyDP(c,0.5,true));

                //a new Mat used for rendering the selected Contours
                var matMaskForeground = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0));
                //thickness: -1 means filling the inner space 
                matMaskForeground.DrawContours(contoursExternalForeground, -1, new Scalar(255),
                    thickness: -1);

                //matInternalHollow is the inner Hollow parts of body part.
                var matInternalHollow = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0));
                Cv2.BitwiseXor(matMaskForeground, matMask, matInternalHollow);

                int minHollowArea = (int)(minBlockArea * 0.5);//the lower size limitation of InternalHollow is less than minBlockArea, because InternalHollows are smaller
                //find the Contours of Internal Hollow  
                var contoursInternalHollow = Cv2.FindContoursAsArray(matInternalHollow, RetrievalModes.External, ContourApproximationModes.ApproxNone)
                    .Select(c => new { contour = c, Area = Cv2.ContourArea(c) })
                    .Where(c => c.Area >= minHollowArea)
                    .OrderByDescending(c => c.Area).Take(10).Select(c => c.contour);
                //draw hollows
                foreach (var c in contoursInternalHollow)
                {
                    matMaskForeground.FillConvexPoly(c, new Scalar(0));
                }

                var element = t.T(Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3)));
                //smooth the edge of matMaskForeground
                Cv2.MorphologyEx(matMaskForeground, matMaskForeground, MorphTypes.Close,
                    element, iterations: 6);
                Cv2.GaussianBlur(matMaskForeground, matMaskForeground, new Size(3, 3), 0, 0);

                var foreground = t.NewMat(src.Size(), MatType.CV_8UC4, new Scalar(0));
                ZackCVHelper.AddAlphaChannel(src, foreground, matMaskForeground);
                if(_backgroundImage!=null)
                {
                    //resize the _backgroundImage to the same size of src
                    Cv2.Resize(_backgroundImage, src, src.Size());
                }
                //GammaCorrect(foreground, 0.8);

                //draw foreground(people) on the backgroundimage
                ZackCVHelper.DrawOverlay(src, foreground);
                Debug.WriteLine($"5:{sw.ElapsedMilliseconds}");
            }
        }

        public void Dispose()
        {
            if (this._backgroundImage != null)
            {
                this._backgroundImage.Dispose();
            }
        }
    }
}
