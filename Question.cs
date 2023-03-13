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
        public int idquestion { get; set; } //идентификатор
        public string text { get; set; } // текст вопроса
        public byte[] audio_file { get; set; } // файл аудио
        public Image picture { get; set; } // картинка
        public string video_ref { get; set; } // ссылка на видео
        public int level { get; set; } // уровень вопроса
        public string theme { get; set; } // тема вопроса
        public User author { get; set; } // автор
        public int? correct_answer_id { get; set; }// идентификатор правильного ответа

        public override string ToString()
        {
            return theme + " - " + level + " ур.";
        }

    }
}
