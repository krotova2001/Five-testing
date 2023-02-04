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
        public string Name { get; set; } // имя
        public string Surname { get; set; } // фамилия
        public int Id { get; set; } // номер, соответствует id в БД
        public int? Age { get; set; } // возраст
        public bool IsAdmin { get; set; }
        public bool IsStuden { get; set; }
        public bool IsPrepod { get; set; }
        public string Group { get; set; } // группа обучения
    }
}
