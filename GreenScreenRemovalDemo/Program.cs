using OpenCvSharp;
using System;

namespace GreenScreenRemovalDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoCapture videoCapture;
            /*
            Console.WriteLine("Press v key for playing demo video,and Press number key for playing  the specified camera with the index.");
            var c = Console.ReadKey();            
            if (c.Key== ConsoleKey.V)
            {
                videoCapture = new VideoCapture("monster.mp4");
            }
            else if(c.Key>= ConsoleKey.D0&&c.Key<=ConsoleKey.D9)
            {
                int cameraIndex = c.Key - ConsoleKey.D0;
                videoCapture = new VideoCapture(cameraIndex, VideoCaptureAPIs.DSHOW);
            }
            else
            {
                Console.WriteLine("Invalid input.");
                return;
            }*/
           //videoCapture = new VideoCapture(@"E:\主同步盘\我的坚果云\读书笔记及我的文章\技术学习笔记\opencvsharp\yzk真人绿幕~1.mp4");
            videoCapture = new VideoCapture(1, VideoCaptureAPIs.DSHOW);
            using (videoCapture)
            using (Mat frameMat = new Mat())
            using (Mat matBg = Cv2.ImRead("bg.png"))
            using (var filter = new ReplaceGreenScreenFilter(matBg))
            {
                filter.GreenScale = 35;
                if(videoCapture.CaptureType== CaptureType.Camera)
                {
                    videoCapture.FrameWidth = 800;
                    videoCapture.FrameHeight = 600;
                    videoCapture.FourCC = "MJPG";
                }                
                while (true)
                {
                    if (!videoCapture.Read(frameMat))
                    {
                        //maybe it is at the end of the video, so play from the start
                        if(videoCapture.CaptureType== CaptureType.File)
                        {
                            videoCapture.PosFrames = 0;
                        }                        
                        continue;
                    }                        
                    filter.Apply(frameMat);
                    Cv2.ImShow("press any key to quit", frameMat);
                    //WaitKey() not only reads key from user input,
                    //but also prevents UI from frozen.
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
