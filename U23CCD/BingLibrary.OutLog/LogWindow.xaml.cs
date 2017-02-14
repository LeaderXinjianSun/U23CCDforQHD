using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BingLibrary.OutLog
{
    /// <summary>
    /// LogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogWindow : UserControl
    {
        private const string ConsoleTargetName = "WpfConsole";
        private static Logger _logger;
        private static AsyncTargetWrapper _wrapper;
        private static LogLevel logLevel = LogLevel.Info;

        public LogWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LogMessageProperty =
           DependencyProperty.Register("LogMessage", typeof(string), typeof(LogWindow), new PropertyMetadata(
               new PropertyChangedCallback((d, e) =>
               {
                   var logMessage = e.NewValue as string;
                   if (logMessage != null)
                   {
                       _logger.Log(logLevel, logMessage);
                   }
               }
                 )
               ));

        public string LogMessage
        {
            get { return (string)GetValue(LogMessageProperty); }
            set { SetValue(LogMessageProperty, value); }
        }

        public static readonly DependencyProperty LogTypeProperty =
           DependencyProperty.Register("LogType", typeof(string), typeof(LogWindow), new PropertyMetadata(
               new PropertyChangedCallback((d, e) =>
               {
                   var LogType = e.NewValue as string;
                   if (LogType != null)
                   {
                       if (LogType == "Warn")
                       {
                           logLevel = LogLevel.Warn;
                       }
                       else if (LogType == "Error")
                       {
                           logLevel = LogLevel.Error;
                       }
                       else
                       {
                           logLevel = LogLevel.Info;
                       }
                   }
                   else
                   {
                       logLevel = LogLevel.Info;
                   }
               }
                 )
               ));

        public string LogType
        {
            get { return (string)GetValue(LogTypeProperty); }
            set { SetValue(LogTypeProperty, value); }
        }

        private void LogWindowInitialized(object sender, EventArgs e)
        {
            GlobalVars.NewRTB(TextLog);
            var target = new WpfRichTextBoxTarget
            {
                Name = ConsoleTargetName,
                Layout = "${longdate}|${level:uppercase=true:padding=-5}|${message}|${exception}",
                ControlName = TextLog.Name,
                FormName = Name,
                AutoScroll = true,
                MaxLines = 10000,
                UseDefaultRowColoringRules = true
            };
            _wrapper = new AsyncTargetWrapper
            {
                Name = ConsoleTargetName,
                WrappedTarget = target
            };
            SimpleConfigurator.ConfigureForTargetLogging(_wrapper, LogLevel.Info);
            _logger = LogManager.GetLogger(GetType().Name);
        }
    }
}