/*
    《"跬步"脚本语言微手册》
    撰写人:Cyz
    撰写日期:2019-09
    版本:V0.01

【"跬步"脚本语言的定位】
对标Python,自创"跬步"配置用脚本语言,摒弃传统程序代码中的选择、循环结构,
编写计算机程序代码就像小学生写作文一样简单.

【"跬步"脚本语言对标的开发语言】
Python。
Python最初是用于Unix/Linux的自动化运维之用,所以Python一开始就带有强烈的脚本语言气息
并自带“胶水”特性。
为了学好Pyhton,理解Python的创立初衷,所以自己也照着其思路再创建一种脚本脚本语言。
但这并不意味着"跬步"脚本语言毫无用处,恰恰相反,"跬步"脚本语言的产生一开始就是为了
解决现实应用问题的。

【使用"跬步"脚本语言要达到的目的】
编写脚本就像小学生写作文那样一行一句地写,且当前行的代码只与其上一行代码有关联。
并且,摒弃掉传统程序代码中所要用到的选择、循环结构。
其中,把需要使用选择、循环结构编写程序代码才能实现的功能进行封装供后续开发调用或配置，
就像人类撰写文章为了表达复杂的想法引用成语或名人名言。

【"跬步"脚本语言应用场合】
目前先应用于停车场系统的收费实现,特别是应用于非典型(定制)的收费实现。

【"跬步"脚本语言的主要使用人员】
1、技术支持人员。
2、软件开发人员。

【"跬步"脚本语言的架构实现】
1、内部架构:解释执行引擎 + "跬步"语言脚本。
2、外部架构:停车云平台 + 线下停车场系统 + "跬步"脚本语言撰写端。(注:先以停车收费实现为例)
           [存储与转发]   [接收与执行]       [撰写与上传] 

【"跬步"脚本语言语法示例说明】
语法结构:
“◎”+“命令名”+“实参数列表或虚参数列表”
其中,◎表示该行代码的执行结果为费用,该费用为前面行代码(含本行代码)计算出费用的累加值。
     命令名、各项参数之间用半角逗号隔开。
     虚参数项名被“<>”包裹住,表示数据由外部动态输入。
     虚参数“[on]”表示此参数以上一行代码的执行结果作输入参数值。

◎Duration0,<InDt>,<OutDt>,0.5          注释:车辆出入时长在半小时的费用。
QuitJudge,[on],0                        注释:上一行获取的费用为0时,退出整个执行过程。
TimeRange,08:00,18:00,<InDt>,<OutDt>    注释:获取车辆在当天08:00至18:00时段的停车时长。
◎PerHour,4.00,[on]                     注释:从上一行获取的停车时长按每小时4元计算出费用
                                             累加进先前费用里,并输出。
TimeRange,18:00,08:00,<InDt>,<OutDt>    注释:获取车辆在当天18:00至次日08:00时段的停车时长。
◎PerHour,5.00,[on]                     注释:从上一行获取的停车时长按每小时5元计算出费用
                                             累加进先前费用里,并输出。
◎Duration,<InDt>,<OutDt>,24,[on],30.00 注释:根据从上一行获取的费用,
                                             停车时长在24小时以内,按上一行获取的费用输出,
                                             停车时长在24小时以上,从上一行获取的费用未超
                                             过30元,按上一行获取的费用输出,
                                             停车时长在24小时以上,从上一行获取的费用已超
                                             过30元,直接按30元费用输出。

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
