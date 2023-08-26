using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five_testing
{
    /// <summary>
    /// вопрос в тесте, существует внутри теста
    /// </summary>
    public  class Question : IEnumerable<Answer>
    {
        public int idquestion { get; set; } = 0; //идентификатор
        public string text { get; set; } // текст вопроса
        public byte[] audio_file { get; set; } = null; // файл аудио
        public Image picture { get; set; } // картинка
        public string video_ref { get; set; } // ссылка на видео
        public int level { get; set; } = 1;// уровень вопроса
        public Thema theme { get; set; } // тема вопроса
        public int id_question_theme { get; set; } = 0;
        public User author { get; set; } // автор вопроса
        public int author_id { get; set; } = 0;
        public int? correct_answer_id { get; set; } = 0;// идентификатор правильного ответа
        public List<Answer> Answers { get; set; }// набор ответов

        public Question ()
        {
            Answers = new List<Answer> ();
        }

        public override string ToString()
        {
            return this.theme + " - " + level + " ур.";
        }

        /// <summary>
        /// дать правильный ответ
        /// </summary>
        /// <returns></returns>
        public Answer Get_correct_ans()
        {
            foreach (Answer answer in Answers)
            {
                if (correct_answer_id.HasValue && correct_answer_id==answer.Idanswers)
                    return answer;
            }
            return null;
        }

        public IEnumerator<Answer> GetEnumerator()
        {
            return ((IEnumerable<Answer>)Answers).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Answers).GetEnumerator();
        }
    }
}
