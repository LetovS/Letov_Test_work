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
        public int PersonID { get; set; }
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
        public DateTime Bithday { get; set; }
        /// <summary>
        /// Пол.
        /// </summary>
        public string Gender { get; set; } = "None";

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Person person)
            {
                if (this.LastName == person.LastName && this.FirstName == person.FirstName &&
                    this.MiddleName == person.MiddleName && this.Bithday == person.Bithday)
                {
                    return true;
                }
                else
                    return false;
            }
            throw new InvalidCastException();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
