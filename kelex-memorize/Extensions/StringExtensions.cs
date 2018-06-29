using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class StringExtensions
{
    public static string BreakInLines(this string source)
    {
        return Regex.Unescape(source);
    }
}