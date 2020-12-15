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
        }
    }

}
