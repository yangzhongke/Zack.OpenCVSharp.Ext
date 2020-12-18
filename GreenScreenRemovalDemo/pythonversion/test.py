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

def renderGreenScreenMask(src, matMask):                    
    rows = src.shape[0]
    cols = src.shape[1]
    for x in range(rows):    	
        for y in range(cols):            
            blue,green,red = src[x,y]
            max1 = max(blue,green,red)
            #if this pixel is some green, render the pixel with the same position on matMask as black
            if (green == max1 and green > _greenScale):
                matMask[x, y]=0
            else:
                matMask[x, y]=255

def drawOverlay(bg, overlay):
    rows = bg.shape[0]
    cols = bg.shape[1]
    for x in range(rows):    	
        for y in range(cols):            
            blue,green,red,a = overlay[x,y]
            if(a!=0):
                bg[x, y]=(blue,green,red)
            

def apply(src):
    srcSize = (src.shape[0],src.shape[1])
    #遮罩
    matMask = np.zeros((srcSize[0],srcSize[1], 1), dtype = "uint8")
    renderGreenScreenMask(src, matMask)
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

