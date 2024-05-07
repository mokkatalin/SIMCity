using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CityBuilder.Model
{
    public class Zone : FieldData
    {
        private int currentLevel;

        protected int safety;

        private int bonus;

        protected List<Person> people;

        public int GetCapacity()
        {
            switch (currentLevel)
            {
                case 1: return 20;
                case 2: return 50;
                case 3: return 100;
                default: return 0;
            }
        }
        public int GetCurrentLevel { get { return currentLevel; } }
        public int GetOccupancy { get { return people.Count(); } }

        public int GetSafety { get { return safety; } }

        public int GetBonus { get { return bonus; } }

        public new Boolean IsDestroyable()
        {
            return GetOccupancy <= 0;
        }

        public bool HasSpot() { return (GetCapacity() - GetOccupancy) > 0; }

        public int AllHappiness()
        {
            double sum = 0;
            foreach (Person p in people)
            {
                sum += p.GetHappiness();
            }
            return (int) (sum / people.Count());
        }
        public Zone(int x, int y, FieldType type) : base(x,y,type)
        {
            switch (type)
            {
                case FieldType.Empty:
                case FieldType.Road:
                case FieldType.Police:
                case FieldType.Stadium:
                case FieldType.School:
                case FieldType.University:
                    throw new Exception();// ha nem jol peldanyosit
                default:
                    break;
            }
            currentLevel = 1;
            safety = 0;
            bonus = 0;
            people = new List<Person>();
        }

        public void Upgrade()
        {
            if (currentLevel < 3)
                currentLevel++;
        }

        public void SafetyRise()
        {
            safety++;
        }

        public void SafetyDrop()
        {
            safety--;
        }
       
        
        public void AddBonus()
        {
            bonus++;
        }

        public void RemoveBonus()
        {
            bonus--;
        }

        public void BonusHappinessUpdate()
        {
            foreach(Person person in people)
            {
                person.BonusChange();
            }
        }
        
    }
}
