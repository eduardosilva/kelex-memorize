using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StringExtensions
{
    public static string BreakInLines(this string source)
    {
        return String.Join(Environment.NewLine, source.Split(new[] { @"\n" }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(r => r.Trim()));
    }
}