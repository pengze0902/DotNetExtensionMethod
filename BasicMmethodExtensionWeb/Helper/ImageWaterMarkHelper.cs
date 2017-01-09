using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Org.BouncyCastle.X509.Store;

namespace BasicMmethodExtensionWeb.Helper
{
    public class ImageWaterMark
    {
        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="watermarkFilename">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        public static void AddImageSignPic(Image img, string filename, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(watermarkFilename))
            {
                throw new ArgumentNullException(watermarkFilename);
            }
            Graphics g = null;
            Image watermark = null;
            ImageAttributes imageAttributes = null;
            try
            {
                g = Graphics.FromImage(img);
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                watermark = new Bitmap(watermarkFilename);
                if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                {
                    return;
                }
                imageAttributes = new ImageAttributes();
                var colorMap = new ColorMap
                {
                    OldColor = Color.FromArgb(255, 0, 255, 0),
                    NewColor = Color.FromArgb(0, 0, 0, 0)
                };
                ColorMap[] remapTable = { colorMap };
                imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                var transparency = 0.5F;
                if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                {
                    transparency = (watermarkTransparency / 10.0F);
                }
                float[][] colorMatrixElements =
                {
                    new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                    new[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                    new[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, transparency, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                };
                var colorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                var xpos = 0;
                var ypos = 0;
                switch (watermarkStatus)
                {
                    case 1:
                        xpos = (int)(img.Width * (float).01);
                        ypos = (int)(img.Height * (float).01);
                        break;
                    case 2:
                        xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                        ypos = (int)(img.Height * (float).01);
                        break;
                    case 3:
                        xpos = (int)((img.Width * (float).99) - (watermark.Width));
                        ypos = (int)(img.Height * (float).01);
                        break;
                    case 4:
                        xpos = (int)(img.Width * (float).01);
                        ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                        break;
                    case 5:
                        xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                        ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                        break;
                    case 6:
                        xpos = (int)((img.Width * (float).99) - (watermark.Width));
                        ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                        break;
                    case 7:
                        xpos = (int)(img.Width * (float).01);
                        ypos = (int)((img.Height * (float).99) - watermark.Height);
                        break;
                    case 8:
                        xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                        ypos = (int)((img.Height * (float).99) - watermark.Height);
                        break;
                    case 9:
                        xpos = (int)((img.Width * (float).99) - (watermark.Width));
                        ypos = (int)((img.Height * (float).99) - watermark.Height);
                        break;
                }
                g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0,
                    watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
                var codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (var codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg", StringComparison.Ordinal) > -1)
                    {
                        ici = codec;
                    }
                }
                var encoderParams = new EncoderParameters();
                var qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                {
                    quality = 80;
                }
                qualityParam[0] = quality;
                var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;
                if (ici != null)
                {
                    img.Save(filename, ici, encoderParams);
                }
                else
                {
                    img.Save(filename);
                }

            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (ExternalException extex)
            {
                throw extex;
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
                img.Dispose();
                if (watermark != null)
                {
                    watermark.Dispose();
                }
                if (imageAttributes != null)
                {
                    imageAttributes.Dispose();
                }
            }

        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontname">字体</param>
        /// <param name="fontsize">字体大小</param>
        public static void AddImageSignText(Image img, string filename, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(filename);
            }
            if (string.IsNullOrEmpty(watermarkText))
            {
                throw new ArgumentNullException(watermarkText);
            }
            var g = Graphics.FromImage(img);
            var drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            var crSize = g.MeasureString(watermarkText, drawFont);
            float xpos = 0;
            float ypos = 0;
            switch (watermarkStatus)
            {
                case 1:
                    xpos = img.Width * (float).01;
                    ypos = img.Height * (float).01;
                    break;
                case 2:
                    xpos = (img.Width * (float).50) - (crSize.Width / 2);
                    ypos = img.Height * (float).01;
                    break;
                case 3:
                    xpos = (img.Width * (float).99) - crSize.Width;
                    ypos = img.Height * (float).01;
                    break;
                case 4:
                    xpos = img.Width * (float).01;
                    ypos = (img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = (img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = (img.Width * (float).99) - crSize.Width;
                    ypos = (img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = img.Width * (float).01;
                    ypos = (img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = (img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = (img.Width * (float).99) - crSize.Width;
                    ypos = (img.Height * (float).99) - crSize.Height;
                    break;
            }
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos, ypos);
            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg", StringComparison.Ordinal) > -1)
                {
                    ici = codec;
                }
            }
            var encoderParams = new EncoderParameters();
            var qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }
            qualityParam[0] = quality;
            var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            if (ici != null)
            {
                img.Save(filename, ici, encoderParams);
            }
            else
            {
                img.Save(filename);
            }
            g.Dispose();
            img.Dispose();
        }

        /**/
        /// <summary>
        /// 文字和Logo图片水印
        /// </summary>
        /// <param name="imgFile">原图文件地址</param>
        /// <param name="waterImg">水印图片地址</param>
        /// <param name="textFont">水印文字信息</param>
        /// <param name="sImgPath">生存水印图片后的保存地址</param>
        /// <param name="imgAlpha">水印图片的透明度</param>
        /// <param name="imgiScale">水印图片在原图上的显示比例</param>
        /// <param name="intimgDistance">水印图片在原图上的边距确定,以图片的右边和下边为准,当设定的边距超过一定大小后参数会自动失效</param>
        /// <param name="texthScale">水印文字高度位置,从图片底部开始计算，0－1</param>
        /// <param name="textwidthFont">文字块在图片中所占宽度比例 0－1</param>
        /// <param name="textAlpha">文字透明度 其数值的范围在0到255</param>
        public void ZzsImgTextWater(string imgFile, string waterImg, string textFont, string sImgPath, float imgAlpha, float imgiScale,
            int intimgDistance, float texthScale, float textwidthFont, int textAlpha)
        {
            try
            {
                var fs = new FileStream(imgFile, FileMode.Open);
                var br = new BinaryReader(fs);
                var bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                var ms = new MemoryStream(bytes);
                var imgPhoto = Image.FromStream(ms);
                var imgPhotoWidth = imgPhoto.Width;
                var imgPhotoHeight = imgPhoto.Height;
                var bmPhoto = new Bitmap(imgPhotoWidth, imgPhotoHeight, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);
                var gbmPhoto = Graphics.FromImage(bmPhoto);
                gbmPhoto.InterpolationMode = InterpolationMode.High;
                gbmPhoto.SmoothingMode = SmoothingMode.HighQuality;
                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, imgPhotoWidth, imgPhotoHeight)
                 , 0, 0, imgPhotoWidth, imgPhotoHeight, GraphicsUnit.Pixel
                 );
                //建立字体大小的数组,循环找出适合图片的水印字体
                int[] sizes = { 1000, 800, 700, 650, 600, 560, 540, 500, 450, 400, 380, 360, 340, 320, 300, 280, 260, 240, 220, 200, 180, 160, 140, 120, 100, 80, 72, 64, 48, 32, 28, 26, 24, 20, 28, 16, 14, 12, 10, 8, 6, 4, 2 };
                Font crFont = null;
                SizeF crSize = new SizeF();
                for (var i = 0; i < 43; i++)
                {
                    crFont = new Font("arial", sizes[i], FontStyle.Bold);
                    crSize = gbmPhoto.MeasureString(textFont, crFont);
                    if ((ushort)crSize.Width < (ushort)imgPhotoWidth * textwidthFont)
                        break;
                }
                //设置水印字体的位置
                var yPixlesFromBottom = (int)(imgPhotoHeight * texthScale);
                var yPosFromBottom = ((imgPhotoHeight - yPixlesFromBottom) - (crSize.Height / 2));
                float xCenterOfImg = (imgPhotoWidth * 1 / 2);
                var strFormat = new StringFormat { Alignment = StringAlignment.Center };
                var semiTransBrush2 = new SolidBrush(Color.FromArgb(textAlpha, 0, 0, 0));
                gbmPhoto.DrawString(textFont, crFont, semiTransBrush2, new PointF(xCenterOfImg + 1, yPosFromBottom + 1), strFormat);
                var semiTransBrush = new SolidBrush(Color.FromArgb(textAlpha, 255, 255, 255));
                gbmPhoto.DrawString(textFont, crFont, semiTransBrush, new PointF(xCenterOfImg, yPosFromBottom), strFormat);
                Image imgWatermark = new Bitmap(waterImg);
                var imgWatermarkWidth = imgWatermark.Width;
                var imgWatermarkHeight = imgWatermark.Height;
                //计算水印图片尺寸
                var aScale = Convert.ToDecimal(imgiScale);
                var pScale = 0.05M;
                var minScale = aScale - pScale;
                var maxScale = aScale + pScale;
                var imgWatermarkWidthNew = imgWatermarkWidth;
                var imgWatermarkHeightNew = imgWatermarkHeight;
                if (imgPhotoWidth >= imgWatermarkWidth && imgPhotoHeight >= imgWatermarkHeight && imgPhotoWidth >= imgPhotoHeight)
                    if (imgWatermarkWidth > imgWatermarkHeight)
                        if ((minScale <= Math.Round((Convert.ToDecimal(imgWatermarkWidth) / Convert.ToDecimal(imgPhotoWidth)), 7)) && (Math.Round((Convert.ToDecimal(imgWatermarkWidth) / Convert.ToDecimal(imgPhotoWidth)), 7) <= maxScale))
                        {
                        }
                        else
                        {
                            imgWatermarkWidthNew = Convert.ToInt32(imgPhotoWidth * aScale);
                            imgWatermarkHeightNew = Convert.ToInt32((imgPhotoWidth * aScale / imgWatermarkWidth) * imgWatermarkHeight);
                        }
                    else
                        if ((minScale <= Math.Round((Convert.ToDecimal(imgWatermarkHeight) / Convert.ToDecimal(imgPhotoHeight)), 7)) && (Math.Round((Convert.ToDecimal(imgWatermarkHeight) / Convert.ToDecimal(imgPhotoHeight)), 7) <= maxScale))
                    {
                    }
                    else
                    {
                        imgWatermarkHeightNew = Convert.ToInt32(imgPhotoHeight * aScale);
                        imgWatermarkWidthNew = Convert.ToInt32((imgPhotoHeight * aScale / imgWatermarkHeight) * imgWatermarkWidth);
                    }
                if (imgWatermarkWidth >= imgPhotoWidth && imgWatermarkHeight >= imgPhotoHeight && imgWatermarkWidth >= imgWatermarkHeight)
                {
                    imgWatermarkWidthNew = Convert.ToInt32(imgPhotoWidth * aScale);
                    imgWatermarkHeightNew = Convert.ToInt32(((imgPhotoWidth * aScale) / imgWatermarkWidth) * imgWatermarkHeight);
                }
                if (imgWatermarkWidth >= imgPhotoWidth && imgWatermarkHeight <= imgPhotoHeight && imgPhotoWidth >= imgPhotoHeight)
                {
                    imgWatermarkWidthNew = Convert.ToInt32(imgPhotoWidth * aScale);
                    imgWatermarkHeightNew = Convert.ToInt32(((imgPhotoWidth * aScale) / imgWatermarkWidth) * imgWatermarkHeight);
                }
                if (imgWatermarkWidth <= imgPhotoWidth && imgWatermarkHeight >= imgPhotoHeight && imgPhotoWidth >= imgPhotoHeight)
                {
                    imgWatermarkHeightNew = Convert.ToInt32(imgPhotoHeight * aScale);
                    imgWatermarkWidthNew = Convert.ToInt32(((imgPhotoHeight * aScale) / imgWatermarkHeight) * imgWatermarkWidth);
                }
                if (imgPhotoWidth >= imgWatermarkWidth && imgPhotoHeight >= imgWatermarkHeight && imgPhotoWidth <= imgPhotoHeight)
                    if (imgWatermarkWidth > imgWatermarkHeight)
                        if ((minScale <= Math.Round((Convert.ToDecimal(imgWatermarkWidth) / Convert.ToDecimal(imgPhotoWidth)), 7)) && (Math.Round((Convert.ToDecimal(imgWatermarkWidth) / Convert.ToDecimal(imgPhotoWidth)), 7) <= maxScale))
                        {
                        }
                        else
                        {
                            imgWatermarkWidthNew = Convert.ToInt32(imgPhotoWidth * aScale);
                            imgWatermarkHeightNew = Convert.ToInt32(((imgPhotoWidth * aScale) / imgWatermarkWidth) * imgWatermarkHeight);
                        }
                    else
                        if ((minScale <= Math.Round((Convert.ToDecimal(imgWatermarkHeight) / Convert.ToDecimal(imgPhotoHeight)), 7)) && (Math.Round((Convert.ToDecimal(imgWatermarkHeight) / Convert.ToDecimal(imgPhotoHeight)), 7) <= maxScale))
                    {
                    }
                    else
                    {
                        imgWatermarkHeightNew = Convert.ToInt32(imgPhotoHeight * aScale);
                        imgWatermarkWidthNew = Convert.ToInt32(((imgPhotoHeight * aScale) / imgWatermarkHeight) * imgWatermarkWidth);
                    }
                if (imgWatermarkWidth >= imgPhotoWidth && imgWatermarkHeight >= imgPhotoHeight && imgWatermarkWidth <= imgWatermarkHeight)
                {
                    imgWatermarkHeightNew = Convert.ToInt32(imgPhotoHeight * aScale);
                    imgWatermarkWidthNew = Convert.ToInt32(((imgPhotoHeight * aScale) / imgWatermarkHeight) * imgWatermarkWidth);
                }
                if (imgWatermarkWidth >= imgPhotoWidth && imgWatermarkHeight <= imgPhotoHeight && imgPhotoWidth <= imgPhotoHeight)
                {
                    imgWatermarkWidthNew = Convert.ToInt32(imgPhotoWidth * aScale);
                    imgWatermarkHeightNew = Convert.ToInt32(((imgPhotoWidth * aScale) / imgWatermarkWidth) * imgWatermarkHeight);
                }
                if (imgWatermarkWidth <= imgPhotoWidth && imgWatermarkHeight >= imgPhotoHeight && imgPhotoWidth <= imgPhotoHeight)
                {
                    imgWatermarkHeightNew = Convert.ToInt32(imgPhotoHeight * aScale);
                    imgWatermarkWidthNew = Convert.ToInt32(((imgPhotoHeight * aScale) / imgWatermarkHeight) * imgWatermarkWidth);
                }
                //将原图画出来
                var bmWatermark = new Bitmap(bmPhoto);
                bmWatermark.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                var gWatermark = Graphics.FromImage(bmWatermark);
                //指定高质量显示水印图片质量
                gWatermark.InterpolationMode = InterpolationMode.High;
                gWatermark.SmoothingMode = SmoothingMode.HighQuality;
                var imageAttributes = new ImageAttributes();
                //设置两种颜色,达到合成效果
                ColorMap colorMap = new ColorMap
                {
                    OldColor = Color.FromArgb(255, 0, 255, 0),
                    NewColor = Color.FromArgb(0, 0, 0, 0)
                };
                ColorMap[] remapTable = { colorMap };
                imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                //用矩阵设置水印图片透明度
                float[][] colorMatrixElements = {
                    new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                    new[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                    new[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, imgAlpha, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                    };
                var wmColorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //确定水印边距
                var xPos = imgPhotoWidth - imgWatermarkWidthNew;
                var yPos = imgPhotoHeight - imgWatermarkHeightNew;
                int xPosOfWm;
                int yPosOfWm;
                if (xPos > intimgDistance)
                    xPosOfWm = xPos - intimgDistance;
                else
                    xPosOfWm = xPos;
                if (yPos > intimgDistance)
                    yPosOfWm = yPos - intimgDistance;
                else
                    yPosOfWm = yPos;
                gWatermark.DrawImage(imgWatermark, new Rectangle(xPosOfWm, yPosOfWm, imgWatermarkWidthNew, imgWatermarkHeightNew)
                                , 0, 0, imgWatermarkWidth, imgWatermarkHeight, GraphicsUnit.Pixel, imageAttributes);
                imgPhoto = bmWatermark;
                //以jpg格式保存图片
                imgPhoto.Save(sImgPath, ImageFormat.Jpeg);
                //销毁对象
                gbmPhoto.Dispose();
                gWatermark.Dispose();
                bmPhoto.Dispose();
                imgPhoto.Dispose();
                imgWatermark.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 缩略图
        /// </summary>
        /// <param name="imgFile">原图文件地址</param>
        /// <param name="sImgPath">缩略图保存地址</param>
        /// <param name="resizeWidth">缩略图宽度</param>
        /// <param name="resizeHeight">缩略图高度</param>
        /// <param name="bgColor">缩略图背景颜色,注意,背景颜色只能指定KnownColor中的值,如blue,red,green等</param>
        public bool ZzsResizeImg(string imgFile, string sImgPath, int resizeWidth, int resizeHeight, string bgColor)
        {
            if (string.IsNullOrEmpty(imgFile))
            {
                throw new ArgumentNullException(imgFile);
            }
            if (string.IsNullOrEmpty(sImgPath))
            {
                throw new ArgumentNullException(sImgPath);
            }
            try
            {
                var fs = new FileStream(imgFile, FileMode.Open);
                var br = new BinaryReader(fs);
                var bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                var ms = new MemoryStream(bytes);
                var imgPhoto = Image.FromStream(ms);
                int imgPhotoWidth = imgPhoto.Width;
                int imgPhotoHeight = imgPhoto.Height;
                int startX = 0;
                int startY = 0;
                int newWidth = 0;
                int newHeight = 0;
                if (imgPhotoWidth >= resizeWidth && imgPhotoHeight >= resizeHeight)
                {
                    newWidth = resizeWidth;
                    newHeight = Convert.ToInt32(imgPhotoHeight * Math.Round(Convert.ToDecimal(resizeWidth) / Convert.ToDecimal(imgPhotoWidth), 10));
                    startX = 0;
                    startY = (resizeHeight - newHeight) / 2;
                }
                if (resizeWidth > imgPhotoWidth && resizeHeight < imgPhotoHeight)
                {
                    newHeight = resizeHeight;
                    newWidth = Convert.ToInt32(imgPhotoWidth * Math.Round(Convert.ToDecimal(resizeHeight) / Convert.ToDecimal(imgPhotoHeight), 10));
                    startX = (resizeWidth - newWidth) / 2;
                    startY = 0;
                }
                if (resizeWidth < imgPhotoWidth && resizeHeight > imgPhotoHeight)
                {
                    newWidth = resizeWidth;
                    newHeight = Convert.ToInt32(imgPhotoHeight * Math.Round(Convert.ToDecimal(resizeWidth) / Convert.ToDecimal(imgPhotoWidth), 10));
                    startX = 0;
                    startY = (resizeHeight - newHeight) / 2;
                }
                if (imgPhotoWidth < resizeWidth && imgPhotoHeight < resizeHeight)
                {
                    newWidth = imgPhotoWidth;
                    newHeight = imgPhotoHeight;
                    startX = (resizeWidth - imgPhotoWidth) / 2;
                    startY = (resizeHeight - imgPhotoHeight) / 2;
                }
                //计算缩放图片尺寸
                Bitmap bmPhoto = new Bitmap(resizeWidth, resizeHeight, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);
                Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
                gbmPhoto.Clear(Color.FromName(bgColor));
                gbmPhoto.InterpolationMode = InterpolationMode.High;
                gbmPhoto.SmoothingMode = SmoothingMode.HighQuality;
                gbmPhoto.DrawImage(
                  imgPhoto
                 , new Rectangle(startX, startY, newWidth, newHeight)
                 , new Rectangle(0, 0, imgPhotoWidth, imgPhotoHeight)
                 , GraphicsUnit.Pixel
                 );
                bmPhoto.Save(sImgPath, ImageFormat.Jpeg);
                imgPhoto.Dispose();
                gbmPhoto.Dispose();
                bmPhoto.Dispose();
                ms.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 图片剪切
        /// </summary>
        /// <param name="imgFile">原图文件地址</param>
        /// <param name="sImgPath">缩略图保存地址</param>
        /// <param name="pointX">剪切起始点 X坐标</param>
        /// <param name="pointY">剪切起始点 Y坐标</param>
        /// <param name="cutWidth">剪切宽度</param>
        /// <param name="cutHeight">剪切高度</param>
        public bool ZzsCutImg(string imgFile, string sImgPath, int pointX, int pointY, int cutWidth, int cutHeight)
        {
            if (string.IsNullOrEmpty(imgFile))
            {
                throw new ArgumentNullException(imgFile);
            }
            if (string.IsNullOrEmpty(sImgPath))
            {
                throw new ArgumentNullException(sImgPath);
            }
            MemoryStream ms = null;
            Image imgPhoto = null;
            Graphics gbmPhoto = null;
            Bitmap bmPhoto = null;
            try
            {
                var fs = new FileStream(imgFile, FileMode.Open);
                var br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                ms = new MemoryStream(bytes);
                imgPhoto = Image.FromStream(ms);
                //此处只能用filestream，用 System.Drawing.Image则会报多过进程访问文件的错误，会锁定文件
                bmPhoto = new Bitmap(cutWidth, cutHeight, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);
                gbmPhoto = Graphics.FromImage(bmPhoto);
                gbmPhoto.InterpolationMode = InterpolationMode.High;
                gbmPhoto.SmoothingMode = SmoothingMode.HighQuality;
                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, cutWidth, cutHeight),
                    new Rectangle(pointX, pointY, cutHeight, cutHeight)
                    , GraphicsUnit.Pixel);
                bmPhoto.Save(sImgPath, ImageFormat.Jpeg);
                return true;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            catch (IOException ioex)
            {
                throw ioex;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
                if (imgPhoto != null)
                {
                    imgPhoto.Dispose();
                }
                if (gbmPhoto != null)
                {
                    gbmPhoto.Dispose();
                }
                if (bmPhoto != null)
                {
                    bmPhoto.Dispose();
                }
            }
        }

        /// <summary>
        /// 获得图片的类型
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static ImageFormat ImgFormat(string photo)
        {
            //获得图片的后缀,不带点，小写
            var imgExt = photo.Substring(photo.LastIndexOf(".", StringComparison.Ordinal) + 1, photo.Length - photo.LastIndexOf(".", StringComparison.Ordinal) - 1).ToLower();
            ImageFormat imgFormat;
            switch (imgExt)
            {
                case "png":
                    imgFormat = ImageFormat.Png;
                    break;
                case "gif":
                    imgFormat = ImageFormat.Gif;
                    break;
                case "bmp":
                    imgFormat = ImageFormat.Bmp;
                    break;
                default:
                    imgFormat = ImageFormat.Jpeg;
                    break;
            }
            return imgFormat;
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式
        /// <code>HW:指定高宽缩放（可能变形）</code>
        /// <code>W:指定宽，高按比例  </code>
        /// <code>H:指定高，宽按比例</code>
        /// <code>CUT:指定高宽裁减（不变形） </code>
        /// <code>FILL:填充</code>
        /// </param>    
        public static bool LocalImage2Thumbs(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            if (string.IsNullOrEmpty(originalImagePath))
            {
                throw new ArgumentNullException(originalImagePath);
            }
            if (string.IsNullOrEmpty(thumbnailPath))
            {
                throw new ArgumentNullException(thumbnailPath);
            }
            if (string.IsNullOrEmpty(mode))
            {
                throw new ArgumentNullException(mode);
            }
            Image originalImage = null;
            try
            {
                originalImage = Image.FromFile(originalImagePath);
                Image2Thumbs(originalImage, thumbnailPath, width, height, mode);
                return true;
            }
            catch (OutOfMemoryException otex)
            {
                throw otex;
            }
            catch (FileNotFoundException fileNotex)
            {
                throw fileNotex;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            finally
            {
                if (originalImage != null)
                {
                    originalImage.Dispose();
                }
            }

        }

        /// <summary>
        /// 生成远程图片的缩略图
        /// </summary>
        /// <param name="remoteImageUrl"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool RemoteImage2Thumbs(string remoteImageUrl, string thumbnailPath, int width, int height, string mode)
        {
            if (string.IsNullOrEmpty(remoteImageUrl))
            {
                throw new ArgumentNullException(remoteImageUrl);
            }
            if (string.IsNullOrEmpty(thumbnailPath))
            {
                throw new ArgumentNullException(thumbnailPath);
            }
            if (string.IsNullOrEmpty(mode))
            {
                throw new ArgumentNullException(mode);
            }
            Image originalImage = null;
            try
            {
                var request = WebRequest.Create(remoteImageUrl);
                request.Timeout = 20000;
                var stream = request.GetResponse().GetResponseStream();
                if (stream == null)
                {
                    return true;
                }
                originalImage = Image.FromStream(stream);
                Image2Thumbs(originalImage, thumbnailPath, width, height, mode);
                return true;
            }
            catch (NotSupportedException seex)
            {
                throw seex;
            }
            catch (ArgumentException arex)
            {
                throw arex;
            }
            finally
            {
                if (originalImage != null)
                {
                    originalImage.Dispose();
                }
            }

        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImage">源图</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="photoWidth">最终缩略图宽度</param>
        /// <param name="height">最终缩略图高度</param>
        /// <param name="photoHeight"></param>
        /// <param name="mode">生成缩略图的方式
        /// <code>HW:指定高宽缩放（可能变形）</code>
        /// <code>W:指定宽，高按比例  </code>
        /// <code>H:指定高，宽按比例</code>
        /// <code>CUT:指定高宽裁减（不变形） </code>
        /// <code>FILL:填充</code>
        /// </param> 
        public static void Image2Thumbs(Image originalImage, string thumbnailPath, int photoWidth, int photoHeight, string mode)
        {
            //最后缩略图的宽度
            var lastPhotoWidth = photoWidth;
            //最后缩略图的高度
            var lastPhotoHeight = photoHeight;
            //原图片被压缩的宽度
            var toWidth = photoWidth;
            //原图片被压缩的高度
            var toHeight = photoHeight;
            var x = 0;
            var y = 0;
            var ow = originalImage.Width;
            var oh = originalImage.Height;
            var bgX = 0;
            var bgY = 0;
            switch (mode.ToUpper())
            {
                //压缩填充至指定区域
                case "FILL":
                    toHeight = photoHeight;
                    toWidth = toHeight * ow / oh;
                    if (toWidth > photoWidth)
                    {
                        toHeight = toHeight * photoWidth / toWidth;
                        toWidth = photoWidth;
                    }
                    bgX = (photoWidth - toWidth) / 2;
                    bgY = (photoHeight - toHeight) / 2;
                    break;
                //指定高宽缩放（可能变形）
                case "HW":
                    break;
                //指定宽，高按比例 
                case "W":
                    toHeight = lastPhotoHeight = originalImage.Height * photoWidth / originalImage.Width;
                    break;
                //指定高，宽按比例
                case "H":
                    toWidth = lastPhotoWidth = originalImage.Width * photoHeight / originalImage.Height;
                    break;
                //指定高宽裁减（不变形）
                case "CUT":
                    if (originalImage.Width / (double)originalImage.Height > lastPhotoWidth / (double)lastPhotoHeight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * lastPhotoWidth / lastPhotoHeight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * photoHeight / lastPhotoWidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
            }
            //新建一个bmp图片
            Image bitmap = new Bitmap(lastPhotoWidth, lastPhotoHeight);
            //新建一个画板
            var g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            //白色
            g.Clear(Color.White);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(bgX, bgY, toWidth, toHeight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(thumbnailPath, ImgFormat(thumbnailPath));
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (ExternalException exex)
            {
                throw exex;
            }
            finally
            {
                bitmap.Dispose();
                g.Dispose();
            }

        }


        /// <summary>
        /// 切割后生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="toW">缩略图最终宽度</param>
        /// <param name="toH">缩略图最终高度</param>
        /// <param name="X">X坐标（zoom为1时）</param>
        /// <param name="Y">Y坐标（zoom为1时）</param>
        /// <param name="W">选择区域宽（zoom为1时）</param>
        /// <param name="H">选择区域高（zoom为1时）</param>
        public static void MakeMyThumbs(string originalImagePath, string thumbnailPath, int toW, int toH, int X, int Y, int W, int H)
        {
            if (string.IsNullOrEmpty(originalImagePath))
            {
                throw new ArgumentNullException(originalImagePath);
            }
            if (string.IsNullOrEmpty(thumbnailPath))
            {
                throw new ArgumentNullException(thumbnailPath);
            }
            var originalImage = Image.FromFile(originalImagePath);
            var towidth = toW;
            var toheight = toH;
            var x = X;
            var y = Y;
            var ow = W;
            var oh = H;
            Image bitmap = null;
            Graphics g = null;
            try
            {
                bitmap = new Bitmap(towidth, toheight);
                //新建一个画板
                g = Graphics.FromImage(bitmap);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

                bitmap.Save(thumbnailPath, ImgFormat(thumbnailPath));
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (ExternalException etex)
            {
                throw etex;
            }
            finally
            {
                originalImage.Dispose();
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
            }
        }
    }
}