using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five_testing
{
    /// <summary>
    /// Класс ответа на отдельный вопрос теста
    /// </summary>
    public class Answer
    {
        public int Idanswers { get; set; } // идентификатор 
        public string Text { get; set; } // текст ответа
        public int Question_id { get; set; } // идентификатор вопроса, к которому он принадлежит
    }
}
