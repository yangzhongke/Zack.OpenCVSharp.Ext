# Zack.OpenCVSharp.Ext
这是OpenCvSharp的一个扩展库。这里面的ResourceTracker类用来简化Mat等资源的管理。它也提供了名字叫做np的类，他是Numpy的.NET原生托管版本。

NuGet 包

```
Install-Package Zack.OpenCVSharp.Ext
```
## ResourceTracker
在OpenCVSharp中，Mat 和 MatExpr等类有非托管资源，所以他们需要通过代码手动释放。但是，代码非常啰嗦。更糟糕的是，+、-、*等运算符每次都会创建一个新的对象，这些对象都需要释放。啰嗦的代码就像这样的。

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

ResourceTracker类用来管理OpenCV的资源，例如 Mat、 MatExpr等。
* T(). ResourceTracker类的T()方法用于把OpenCV对象加入跟踪记录，然后再把对象返回，所以T()方法就相当于一个待释放资源的包裹器。当ResourceTracker类的 Dispose()方法被调用后，ResourceTracker跟踪的所有资源都会被释放。T()方法可以跟踪一个对象或者一个对象数组。
* NewMat(). 这个方法是T(new Mat(...)) 的一个简化。

例子代码：

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

因为+、-、*等运算符每次都会创建一个新的对象，这些对象都需要释放，他们可以使用T()进行包裹。例如：t.T(255 - t.T(picMat * 0.8))

## np
np类就像Python里的np一样。因为Python的语法和C#的不一样，我认为Numpy的.NET绑定版不是最好的方案。而且，Numpy.NET之类的移植包无法充分利用C#的lambda之类的语法糖。
因此，我创建了一个np的托管版本，它提供了zeros_like, array, where等方法。我是Numpy领域的新人，我在Numpy领域也还没深入研究，因此我没有实现Numpy中的大部分方法。如果您原因贡献更多代码到np类，我会非常开心。

## GreenScreenRemovalDemo
GreenScreenRemovalDemo项目是一个实际的案例，它用来移除视频或者摄像头中的绿幕。

关于这个项目的文章: [编程去除背景绿幕抠图，基于.NET+OpenCVSharp](https://www.bilibili.com/read/cv8850462)

GreenScreenRemovalDemo的二进制可执行程序:
* Windows(X86 and X64): [从 百度网盘下载](https://pan.baidu.com/s/1mRs4etacjO-jb1b7iH1R3Q)  （提取码：6cu7）||  [从 OneDrive下载](https://1drv.ms/u/s!ArtUX5uRoj_cmWWM1xf0CfVMx4FI?e=YupHDl)
* MacOS (osx.10.15-x64):[从 百度网盘下载](https://pan.baidu.com/s/1bSxtpFxnfXwl1VfhpGO-rQ) （提取码：w7ki）||  [从 OneDrive下载](https://1drv.ms/u/s!ArtUX5uRoj_cmWIlLKUw77KVx0r7?e=zfdafg)
* centos7-x64: [从 OneDrive下载](https://1drv.ms/u/s!ArtUX5uRoj_cmWMwztai5lT-ag8n?e=rTiejq)
* ubuntu.16.04-x64: [从 OneDrive下载](https://1drv.ms/u/s!ArtUX5uRoj_cmWbBCY5TRpcwjb_y?e=xWBBkx)
* debian.10-amd64:[从 OneDrive下载](https://1drv.ms/u/s!ArtUX5uRoj_cmWQcrpCEMT3cNjBz?e=0fg5XV)