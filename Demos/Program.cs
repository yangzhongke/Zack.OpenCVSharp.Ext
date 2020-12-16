using OpenCvSharp;
using System;

namespace Demos
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ResourceTracker t = new ResourceTracker())
            {
                Mat picMat = t.T(Cv2.ImRead("bg.png"));
                Mat mat1 = t.T(255 - picMat);
                Mat mat2 = t.T(np.zeros_like(mat1));
                Mat mat3 = t.NewMat();
                Mat mat4 = t.T(np.array(new byte[] { 33, 88, 99 }));
                Mat mat5 = t.T(np.array(33, 88, 99));
                Mat mat6 = t.T(255 - t.T(picMat * 0.8));
                Mat[] mats1 = t.T(picMat.Split());
                Mat[] mats2 = new Mat[] { mats1[0], mats1[1], mats1[2], t.T(np.zeros_like(picMat)) };
                Cv2.Merge(mats2, mat3);                
            }

            using (ResourceTracker t = new ResourceTracker())
            {
                Mat mat1 = t.NewMat(new Size(100, 100), MatType.CV_8UC3,new Scalar(0));
                Mat mat3 = t.T(255-t.T(mat1*0.8));
                Mat[] mats1 = t.T(mat3.Split());
                Mat mat4 = t.NewMat();
                Cv2.Merge(new Mat[] { mats1[0], mats1[1], mats1[2] }, mat4);
            }

            using (ResourceTracker t = new ResourceTracker())
            {
                var img1 = t.T(Cv2.ImRead("bg.png"));
                var img2 = t.T(Cv2.ImRead("2.jpg"));
                var img2_resized = t.T(np.zeros_like(img1));
                Cv2.Resize(img2, img2_resized, img1.Size());
                var img3 = t.T(np.zeros_like(img1));
                np.where<Vec3b>(img1, img3, p => p.Item0 < 100 || p.Item1 < 100, img1, img2_resized);
                Cv2.ImShow("a",img3);
                Cv2.WaitKey();
            }
        }
    }

}
