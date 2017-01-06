using System;
using System.Drawing;
using System.IO;
using System.Web;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace BasicMmethodExtensionWeb.Helper
{
    public class ThoughtWorksQrCodeHelper
    {
        //生成二维码方法一

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">带生成二维码的字符串</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string CreatehoughtWorksQrCode(string content, string path)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(content);
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }
            var qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeScale = 4,
                QRCodeVersion = 8,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M
            };
            Image image = qrCodeEncoder.Encode(content);
            var filename = DateTime.Now.ToString("yyyymmddhhmmssfff") + ".jpg";
            var filepath = string.Format("{0}{1}", path, filename);
            FileStream fs = null;
            try
            {
                fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
                image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message);
            }
            finally
            {
                if (fs != null) fs.Close();
                image.Dispose();
            }
            return CodeDecoder(filepath);
        }


        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns></returns>
        public static string CodeDecoder(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(filePath);
            }
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }                 
                var myBitmap = new Bitmap(Image.FromFile(filePath));
                var decoder = new QRCodeDecoder();
                var decodedString = decoder.decode(new QRCodeBitmapImage(myBitmap));
                return decodedString;
            }
            catch (IOException ioex)
            {
                throw ioex;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// 选择生成二维码的相关类型
        /// <param name="strData">要生成的文字或者数字，支持中文。如： "4408810820 深圳－广州" 或者：4444444444</param>
        /// <param name="qrEncoding">三种尺寸：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="level">大小：L M Q H</param>
        /// <param name="version">版本：如 8</param>
        /// <param name="scale">比例：如 4</param>
        /// <returns></returns>
        /// </summary>
        public void CreateCode_Choose(string strData, string qrEncoding, string level, int version, int scale)
        {
            if (string.IsNullOrEmpty(strData))
            {
                throw new ArgumentNullException(strData);
            }
            if (string.IsNullOrEmpty(qrEncoding))
            {
                throw new ArgumentNullException(qrEncoding);
            }
            if (string.IsNullOrEmpty(level))
            {
                throw new ArgumentNullException(level);
            }
            var qrCodeEncoder = new QRCodeEncoder();
            var encoding = qrEncoding;
            switch (encoding)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = scale;
            qrCodeEncoder.QRCodeVersion = version;
            switch (level)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }
            Image image = null;
            FileStream fs = null;
            try
            {
                //文字生成图片
                image = qrCodeEncoder.Encode(strData);
                var filename = DateTime.Now.ToString("yyyymmddhhmmssfff") + ".jpg";
                var filepath = HttpContext.Current.Server.MapPath(@"~\Upload") + "\\" + filename;
                fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
                image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (image != null)
                {
                    image.Dispose();
                }
            }
        }

    }
}
