#coding:utf-8
import numpy as np
import cv2

_greenScale = 35
_minBlockPercent = 0.01
_backgroundImage = cv2.imread("e:/temp/bg.png")

def addAlphaChannel(src, alpha):
    b,g,r = cv2.split(src)
    #split is used for splitting the channels separately
    return cv2.merge((b,g,r,alpha))

def renderGreenScreenMask(src):
    #https://codereview.stackexchange.com/questions/184044/processing-an-image-to-extract-green-screen-mask
    RED, GREEN, BLUE = (2, 1, 0)
    reds = src[:, :, RED]
    greens = src[:, :, GREEN]
    blues = src[:, :, BLUE]
    mask = (greens < 35) | (reds > greens) | (blues > greens)
    #astype(np.uint8): https://answers.opencv.org/question/225478/convert-a-2d-numpy-array-to-cv_8uc1-type/
    mask = np.where(mask, 255, 0).astype(np.uint8)
    return mask
    

#https://stackoverflow.com/questions/54069766/overlaying-an-image-over-another-image-both-with-transparent-background-using-op   
def drawOverlay(bg, overlay):
    y1, y2 = 0, overlay.shape[0]
    x1, x2 = 0, overlay.shape[1]
    alpha_s = overlay[:, :, 3] / 255.0
    alpha_l = 1.0 - alpha_s

    for c in range(0, 3):
        bg[y1:y2, x1:x2, c] = (alpha_s * overlay[:, :, c] +
                                  alpha_l * bg[y1:y2, x1:x2, c])

def apply(src):
    srcSize = (src.shape[0],src.shape[1])
    #遮罩
    #matMask = np.zeros((srcSize[0],srcSize[1], 1), dtype = "uint8")
    #renderGreenScreenMask(src, matMask)
    matMask = renderGreenScreenMask(src)
    #最小连续区域面积
    minBlockArea = src.shape[0] * src.shape[1] * _minBlockPercent
    #从matMask中查找连续区域，连续区域的数据在contours中
    contours,h = cv2.findContours(matMask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)

    #只取出来contours中面积大的一些
    contoursExternalForeground =[]
    for contourMat in contours:
        contourArea = cv2.contourArea(contourMat)
        if(contourArea>minBlockArea):
            contoursExternalForeground.append(contourMat)

    #前景遮罩
    matMaskForeground = np.zeros((srcSize[0],srcSize[1], 1), dtype = "uint8")
    #thickness: -1 means filling the inner space 
    #把大面积的识别出来的“身体”绘制到matMaskForeground上
    cv2.drawContours(matMaskForeground,contoursExternalForeground,-1,255, thickness=-1);

    #身体里的“镂空”区域
    matInternalHollow = cv2.bitwise_xor(matMaskForeground, matMask)

    #把大面积的识别出来的“身体内镂空”绘制到matMaskForeground上
    minHollowArea = minBlockArea * 0.01 #the lower size limitation of InternalHollow is less than minBlockArea, because InternalHollows are smaller
    #find the Contours of Internal Hollow 
    contoursInternalHollow,h = cv2.findContours(matInternalHollow,cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE);
    for contourMat in contoursInternalHollow:
        contourArea = cv2.contourArea(contourMat)
        if(contourArea>minHollowArea):
            cv2.fillConvexPoly(matMaskForeground,contourMat,(0)) 
    
    foreground = addAlphaChannel(src, matMaskForeground)
    newPic = cv2.resize(_backgroundImage, (srcSize[1],srcSize[0]))
    #draw foreground(people) on the backgroundimage
    drawOverlay(newPic, foreground)
    return newPic

videoCapture = cv2.VideoCapture("E:\\主同步盘\\我的坚果云\\MyCode\\DOTNET\\Zack.OpenCVSharp.Ext\\GreenScreenRemovalDemo\\monster.mp4");
while(True):
    ret, matFrame = videoCapture.read()
    matFrame = apply(matFrame)
    cv2.imshow("press any key to quit",matFrame)
    if(cv2.waitKey(1)>0):
        videoCapture.release()
        cv2.destroyAllWindows() 
        break

