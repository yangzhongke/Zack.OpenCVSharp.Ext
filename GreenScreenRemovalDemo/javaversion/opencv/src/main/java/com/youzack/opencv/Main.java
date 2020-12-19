package com.youzack.opencv;

import static org.bytedeco.opencv.global.opencv_highgui.imshow;
import static org.bytedeco.opencv.global.opencv_highgui.waitKey;
import static org.bytedeco.opencv.global.opencv_imgcodecs.imread;

import org.bytedeco.opencv.opencv_core.Mat;
import org.bytedeco.opencv.opencv_videoio.VideoCapture;

public class Main {

	public static void main(String[] args) {
		try(ResourceTracker t = new ResourceTracker())
		{
			Mat bgMat = t.T(imread("e:/temp/bg.png"));
			ReplaceGreenScreenFilter filter = new ReplaceGreenScreenFilter(bgMat);
			VideoCapture videoCapture = t.T(new VideoCapture("E:\\主同步盘\\我的坚果云\\MyCode\\DOTNET\\Zack.OpenCVSharp.Ext\\GreenScreenRemovalDemo\\monster.mp4"));
			Mat matFrame = new Mat();
			while(true)
			{
				if(!videoCapture.read(matFrame))
				{
					System.out.println("the end");
					break;
				}
				filter.apply(matFrame);
				imshow("press any key to quit",matFrame);
				if(waitKey(33)>0)
				{
					break;
				}
			}
		}
		catch(Exception ex)
		{
			ex.printStackTrace();
		}

	}

}
