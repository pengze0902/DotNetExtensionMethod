using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    public class EncryptXmlDocumentHelper
    {
        /// <summary>
        /// 该方法进行数字签名XML文件传递的参数和
        ///写相同名称，sobreponto通过参数告知XML签署的XML。
        ///它还提供了一个与物业签订XML字符串（this.vXmlStringAssinado）
        /// </summary>
        /// <param name="arqXmlAssinar">要签名XML文件名</param>
        /// <param name="tagAssinatura">标签名在得到签名</param>
        /// <param name="tagAtributoId">有将要签署的ID属性标记标签名称</param>
        /// <param name="x509Cert">在签名时使用证书</param>
        public void EncryptXmlDocument(string arqXmlAssinar, string tagAssinatura, string tagAtributoId, X509Certificate2 x509Cert)
        {
            StreamReader sr = null;
            try
            {
                //打开XML文件进行签名和阅读你的内容
                sr = System.IO.File.OpenText(arqXmlAssinar);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                sr = null;
                // 创建一个新的XML文档。
                XmlDocument doc = new XmlDocument { PreserveWhitespace = false };
                doc.LoadXml(xmlString);
                if (doc.GetElementsByTagName(tagAssinatura).Count == 0)
                {
                    throw new Exception(tagAssinatura.Trim());
                }
                if (doc.GetElementsByTagName(tagAtributoId).Count == 0)
                {
                    throw new Exception(tagAtributoId.Trim());
                }
                // 有多个标记进行签名
                XmlNodeList lists = doc.GetElementsByTagName(tagAssinatura);
                foreach (XmlNode nodes in lists)
                {
                    foreach (XmlNode childNodes in nodes.ChildNodes)
                    {
                        if (!childNodes.Name.Equals(tagAtributoId))
                            continue;
                        if (childNodes.NextSibling != null && childNodes.NextSibling.Name.Equals("Signature"))
                            continue;
                        //创建要签名的引用
                        Reference reference = new Reference { Uri = "" };
                        //需要要签名的孩子                                   
                        XmlElement childElemen = (XmlElement)childNodes;
                        if (childElemen.GetAttributeNode("Id") != null)
                        {
                            var attributeNode = childElemen.GetAttributeNode("Id");
                            if (attributeNode != null)
                                reference.Uri = "#" + attributeNode.Value;
                        }
                        else if (childElemen.GetAttributeNode("id") != null)
                        {
                            var attributeNode = childElemen.GetAttributeNode("id");
                            if (attributeNode != null)
                                reference.Uri = "#" + attributeNode.Value;
                        }
                        XmlDocument documentoNovo = new XmlDocument();
                        documentoNovo.LoadXml(nodes.OuterXml);
                        // 创建一个SignedXml对象。
                        SignedXml signedXml = new SignedXml(documentoNovo) { SigningKey = x509Cert.PrivateKey };
                        // 将密钥添加到签名的Xml文档
                        // 向引用添加包络变换。
                        XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                        reference.AddTransform(env);
                        XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                        reference.AddTransform(c14);
                        // 将引用添加到SignedXml对象。
                        signedXml.AddReference(reference);
                        // 创建一个新的键入对象
                        KeyInfo keyInfo = new KeyInfo();
                        // 将证书加载到KeyInfoX509Data对象中
                        //并将它添加到KeyInfo对象。
                        keyInfo.AddClause(new KeyInfoX509Data(x509Cert));
                        //将KeyInfo对象添加到SignedXml对象。
                        signedXml.KeyInfo = keyInfo;
                        signedXml.ComputeSignature();
                        // 获取签名的XML表示并保存它到一个XmlElement对象。
                        XmlElement xmlDigitalSignature = signedXml.GetXml();
                        // 保存该元素的XML文档中
                        nodes.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                    }
                }

                var xmlDoc = doc;
                // XML的更新字符串已经签约
                var stringXmlAssinado = xmlDoc.OuterXml;
                // 编写XML高清签名
                StreamWriter sw2 = System.IO.File.CreateText(arqXmlAssinar);
                sw2.Write(stringXmlAssinado);
                sw2.Close();
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException(ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
            }
        }
    }
}
