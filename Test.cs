using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five_testing
{
    /// <summary>
    /// Класс отдельного теста для прохождения учеником
    /// </summary>
    public class Test:IEnumerable<Question>
    {
        public int idtest { get; set; } //идентификатор
        public string name { get; set; } // название
        public string info { get; set; } // описание
        public int author_id { get; set; } //идентификатор автора
        public DateTime date { get; set; } // дата создания, надо поменять потом на дату редактирования
        public string text { get; set; } // приветственная информация
        public List<Question> questions { get; set; } // список вопросов
        User author { get; set; } // автор теста

        public override string ToString()
        {
            return name;
        }

        public Test()
        {
            questions = new List<Question>();
        }

        public IEnumerator<Question> GetEnumerator()
        {
            return ((IEnumerable<Question>)questions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)questions).GetEnumerator();
        }
    }
}
