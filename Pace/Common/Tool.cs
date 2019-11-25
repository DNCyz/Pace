using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Pace.Common
{
    public class Tool
    {
        /// <summary>
        /// 两个时间点相差的秒数
        /// </summary>
        /// <param name="dtStart">起始时间点</param>
        /// <param name="dtEnd">截至时间点</param>
        /// <returns></returns>
        public static long DiffSeconds(DateTime dtStart, DateTime dtEnd)
        {
            try
            {
                long lDiff = Math.Abs((dtEnd.Ticks - dtStart.Ticks) / 10000000);
                return lDiff;
            }
            catch { }
            return 0;
        }
        /// <summary>
        /// 从文本框中转换成行字符串数组
        /// </summary>
        /// <param name="txtBox"></param>
        /// <returns></returns>
        public static string[] GetFromTextBox(TextBox txtBox)
        {
            try
            {
                string txt = txtBox.Text;
                txt = txt.Replace("\r\n", "¤");
                string[] txtArray = txt.Split('¤');
                return txtArray;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 从文本中转换成行字符串数组
        /// </summary>
        /// <param name="txtBox"></param>
        /// <returns></returns>
        public static string[] GetFromString(string txt)
        {
            try
            {
                //string txt = txtBox.Text;
                txt = txt.Replace("\r\n", "¤");
                string[] txtArray = txt.Split('¤');
                return txtArray;
            }
            catch { }
            return null;
        }


    }

    /// <summary>
    /// 日志数据写入公共类
    ///   Cyz增补 2017-10-26
    /// </summary>
    public class LogWrite
    {
        private static bool _IsWriteLog = true;
        /// <summary>
        /// 是否允许数据写入
        /// </summary>
        public static bool IsWriteLog
        {
            get
            {
                return _IsWriteLog;
            }
            set
            {
                _IsWriteLog = value;
            }
        }

        /// <summary>
        /// 锁对像
        ///   Cyz注:向外存(如硬盘)设备中的文件写入数据没有在内存中的快，需加锁排队写入
        ///         外部设备资源一般稀缺、缓慢，要"独享"操作.
        /// </summary>
        private static object lockObj = new object();

        #region 导出数据

        /// <summary>
        /// 写日志数据2
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <param name="TextAll"></param>
        /// <param name="ex"></param>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        public static bool TxtExport(string FullFileName, string TextAll, Exception ex, string ModuleName)
        {
            try
            {
                TextAll += (" " + ModuleName);
            }
            catch { }
            return TxtExport(FullFileName, TextAll);
        }

        /// <summary>
        /// 自动添加当前日期时间的导出数据处理
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <param name="TextAll"></param>
        /// <returns></returns>
        public static bool TxtExport_DateNow(string FullFileName, string TextAll)
        {
            return TxtExport(FullFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + "  " + TextAll);
        }

        /// <summary>
        /// 自动添加当前日期时间的导出数据处理2
        /// </summary>
        /// <param name="TextAll"></param>
        /// <returns></returns>
        public static bool TxtExport_DateNow(string TextAll)
        {
            return TxtExport_DateNow("DNCyzTest.txt", TextAll);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <param name="TextAll"></param>
        /// <returns></returns>        
        public static bool TxtExport(string FullFileName, string TextAll)
        {
            try
            {
                if (!IsWriteLog)
                {
                    return false;
                }
                lock (lockObj)
                {
                    if (!CreatFile(ref FullFileName))
                    {
                        return false;
                    }
                    StreamWriter sw = new StreamWriter(FullFileName, true, Encoding.UTF8);   //该编码类型不会改变已有文件的编码类型
                    sw.WriteLine(TextAll);
                    sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(FullFileName + ".Et" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), true, Encoding.UTF8);   //该编码类型不会改变已有文件的编码类型
                sw.WriteLine(TextAll);
                sw.WriteLine("Err:" + ex.Message.ToString());
                sw.Close();
                return true;
            }

        }
        ///// <summary>
        ///// 日志数据打印保存
        ///// </summary>
        ///// <param name="FullFileName">日志文件全路径(如:D:\Log\Log20150809.txt)</param>
        ///// <param name="Msg">日志数据内容</param>
        //public static void LogSave(string FullFileName, string Msg)
        //{
        //    lock (lockObj)
        //    {
        //        StreamWriter sw = null;
        //        try
        //        {
        //            sw = new StreamWriter(FullFileName, true, Encoding.UTF8);
        //            sw.WriteLine(Msg);                    
        //        }
        //        catch { }
        //        finally
        //        {
        //            try
        //            {
        //                if (sw != null)
        //                {
        //                    sw.Close();
        //                }
        //            }
        //            catch { }
        //        }
        //    }
        //}

        /// <summary>
        /// 导出数据2
        ///    无需再处理文件路径
        /// </summary>
        /// <param name="FullFileName"></param>
        /// <param name="TextAll"></param>
        /// <returns></returns>        
        public static bool TxtExport2(string FullFileName, string TextAll)
        {
            try
            {
                StreamWriter sw = new StreamWriter(FullFileName, true, Encoding.UTF8);   //该编码类型不会改变已有文件的编码类型
                sw.WriteLine(TextAll);
                sw.Close();
                return true;
            }
            catch
            {
                StreamWriter sw = new StreamWriter(FullFileName + ".Et" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), true, Encoding.UTF8);   //该编码类型不会改变已有文件的编码类型
                sw.WriteLine(TextAll);
                sw.Close();
                return true;
            }
        }

        #endregion 导出数据

        #region 创建文件（文件存在则跳过）

        /// <summary>
        /// 创建文件（文件存在则跳过）
        /// </summary>
        /// <param name="FullFileName">带路径的文件名</param>
        /// <returns></returns>
        private static bool CreatFile(ref string FullFileName)
        {
            bool bResult = false;

            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                if (!Directory.Exists(@"Log\" + strDate))//若文件夹不存在则新建文件夹  
                {
                    Directory.CreateDirectory(@"Log\" + strDate); //新建文件夹  
                }
            }
            catch { }
            FullFileName = @"Log\" + strDate + @"\" + FullFileName;

            try
            {
                if (File.Exists(FullFileName))
                {
                    bResult = true;
                }
                else
                {
                    try
                    {
                        FileStream fs = new FileStream(FullFileName, FileMode.CreateNew);
                        fs.Close();
                        bResult = true;
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message.ToString());
                        bResult = false;
                    }
                }
            }
            catch { }
            return bResult;
        }

        #endregion 创建文件（文件存在则跳过）

        #region 硬件日志数据记录

        /// <summary>
        /// 硬件日志数据记录
        /// </summary>
        /// <param name="strLogContent"></param>
        /// <returns></returns>
        public static bool HardWorkLogDeal(string strLogContent)
        {
            try
            {
                return TxtExport("HardLog.Log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + strLogContent);
            }
            catch { }
            return false;
        }

        #endregion 硬件日志数据记录
    }

}
