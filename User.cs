using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Five_testing
{
    /// <summary>
    /// Пользователь программы
    /// </summary>
    public class User
    {
        public int Id { get; set; } // номер, соответствует id в БД
        public string username { get; set; } // имя
        public string password { get; set; } // имя
        public string email { get; set; } // имя
        public string name { get; set; } // имя
        public string surname { get; set; } // фамилия
        public bool is_prepod { get; set; }
        public bool is_student { get; set; }
        public bool is_admin { get; set; }
        public DateTime create_time { get; set; }
        public int? age { get; set; } // возраст
        public string group_id { get; set; } // группа обучения

        public override string ToString()
        {
            return name + " " + surname;
        }
    }
}
