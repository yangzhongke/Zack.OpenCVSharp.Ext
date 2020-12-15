using System;
using System.Collections.Generic;
namespace OpenCvSharp
{
    public class ResourceTracker : IDisposable
    {
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
        }
    }
}
