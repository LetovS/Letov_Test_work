using MainProj.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MainProj.Infrastructure.Compapeps
{
    /// <summary>
    /// Для сравнения объектов в Distinct'a
    /// </summary>
    internal class PersonComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person? x, Person? y)
        {
            if (x != null && y != null)
            {
                if (x.LastName == y.LastName && x.FirstName == y.FirstName &&
                    x.MiddleName == y.MiddleName && x.Bithday == y.Bithday)
                {
                    return true;
                }
                else
                    return false;
            }
            throw new InvalidCastException();
        }

        public int GetHashCode([DisallowNull] Person obj)
        {
            return base.GetHashCode();
        }
    }
}