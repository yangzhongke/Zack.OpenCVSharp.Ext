[中文版文档(Chinese version)](https://github.com/yangzhongke/Zack.OpenCVSharp.Ext/blob/main/README_CN.md)

# Zack.OpenCVSharp.Ext
It is an extension library of OpenCvSharp. It provides ResourceTracker, which can facilitate the resources management of Mat and other unmanaged resources. It also provide a class, named np, which is a .NET native and  managed version of Numpy.

NuGet Package

```
Install-Package Zack.OpenCVSharp.Ext
```
## ResourceTracker
In OpenCVSharp, Mat and MatExpr have unmanaged resources, so they should be disposed be code. However, the code is tedious. Worst of all, every operator, like +,-,* and others, will create new objects, and they should by disposed one by one. The tedious code is as follow.
```csharp
using (Mat mat1 = new Mat(new Size(100, 100), MatType.CV_8UC3))
using (Mat mat2 = mat1 * 0.8)
using (Mat mat3 = 255-mat2)
{
	Mat[] mats1 = mat3.Split();
	using (Mat mat4 = new Mat())
	{
		Cv2.Merge(new Mat[] { mats1[0], mats1[1], mats1[2] }, mat4);
	}
	foreach(var m in mats1)
	{
		m.Dispose();
	}
}
```

The class ResourceTracker is used for managing OpenCV resources, like Mat, MatExpr, etc.
* T(). The method T() of ResourceTracker is used for add OpenCV objects to the tracking records. After Dispose() of ResourceTracker is called, all the resoruces kept by ResourceTracker will be disposed. The method T() can take one object and an array of objects.
* NewMat(). The method NewMat() is a combination of T(new Mat(...)) 

Sample code:

```csharp
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
```

Because every operator, like +,-,* and others, will create a new object, so they should be wrapped by T(). For example: t.T(255 - t.T(picMat * 0.8))

## np
The class np is like the np of NumPy in Python.
Because the syntax of Python is different from that of C#, I don't the .NET binding version of Numpy is a good idea. Furthermore, the ported packages, like Numpy.NET, cannot take advantages of C# syntax sugar, like lambda.

Therefore, I created the managed version of np, which provies zeros_like, array, where. I'm a newbie to the Numpy field, and I didn't do much research in Numpy, so I didn't implement most methods of Numpy. I will be appreciated if any one can contribute more methods to np.cs.

## GreenScreenRemovalDemo
The project GreenScreenRemovalDemo is a practical case. It can remove the green screen of a video or the images from the webcamera.
