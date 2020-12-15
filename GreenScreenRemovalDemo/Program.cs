using OpenCvSharp;
using System;

namespace GreenScreenRemovalDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press v key for playing demo video,and Press number key for playing  the specified camera with the index.");
            var c = Console.ReadKey();
            VideoCapture videoCapture;
            if (c.Key== ConsoleKey.V)
            {
                videoCapture = new VideoCapture("monster.mp4");
            }
            else if(c.Key>= ConsoleKey.D0&&c.Key<=ConsoleKey.D9)
            {
                videoCapture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
            }
            else
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            using (videoCapture)
            using (Mat frameMat = new Mat())
            using (Mat matBg = Cv2.ImRead("bg.png"))
            using (var filter = new ReplaceGreenScreenFilter(matBg))
            {
                filter.GreenScale = 35;
                if(videoCapture.CaptureType== CaptureType.Camera)
                {
                    videoCapture.FrameWidth = 1024;
                    videoCapture.FrameHeight = 768;
                    videoCapture.FourCC = "MJPG";
                }                
                while (true)
                {
                    if (!videoCapture.Read(frameMat))
                        continue;
                    filter.Apply(frameMat);
                    Cv2.ImShow("press any key to quit", frameMat);
                    if(Cv2.WaitKey(33)>0)
                    {
                        break;
                    }
                }
            }
            Cv2.DestroyAllWindows();
        }
    }
}
