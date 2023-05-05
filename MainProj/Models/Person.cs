using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProj.Models
{
    /// <summary>
    /// Сущность БД
    /// </summary>
    internal class Person
    {
        /// <summary>
        /// Имя.
        /// </summary>
        public string FirstName { get; set; } = "None";
        /// <summary>
        /// Отчество.
        /// </summary>
        public string MiddleName { get; set; } = "None";
        /// <summary>
        /// Фамилия.
        /// </summary>
        public string LastName { get; set; } = "None";
        /// <summary>
        /// Дата рождения (только дата).
        /// </summary>
        public DateOnly Bithday { get; set; }
        /// <summary>
        /// Пол.
        /// </summary>
        public string Gender { get; set; } = "None";

    }
}
