using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    public class Educational : FieldData
    {
        private int capacity;
        protected IDictionary<Person, int> people;


        public int GetCapacity { get { return capacity; } }

        public int GetOccupancy { get { return people.Count; } }

        public Educational(int x, int y, FieldType type) : base(x,y,type) {
            switch (type)
            {
                case FieldType.School:
                    this.capacity = 500;
                    break;
                case FieldType.University:
                    this.capacity = 1000;
                    break;
                default:
                    throw new Exception(); 
            }
            people = new Dictionary<Person, int>();
        }

        public new Boolean IsDestroyable()
        {
            return people.Count <= 0;
        }

        public bool HasSpot() { return people.Count < capacity; }

        /// <summary>
        /// Add person to people
        /// </summary>
        /// <param name="person"></param>
        public void Accept(Person person)
        {
            if (!people.ContainsKey(person) && HasSpot())
            {
                people.Add(person, 0);
            }
        }

        /// <summary>
        /// Builds data into string
        /// </summary>
        /// <returns>information to display</returns>
        public override String GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            if (demolished)
            {
                sb.Append("This edu.building is demolished\n");
            }
            else
            {
                sb.Append("Current number of students: " + GetOccupancy + "\n");
                sb.Append("Max number of students: " + capacity + "\n");
            }      
            return sb.ToString();
        }

        /// <summary>
        /// Graduate (remove) person or just add one year to their attendance years
        /// </summary>
        /// <returns></returns>
        public int Graduation()
        {
            int ret = 0;
            List<Person> keys = new List<Person>();
            foreach(var person in people)
            {
                if(person.Value == 2)
                {
                    keys.Add(person.Key);
                }
                else
                {
                    int s = person.Value;
                    s++;
                    people[person.Key] = s;
                }
            }
            foreach(Person person in keys)
            {
                people.Remove(person);
                if (person.Graduate(this)) ret++;
            }
            return ret;
        }

        /// <summary>
        /// What happens when catastrophe strucks
        /// </summary>
        public new void Collapse()
        {
            destroyable = true;
            demolished = true;
            foreach (var student in people)
            {
                student.Key.SchooCollapse();
            }
            people = new Dictionary<Person, int>();
        }

        /// <summary>
        /// What happens when a student dies or moves out
        /// </summary>
        /// <param name="person"></param>
        public void DropOut(Person person)
        {
            people.Remove(person);
        }
    }
}
