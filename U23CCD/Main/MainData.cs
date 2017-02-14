using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using BingLibrary.hjb;
using BingLibrary.hjb.Intercepts;
using NationalInstruments.Vision;
using NationalInstruments.VBAI;
using NationalInstruments.VBAI.Structures;
using NationalInstruments.VBAI.Enums;
using NationalInstruments.Vision.WindowsForms;
using System.Windows.Forms;
using System.IO;
using HalconDotNet;
using System.IO.Ports;
namespace Main
{

    [BingAutoNotify]
    public class MainData:DataSource
    {
        public static string iniPath = System.Environment.CurrentDirectory + "\\parameter.ini";
        public static string iniPath1 = System.Environment.CurrentDirectory + "\\VBAI.ini";

        public virtual HImage img { set; get; }
        public virtual string comstatus { set; get; }
        public virtual string BarCode { set; get; } = "";
        public virtual string Receive { set; get; } = "NG";
        public virtual string BC { set; get; }
        public virtual string FB { set; get; }
        public virtual string com { set; get; }
        public virtual double Line { set; get; } = 0;
        public virtual HImage img2 { set; get; }
        public virtual double LineDown{ set; get; }
        public virtual double LineUp { set; get; }
        public virtual VisionImage img3 { set; get; }
        public virtual string Adress { set; get; }
      

        private VBAIClass vBAIClass = new VBAIClass();

    
        public void CameraInit()
        {
            vBAIClass.vbaipath = System.Environment.CurrentDirectory+ "\\a1.vbai";
            Async.RunFuncAsync<bool>(vBAIClass.OpenEngine, openfinishback);
        }
        public void openfinishback(bool ojb)
        {
            Log.Default.Info("视觉引擎加载成功");
        }

        public void CameraInspect()
        {
            Async.RunFuncAsync<List<StepMeasurements>>(vBAIClass.InspectEngine, CameraInspectCallBack);
        }

        public void CameraClose()
        {
            Async.RunFuncAsync(vBAIClass.CloseEngine, backclose);

        }
        public void backclose()
        {
            Log.Default.Info("视觉引擎关闭");
        }

        public void CameraInspectCallBack(List<StepMeasurements> ms)
        {
            
            img3 = vBAIClass.VBAIImage;
            string aaa = "";
            aaa = Inifile.INIGetStringValue(iniPath1, "VBAI INI Variables", "BarCode", "aaaaaaaa");
            BarCode = aaa.Substring(0, aaa.Length - 7);
            if (BarCode=="")
            {
                BarCode = "Error";
                FB = "Red";
            }
            else
            {
                FB = "Green";
            }
            string mm = Inifile.INIGetStringValue(iniPath1, "VBAI INI Variables", "Line", "12.5");
            if (mm!="NaN")
            {
                Line = double.Parse(mm);
                
            }
            else
            {
                Line = 0;
            }
           
            if (Line>LineUp || Line<LineDown)
            {
                Receive = "NG";
                BC = "Red";
               
            }
            else
            {
                Receive = "Pass";
                BC = "Green";
            }
            string[] AA = new string[3];
            AA[0] = BarCode;AA[1] = Line.ToString();AA[2] = Receive;
            Csvfile.savetocsv(Adress, AA);
            Log.Default.Info("拍照成功");
            serial.setM(9, false);


        }

        public static SerialPort sp;
        public static XinjiePlc serial;

        [Initialize]
        public async void InitialSerial()
        {
            await Task.Delay(400);
            bool r = false;
            while (true)
            {

                await Task.Delay(10);
                if (!r)
                {
                    comstatus = "通讯失败";
                    try
                    {
                        serial.Closed();
                        serial.SerialPort1 = sp;

                    }
                    catch { }
                    try
                    {
                        serial = new XinjiePlc(com, 19200, System.IO.Ports.Parity.Even, 8, System.IO.Ports.StopBits.One);
                        r = serial.Connect();
                    }
                    catch { }
                    if (r)
                    {
                        sp = serial.SerialPort1;
                        r = serial.readM(24576);
                    }
                }
                else
                {
                    comstatus = "通讯成功";
                    r = serial.readM(24576);
                    serial.setM(100, false);
                }


            }

        }


       [Initialize]
          public void ReadPar()
        {
          
            com = Inifile.INIGetStringValue(iniPath, "Com", "LocalPort", "COM12");
            LineDown =double.Parse( Inifile.INIGetStringValue(iniPath, "LineDown", "LineDown1", "12.5"));
            LineUp = double.Parse(Inifile.INIGetStringValue(iniPath, "LineUp", "LineUp1", "12.5"));
            Adress = Inifile.INIGetStringValue(iniPath, "Adress", "Adress1", @"F:\test.txt");
            CameraInit();
        }

        bool r = false;
        [Initialize]
        public async void MainRun()
        {
            await Task.Delay(1000);
            while(true)
            {
                await Task.Delay(100);
                r = serial.readM(10);
                if(r)
                {
                    CameraInspect();
                    serial.setM(10, false);
                    r = false;
                   
                    
                }
            }

        }





    }
}
