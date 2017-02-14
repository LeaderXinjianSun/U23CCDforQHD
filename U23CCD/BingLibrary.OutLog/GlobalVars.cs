using System.Windows.Controls;

namespace BingLibrary.OutLog
{
    internal static class GlobalVars
    {
        public static RichTextBox RTB;

        public static void NewRTB(RichTextBox rtb)
        {
            RTB = rtb;
        }
    }
}