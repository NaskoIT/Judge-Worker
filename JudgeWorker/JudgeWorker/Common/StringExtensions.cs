using System;
using System.Text;

namespace JudgeWorker.Common
{
    public static class StringExtensions
    {
        public static string ToUTF8(this string base64String)
        {
            byte[] data = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(data);
        }
    }
}
