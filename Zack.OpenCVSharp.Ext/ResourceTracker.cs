using OpenCvSharp.Util;
using System;
using System.Collections.Generic;
namespace OpenCvSharp
{
    [Obsolete("This class has been moved into OpenCVSharp as a part of OpenCVSharp, please use the  ResourcesTracker under the namespace of OpenCvSharp.Util instead. See https://github.com/shimat/opencvsharp/pull/1110")]
    public class ResourceTracker : ResourcesTracker
    {
        /*
        private ISet<DisposableObject> trackedObjects = new HashSet<DisposableObject>();
        private object asyncLoc = new object();

        /// <summary>
        /// Trace the object obj, and return it
        /// </summary>
        /// <typeparam name="TCV"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TCV T<TCV>(TCV obj) where TCV : DisposableObject
        {
            if (obj == null)
            {
                return obj;
            }
            lock (asyncLoc)
            {
                trackedObjects.Add(obj);
            }
            return obj;
        }

        public TCV[] T<TCV>(TCV[] objs) where TCV : DisposableObject
        {
            foreach (var obj in objs)
            {
                T(obj);
            }
            return objs;
        }


        public Mat NewMat()
        {
            return T(new Mat());
        }

        public Mat NewMat(Size size, MatType matType, Scalar s)
        {
            return T(new Mat(size, matType, s));
        }

        public void DisposeAll()
        {
            lock (asyncLoc)
            {
                foreach (var obj in trackedObjects)
                {
                    if (obj.IsDisposed == false)
                    {
                        obj.Dispose();
                    }
                }
                trackedObjects.Clear();
            }
        }


        public void Dispose()
        {
            DisposeAll();
        }*/
    }
}
