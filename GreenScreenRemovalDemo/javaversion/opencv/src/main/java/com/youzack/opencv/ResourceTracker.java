package com.youzack.opencv;

import java.util.LinkedList;

import org.bytedeco.opencv.opencv_core.Mat;
import org.bytedeco.opencv.opencv_core.Scalar;
import org.bytedeco.opencv.opencv_core.Size;

public class ResourceTracker implements AutoCloseable {
	private LinkedList<AutoCloseable> objects = new LinkedList<AutoCloseable>();
	
	public <TCV extends AutoCloseable> TCV T(TCV obj)
	{
		objects.add(obj);
		return obj;
	}

	public void close() throws Exception {
		for(AutoCloseable obj : objects)
		{
			obj.close();
		}
		objects.clear();
	}
	
	public Mat newMat(Size size,int matType,Scalar s)
	{
		return T(new Mat(size,matType,s));
	}
	
	public Mat newMat()
	{
		return T(new Mat());
	}
}
