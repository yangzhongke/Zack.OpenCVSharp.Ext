using System;

namespace OpenCvSharp
{
    public static class np
    {
        //https://stackoverflow.com/questions/52016253/python-np-array-equivilent-in-opencv-opencvsharp
        public static Mat zeros_like(Mat a)
        {
            return new Mat(a.Size(), a.Type(),new Scalar(0));
        }
        public static Mat array(params byte[] array_like)
        {
            return Mat.FromArray(array_like);
        }

        public static Mat array(Vec3b[] array_like)
        {
            return Mat.FromArray(array_like);
        }

        public static Mat array(Vec2b[] array_like)
        {
            return Mat.FromArray(array_like);
        }

        public static Mat array(Vec4b[] array_like)
        {
            return Mat.FromArray(array_like);
        }

        public unsafe static void where<T>(Mat src, Mat dst, Func<T, bool> condition, Mat x, Mat y) where
            T: unmanaged
        {
            if(!src.IsContinuous()|| !dst.IsContinuous() || !x.IsContinuous() || !y.IsContinuous())
            {
                throw new ArgumentException("all the Mat should be IsContinuous()==true");
            }
            //https://github.com/opencv/opencv/pull/8311
            //https://github.com/opencv/opencv/issues/8304
            //f.At(); f.ElemSize(); f.Ptr() are recommended instead of using pointer for memory safety,
            //however, they are low performant.
            T* pSrcStart = (T*)src.DataStart;
            T* pSrcEnd = (T*)src.DataEnd;
            T* px = (T*)x.DataStart;
            T* py = (T*)y.DataStart;
            T* pDst = (T*)dst.DataStart;
            for (T* pSrc = pSrcStart; pSrc <= pSrcEnd; pSrc++, pDst++, px++, py++)
            {
                *pDst = condition(*pSrc) ? *px : *py;
            }
        }

        public static byte clip(byte value,byte min,byte max)
        {
            if(value>max)
            {
                return max;
            }
            if(value<min)
            {
                return min;
            }
            return value;
        }
    }
}
