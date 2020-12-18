package com.youzack.opencv;

import org.bytedeco.javacv.OpenCVFrameConverter;
import org.bytedeco.opencv.opencv_core.Size;
import org.opencv.core.Mat;

public class Utils {
	public static boolean equals(Size s1,Size s2)
	{
		return s1.height()==s2.height()&&s1.width()==s2.width();
	}
}
