using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCvSharp
{
	public static class ZackCVHelper
	{
		/// <summary>
		/// draw overlay(has transparent background ) on bg
		/// </summary>
		/// <param name="bg"></param>
		/// <param name="overlay"></param>
		public unsafe static void DrawOverlay(Mat bg, Mat overlay)
		{
			if (bg.Size() != overlay.Size())
			{
				throw new System.ArgumentException("bg.Size()!=overlay.Size()");
			}
			if (overlay.Channels() < 4)
			{
				throw new System.ArgumentException("overlay.Channels()<4");
			}
			int colsOverlay = overlay.Cols;
			int rowsOverlay = overlay.Rows;
			//https://stackoverflow.com/questions/54069766/overlaying-an-image-over-another-image-both-with-transparent-background-using-op
			for (int i = 0; i < rowsOverlay; i++)
			{
				Vec3b* pBg = (Vec3b*)bg.Ptr(i);
				Vec4b* pOverlay = (Vec4b*)overlay.Ptr(i);
				for (int j = 0; j < colsOverlay; j++)
				{
					Vec3b* pointBg = pBg + j;
					Vec4b* pointOverlay = pOverlay + j;
					if (pointOverlay->Item3 != 0)
					{
						pointBg->Item0 = pointOverlay->Item0;
						pointBg->Item1 = pointOverlay->Item1;
						pointBg->Item2 = pointOverlay->Item2;
					}
				}
			}
		}

		/// <summary>
		///     Adds transparency channel to source image and writes to output image.
		/// </summary>
		public static void AddAlphaChannel(Mat src, Mat dst, Mat alpha)
		{
			using (ResourceTracker t = new ResourceTracker())
			{
				//split is used for splitting the channels separately
				var bgr = t.T(Cv2.Split(src));
				var bgra = new[] { bgr[0], bgr[1], bgr[2], alpha };
				Cv2.Merge(bgra, dst);
			}
		}
	}
}
