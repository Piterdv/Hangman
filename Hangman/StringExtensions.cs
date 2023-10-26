using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman
{
    public static class StringExtensions
    {
        public static string EvalOrOddCharToUpper(this string value)
        {
            if(value == null) return string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                if(value[0].ToString().ToUpper() != value[0].ToString())
                    if (i % 2 == 0) sb.Append(value[i].ToString().ToUpper()); else sb.Append(value[i].ToString().ToLower());
                else
                    if (i % 2 != 0) sb.Append(value[i].ToString().ToUpper()); else sb.Append(value[i].ToString().ToLower());
            }

            return sb.ToString();
        }

    }
}
