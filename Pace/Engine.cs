/*
   Cyz
   2019-09
   V0.01
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pace.Common;

namespace Pace
{
    /// <summary>
    /// 引擎类
    /// </summary>
    public class Engine
    {
        private static Engine engineObj = new Engine();

        public static Engine Init()
        {
            return engineObj;
        }

        private Engine() { }

        /// <summary>
        /// 执行动态方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="parameterArray">参数列表</param>
        /// <returns></returns>
        private object Execute(string methodName, object[] parameterArray)
        {
            try
            {
                // function name
                string name = methodName; //max

                // function parameter(s)
                object[] parameter = parameterArray; //new object[2];
                //parameter[0] = Convert.ToDouble(code.Substring(4, 1)); //1
                //parameter[1] = Convert.ToDouble(code.Substring(6, 1)); //2 

                var function = Activator.CreateInstance(typeof(BLLDeal/*Function*/)) as BLLDeal/*Function*/;
                var method = function.GetType().GetMethod(name);
                object result = method.Invoke(function, parameter);

                return result;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// "跬步"脚本语言引擎启动运行
        /// </summary>
        /// <param name="scriptArray">表达式行列表</param>
        /// <returns></returns>
        public string Run(string[] scriptArray)
        {
            float fResult = 0;
            try
            {
                string previouslySObj = "";
                foreach (string str in scriptArray)
                {
                    try
                    {
                        bool isFee = false;//是否为算费处理
                        string[] cmdArray = null;
                        string[] strArray = str.Split('◎');
                        if (strArray.Length >= 2)//当前行表达式为算费处理
                        {
                            isFee = true;
                            cmdArray = strArray[1].Split(',');
                        }
                        else
                        {
                            cmdArray = strArray[0].Split(',');
                        }
                        for (int i = 0; i < cmdArray.Length; i++)
                        {
                            if (cmdArray[i] == "[on]")
                            {
                                cmdArray[i] = previouslySObj;
                            }
                        }
                        string[] parameterArray = new string[cmdArray.Length - 1];
                        Array.Copy(cmdArray, 1, parameterArray, 0, parameterArray.Length);
                        object obj = this.Execute(cmdArray[0], parameterArray);
                        string sObj = obj.ToString();
                        if (isFee && (sObj != "NaN"))
                        {
                            fResult += float.Parse(sObj);
                            sObj = fResult.ToString();
                        }
                        previouslySObj = sObj;
                        if (sObj.Trim().ToLower() == "true" && cmdArray[0] == "QuitJudge")
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWrite.TxtExport_DateNow("LogErr.txt", "Engine Run [内]Err:" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogWrite.TxtExport_DateNow("LogErr.txt", "Engine Run Err:" + ex.ToString());
            }
            return fResult.ToString();
        }

    }
    /// <summary>
    /// 业务运算处理类
    /// </summary>
    public class BLLDeal
    {
        public string Test123(int a, int b)
        {
            return "【富贵'创'中求】" + (a + b).ToString();
        }

        public string 获取当前时间(string name)
        {
            return name + "获取当前时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 是否可退出。为真表示退出此次的处理过程.
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="quitValue">判断阀值(当前值小等于此值时为真)</param>
        /// <returns></returns>
        public /*bool*/string QuitJudge(string value, string quitValue)
        {
            if (value == "NaN")
            {
                return false.ToString();
            }
            return (float.Parse(value) <= float.Parse(quitValue)).ToString();
        }

        /// <summary>
        /// 出入场免费时长
        /// </summary>
        /// <param name="InDt">车辆入场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <param name="OutDt">车辆离场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <param name="freehours">免费小时数</param>
        /// <returns></returns>
        public string Duration0(string InDt, string OutDt, string freehours)
        {
            try
            {
                DateTime dtIn = DateTime.Parse(InDt);
                DateTime dtOut = DateTime.Parse(OutDt);
                long lDiff = Tool.DiffSeconds(dtIn, dtOut);
                long lTmp = (long)(float.Parse(freehours) * 60 * 60);
                if (lDiff <= lTmp)
                {
                    return "0";
                }
            }
            catch { }
            return "NaN";
        }

        /// <summary>
        /// 注:"跬步"脚本语言中被封装的功能方法 内部实现代码 的示例
        /// 
        /// 获取在当天起始时间点与当天结束时间点时段内的停车小时数
        /// </summary>
        /// <param name="StartDtTag">当天起始时间点(如:08:00)</param>
        /// <param name="EndDtTag">当天结束时间点(如:18:00)</param>
        /// <param name="InDt">车辆入场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <param name="OutDt">车辆离场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <returns></returns>
        public /*float*/string TimeRange(string StartDtTag, string EndDtTag, string InDt, string OutDt)
        {
            try
            {
                string CurrDay = DateTime.Now.ToString("yyyy-MM-dd");
                DateTime dtStartDtTag = DateTime.Parse(CurrDay + " " + StartDtTag + ":00");
                DateTime dtEndDtTag = DateTime.Parse(CurrDay + " " + EndDtTag + ":59");
                if (dtEndDtTag < dtStartDtTag)
                {
                    dtEndDtTag = dtEndDtTag.AddHours(24);
                }
                DateTime dtIn = DateTime.Parse(InDt);
                DateTime dtOut = DateTime.Parse(OutDt);

                DateTime dtSart = DateTime.Now;
                DateTime dtEnd = DateTime.Now;

                bool bInFlag = false;
                if (dtIn <= dtStartDtTag && dtOut <= dtEndDtTag && dtOut >= dtStartDtTag)
                {
                    bInFlag = true;
                    dtSart = dtStartDtTag;
                }
                else if (dtIn >= dtStartDtTag && dtOut >= dtEndDtTag && dtIn <= dtEndDtTag)
                {
                    bInFlag = true;
                    dtSart = dtIn;
                }
                else if (dtIn <= dtStartDtTag && dtOut >= dtEndDtTag)
                {
                    bInFlag = true;
                    dtSart = dtStartDtTag;
                }
                else if (dtIn >= dtStartDtTag && dtOut <= dtEndDtTag)
                {
                    bInFlag = true;
                    dtSart = dtIn;
                }

                bool bOutFlag = false;

                if (dtOut <= dtEndDtTag)
                {
                    bOutFlag = true;
                    dtEnd = dtOut;
                }
                else if (dtOut > dtEndDtTag)
                {
                    bOutFlag = true;
                    dtEnd = dtEndDtTag;
                }

                if (bInFlag && bOutFlag)
                {
                    long lDiff = Tool.DiffSeconds(dtSart, dtEnd);
                    float fResult = (float)((lDiff / 60.00) / 60.00);
                    return fResult.ToString();
                }
            }
            catch { }
            return "0";
        }

        /// <summary>
        /// 按时长单价算费
        /// </summary>
        /// <param name="unitPrice"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public /*float*/string PerHour(string unitPrice, string duration)
        {
            string str = (float.Parse(unitPrice) * float.Parse(duration)).ToString();
            return str;
        }

        /// <summary>
        /// 收费限制，停车超过设定的小时数后，按设定的最高费用收费
        /// </summary>
        /// <param name="InDt">车辆入场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <param name="OutDt">车辆离场时间点("yyyy-MM-dd HH:mm:ss"格式)</param>
        /// <param name="durationHours">设定的小时数</param>
        /// <param name="fee">计算出的费用</param>
        /// <param name="maxFee">限制的费用</param>
        /// <returns></returns>
        public string Duration(string InDt, string OutDt, string durationHours, string fee, string maxFee)
        {
            try
            {
                DateTime dtIn = DateTime.Parse(InDt);
                DateTime dtOut = DateTime.Parse(OutDt);
                long lDiff = Tool.DiffSeconds(dtIn, dtOut);
                long lTmp = (long)(float.Parse(durationHours) * 60 * 60);
                //if (lDiff >= lTmp)
                //{
                //    return maxFee.ToString();
                //}
                //return fee.ToString();
                return this.FeeCompensate(fee, maxFee, (lDiff >= lTmp));
            }
            catch { }
            return "NaN";
        }

        /// <summary>
        /// 费用补偿计算处理
        /// </summary>
        /// <param name="fee">已累计的费用</param>
        /// <param name="fonstantFee">恒定费用</param>
        /// <param name="isFonstant">是否返回恒定费用</param>
        /// <returns></returns>
        private string FeeCompensate(string fee, string fonstantFee, bool isFonstant)
        {
            try
            {
                float fFee = float.Parse(fee);
                float fFonstantFee = float.Parse(fonstantFee);
                float fResult = 0;
                if (isFonstant)
                {
                    fResult = fFonstantFee - fFee;
                }
                else
                {
                    fResult = 0;
                }
                return fResult.ToString();
            }
            catch { }
            return "NaN";
        }

    }

}
