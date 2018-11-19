using ClassicComedians.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassicComedians.Data
{
    /// <summary>
    ///  This class represents a Fake database.
    ///  It holds data in memory, therefore the data will be reset each time the application is restarted.
    ///  
    ///  In a real application data would be stored in a real database (e.g. Sql Server)
    /// </summary>
    public static class Database
    {
        private static List<Group> _groups = new List<Group>
        {
            new Group { Id = 1, Name = "None (solo comedian)" },
            new Group { Id = 2, Name = "The Marx Brothers" },
            new Group { Id = 3, Name = "Burns and Allen" },
            new Group { Id = 4, Name = "The Three Stooges" },
            new Group { Id = 5, Name = "Laurel and Hardy" },
        };

        private static List<Comedian> _comedians = new List<Comedian>
        {
            new Comedian
            {
                Id = 1,
                FirstName = "Groucho",
                LastName = "Marx",
                BirthDate = new DateTime(1890, 10, 2),
                DeathDate = new DateTime(1977, 8, 19),
                GroupId = 2,
            },
            new Comedian
            {
                Id = 2,
                FirstName = "Chico",
                LastName = "Marx",
                BirthDate = new DateTime(1887, 3, 22),
                DeathDate = new DateTime(1961, 10, 11),
                GroupId = 2,
            },
            new Comedian
            {
                Id = 3,
                FirstName = "Harpo",
                LastName = "Marx",
                BirthDate = new DateTime(1888, 11, 23),
                DeathDate = new DateTime(1964, 9, 28),
                GroupId = 2,
            },
            new Comedian
            {
                Id = 4,
                FirstName = "Lucy",
                LastName = "Ball",
                BirthDate = new DateTime(1911, 8, 6),
                DeathDate = new DateTime(1989, 4, 26),
                GroupId = 1,
            },
            new Comedian
            {
                Id = 5,
                FirstName = "Gracie",
                LastName = "Allen",
                BirthDate = new DateTime(1895, 7, 26),
                DeathDate = new DateTime(1964, 8, 27),
                GroupId = 3,
            },
            new Comedian
            {
                Id = 6,
                FirstName = "George",
                LastName = "Burns",
                BirthDate = new DateTime(1896, 1, 20),
                DeathDate = new DateTime(1996, 3, 9),
                GroupId = 3,
            },
        };

        public static Comedian GetComedianById(int id)
        {
            return _comedians.FirstOrDefault(c => c.Id == id);
        }

        public static IEnumerable<Comedian> GetAllComedians()
        {
            return new List<Comedian>(_comedians);
        }

        public static void AddComedian(Comedian comedian)
        {
            var nextId = _comedians.Max(c => c.Id) + 1;
            comedian.Id = nextId;
            _comedians.Add(comedian);
        }

        public static void UpdateComedian(Comedian comedian)
        {
            var existing = GetComedianById(comedian.Id);
            existing.FirstName = comedian.FirstName;
            existing.LastName = comedian.LastName;
            existing.BirthDate = comedian.BirthDate;
            existing.DeathDate = comedian.DeathDate;
        }

        public static void DeleteComedian(int id)
        {
            _comedians = _comedians.Where(c => c.Id != id).ToList();
        }

        public static IEnumerable<Group> GetAllGroups()
        {
            return new List<Group>(_groups);
        }
    }
}
