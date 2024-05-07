using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    public class WorkPlace : Zone
    {
        public event EventHandler? WorkplaceUpdate;
        public WorkPlace(int x, int y, FieldType type) : base(x, y, type)
        {
        }

        /// <summary>
        /// Build connection with employee
        /// </summary>
        /// <param name="employee">caller person</param>
        public void Hire(Person employee)
        {
            this.people.Add(employee);
            if (GetOccupancy > 0) destroyable = false;
            else destroyable = true;
            WorkplaceUpdate?.Invoke(this, EventArgs.Empty);

        }

        /// <summary>
        /// Sever connection with employee
        /// </summary>
        /// <param name="employee">caller person</param>
        public void Quit(Person employee)
        {
            this.people.Remove(employee);
            if (GetOccupancy > 0) destroyable = false;
            else destroyable = true;
        }


        public void SafetyHappinessUpdate()
        {
            foreach (Person person in people)
            {
                person.WorkSafetyChange(safety, GetOccupancy);
            }
        }

        /// <summary>
        /// What happens when catastrophe strucks
        /// </summary>
        public new void Collapse()
        {
            destroyable = true;
            demolished = true;
            foreach (Person person in people)
            {
                person.Fire();
            }
            people = new List<Person>();
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
                sb.Append("This workplace is demolished");
            }
            else
            {
                sb.Append("Number of workers: " + people.Count + "/" + GetCapacity() + "\n");
                if (people.Count != 0)
                {
                    sb.Append("Happiness: " + AllHappiness() / 5 + "%\n");
                }
                sb.Append("Current level: " + GetCurrentLevel + "\n");
            }            
            return sb.ToString();
        }
    }
}
