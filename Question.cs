using System;
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
    public  class Question
    {
        public int idquestion { get; set; } = 0; //идентификатор
        public string text { get; set; } // текст вопроса
        public byte[] audio_file { get; set; } = null; // файл аудио
        public Image picture { get; set; } // картинка
        public string video_ref { get; set; } // ссылка на видео
        public int level { get; set; } = 1;// уровень вопроса
        public string theme { get; set; } // тема вопроса
        public User author { get; set; } // автор вопроса
        public int? correct_answer_id { get; set; }// идентификатор правильного ответа
        public List<Answer> Answers { get; set; }// набор ответов

        public override string ToString()
        {
            return theme + " - " + level + " ур.";
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
    }
}
