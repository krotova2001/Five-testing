using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five_testing
{
    /// <summary>
    /// тема вопросов
    /// </summary>
    public class Thema
    {
        public string Theme { get; set; }
        public int? idtheme { get; set; } = 0;
        public override string ToString()
        {
            return Theme;
        }
    }
}
