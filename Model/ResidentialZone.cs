using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CityBuilder.Model
{
    /// <summary>
    /// Residential Zone, people live here
    /// </summary>
    public class ResidentialZone : Zone
    {
        #region Private Data
        //bonus, minus for happiness
        private int closeIndustrial;
        private int forestBonus;

        //institutions, distance
        private IDictionary<Educational, int> connectedSchools;
        private IDictionary<WorkPlace, int> connectedWorkplaces;

        #endregion

        #region Constructor
        public ResidentialZone(int x, int y) : base(x, y, FieldType.ResidentialZone)
        {
            connectedSchools = new Dictionary<Educational, int>();
            connectedWorkplaces = new Dictionary<WorkPlace, int>();
            closeIndustrial = 0;
            forestBonus = 0;
        }

        #endregion

        #region Getters, Properties

        public int GetForestBonus { get { return forestBonus; } }

        #endregion

        #region Modifiers

        public void SmogRise()
        {
            closeIndustrial++;
        }

        public void SmogDrop()
        {
            closeIndustrial--;
        }
        public void AddForestBonus()
        {
            forestBonus++;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// People get work in commertial zones for balance
        /// </summary>
        /// <param name="commertial">Number of commertial workers </param>
        /// <param name="industrial">Number of industrial workers</param>
        /// <param name="movein">Limit of new persons</param>
        /// <returns>Number of new people</returns>
        private int FillCommertial(int commertial, int industrial, int movein)
        {
            int i = 0;
            Random r = new Random(DateTime.Now.Millisecond);
            Person tempp;
            bool isplace = true;
            while (isplace && commertial < industrial && HasSpot() && i < movein)
            {
                tempp = new Person(r.Next(18, 60), this);
                (WorkPlace? work, int distance) = SearchJobCom();
                if (tempp.GetJob(work, distance))
                {
                    commertial++;
                    people.Add(tempp);
                    i++;
                }
                else isplace = false;
            }
            return i;
        }

        /// <summary>
        /// People get work in industrial zones for balance
        /// </summary>
        /// <param name="commertial">Number of commertial workers </param>
        /// <param name="industrial">Number of industrial workers</param>
        /// <param name="movein">Limit of new persons</param>
        /// <returns>Number of new people</returns>
        private int FillIndustrial(int commertial, int industrial, int movein)
        {
            int i = 0;
            Random r = new Random(DateTime.Now.Millisecond);
            Person tempp;
            bool isplace = true;
            while (isplace && industrial < commertial && HasSpot() && i < movein)
            {
                tempp = new Person(r.Next(18, 60), this);
                (WorkPlace? work, int distance) = SearchJobInd();
                if (tempp.GetJob(work, distance))
                {
                    industrial++;
                    people.Add(tempp);
                    i++;
                }
                else isplace = false;
            }
            return i;
        }

        /// <summary>
        /// Calculates new people limit
        /// </summary>
        /// <param name="beginning">Is the beginning of the game</param>
        /// <returns>Limit</returns>
        private int MoveInRate(int cityhappiness, bool beginning)
        {
            int movein;
            if (beginning) movein = 4;
            else
            {
                int k = cityhappiness + (100 * (15 - ClosestWork()) / 15)
                    + 100 * (9 - closeIndustrial) / 9 + forestBonus * 20;
                movein = k / 75;
            }
            return movein;
        }

        /// <summary>
        /// New people come to the city, they move in the residential zone
        /// </summary>
        /// <param name="beginning">Is the beginning of the game</param>
        /// <returns></returns>
        private int MoveIn(int cityhappiness, bool beginning)
        {
            int commertial = 0;
            int industrial = 0;
            int movein = MoveInRate(cityhappiness, beginning);

            foreach (var data in connectedWorkplaces)
            {
                if (data.Key is IndustrialZone ind)
                    industrial += ind.GetOccupancy;
                if (data.Key is CommertialZone comm)
                    commertial += comm.GetOccupancy;
            }

            int i = 0;
            i = FillCommertial(commertial, industrial, movein);
            i = FillIndustrial(commertial, industrial, movein);

            Person tempp;
            Random r = new Random(DateTime.Now.Millisecond);
            bool isind = true;
            bool iscom = true;
            while (HasSpot() && (isind || iscom))
            {
                if (iscom && SearchJobCom() != (null, 0) && i < movein)
                {
                    tempp = new Person(r.Next(18, 60), this);
                    (WorkPlace? work, int distance) = SearchJobCom();
                    tempp.GetJob(work, distance);
                    people.Add(tempp);
                    i++;
                }
                else iscom = false;
                if (isind && SearchJobInd() != (null, 0) && i < movein)
                {
                    tempp = new Person(r.Next(18, 60), this);
                    (WorkPlace? work, int distance) = SearchJobInd();
                    tempp.GetJob(work, distance);
                    people.Add(tempp);
                    i++;
                }
                else isind = false;
            }
            return i;
        }

        /// <summary>
        /// Search for the closest avalaible school
        /// </summary>
        /// <returns>the school object</returns>
        private School? SearchForSchool()
        {
            (School?, int) temp = (null, 1000);
            foreach (var con in connectedSchools)
            {
                if (con.Key is School cons)
                {
                    if (con.Value < temp.Item2 && cons.HasSpot())
                    {
                        temp.Item1 = cons;
                        temp.Item2 = con.Value;
                    }
                }
            }
            return temp.Item1;
        }

        /// <summary>
        /// Search for the closest avalaible university
        /// </summary>
        /// <returns>the university object</returns>
        private University? SearchForUniversity()
        {
            (University?, int) temp = (null, 1000);
            foreach (var con in connectedSchools)
            {
                if (con.Key is University conu)
                {
                    if (con.Value < temp.Item2 && conu.HasSpot())
                    {
                        temp.Item1 = conu;
                        temp.Item2 = con.Value;
                    }
                }
            }
            return temp.Item1;
        }

        /// <summary>
        /// Search for the closest work avalaible
        /// </summary>
        /// <returns></returns>
        private int ClosestWork()
        {
            int minim = 15;
            foreach (var workpair in connectedWorkplaces)
            {
                if (workpair.Key.HasSpot() && workpair.Value < minim) //does it need hasspot
                {
                    minim = workpair.Value;
                }
            }
            return minim;
        }

        /// <summary>
        /// Search for job in commertial zone
        /// </summary>
        /// <returns>commertial zone and distance</returns>
        private (WorkPlace? work, int distance) SearchJobCom()
        {
            foreach (var connect in connectedWorkplaces)
            {
                if (connect.Key.HasSpot() &&
                    connect.Key is CommertialZone comm)
                    return (connect.Key, connect.Value);
            }
            return (null, 0);
        }

        /// <summary>
        /// Search for job in industrial zone
        /// </summary>
        /// <returns>industrial zone and distance</returns>
        private (WorkPlace? work, int distance) SearchJobInd()
        {
            foreach (var connect in connectedWorkplaces)
            {
                if (connect.Key.HasSpot() &&
                    connect.Key is IndustrialZone)
                    return (connect.Key, connect.Value);
            }
            return (null, 0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the tax from people
        /// </summary>
        /// <param name="tax">scale of tax</param>
        /// <returns>sum of tax from people</returns>
        public int TaxEveryone(int tax)
        {
            int alltax = 0;
            foreach (Person resident in people)
            {
                alltax += resident.Taxing(tax);
            }
            return alltax;
        }

        /// <summary>
        /// What happens when struck by catastrophe
        /// </summary>
        public new void Collapse()
        {
            destroyable = true;
            demolished = true;

            foreach (Person person in people)
            {
                person.Accident();
            }

            connectedSchools.Clear();
            connectedWorkplaces.Clear();
            people.Clear();
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
                sb.Append("This residential zone is demolished.\n");
            }
            else
            {
                sb.Append("Population: " + people.Count + "/" + GetCapacity() + "\n");
                if (people.Count != 0)
                {
                    sb.Append("Happiness: " + AllHappiness() / 5 + "%\n");
                }
                sb.Append("Current level: " + GetCurrentLevel + "\n");
            }         
            return sb.ToString();
        }

        #endregion

        #region Modifiers for residents

        public void IncreaseAge()
        {
            foreach (Person resident in people)
            {
                resident.IncreaseAge();
            }
        }

        #endregion

        #region Changes in people

        /// <summary>
        /// Moving people out due to long unhappiness
        /// </summary>
        /// <returns>How many people moved out</returns>
        public int MoveOut()
        {
            int mout = 0;
            for (int i = people.Count - 1; i >= 0; i--)
            {
                if (people[i].GetHappiness() < 100) people[i].IncUnHappy();
                else people[i].UnHappyDefault();

                if (people[i].GetUnhappy > 150 && people[i].GetAge < 65)
                {
                    if (people[i].IsWorking()) people[i].Quit();
                    people[i].DropOut();
                    people.RemoveAt(i);
                    mout++;
                }
            }
            return mout;
        }

        /// <summary>
        /// Old people randomly die
        /// </summary>
        /// <returns></returns>
        public int InnerChange()
        {
            int inner = 0;
            for (int i = people.Count - 1; i >= 0; i--)
            {
                if (people[i].GetAge > 65)
                {
                    Random r = new Random(DateTime.Now.Millisecond);
                    int random = r.Next(1, (100 - people[i].GetAge));
                    bool moveout = random == 1 || random == 4;

                    if (moveout)
                    {
                        people.RemoveAt(i);
                        inner++;
                    }
                }
            }
            return inner;
        }

        /// <summary>
        /// One person borns into ResidentialZone (at 18)
        /// </summary>
        /// <returns>Is the birth successfull</returns>
        public bool InnerBorn()
        {
            if (HasSpot())
            {
                Person tempp = new Person(18, this);
                people.Add(tempp);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks the conditions for new people to move in, calls the move in method
        /// </summary>
        /// <param name="cityhappiness"></param>
        /// <param name="beginning"></param>
        /// <returns>Number of new people</returns>
        public int MoveToCity(int cityhappiness, bool beginning)
        {
            if (GetOccupancy > 0) destroyable = false;
            else destroyable = true;

            int populationchange = 0;
            if (HasSpot())
            {
                populationchange += MoveIn(cityhappiness, beginning);
            }

            return populationchange;
        }

        #endregion

        #region Connect People to institutions

        /// <summary>
        /// Jobless people get jobs, similar to MoveIn method
        /// </summary>
        /// <returns>How many did get jobs</returns>
        public int JoblessSeek()
        {
            List<Person> jobless = new List<Person>();
            foreach (Person person in people)
            {
                if (!person.IsWorking())
                {
                    jobless.Add(person);
                }
            }
            int industrial = 0;
            int commertial = 0;
            int i = 0;
            int n = jobless.Count;
            foreach (var data in connectedWorkplaces)
            {
                if (data.Key is IndustrialZone ind)
                    industrial += ind.GetOccupancy;
                if (data.Key is CommertialZone comm)
                    commertial += comm.GetOccupancy;
            }
            bool isplace = true;
            while (isplace && commertial < industrial && i < n)
            {
                (WorkPlace? work, int distance) = SearchJobCom();
                if (jobless[i].GetJob(work, distance))
                {
                    commertial++;
                    i++;
                }
                else isplace = false;
            }
            isplace = true;
            while (isplace && industrial < commertial && i < n)
            {
                (WorkPlace? work, int distance) = SearchJobInd();
                if (jobless[i].GetJob(work, distance))
                {
                    industrial++;
                    i++;
                }
                else isplace = false;
            }

            bool isind = true;
            bool iscom = true;
            while (isind || iscom)
            {
                if (iscom && SearchJobCom() != (null, 0) && i < n)
                {
                    (WorkPlace? work, int distance) = SearchJobCom();
                    jobless[i].GetJob(work, distance);
                    i++;
                }
                else iscom = false;
                if (isind && SearchJobInd() != (null, 0) && i < n)
                {
                    (WorkPlace? work, int distance) = SearchJobInd();
                    jobless[i].GetJob(work, distance);
                    i++;
                }
                else isind = false;
            }

            return i;
        }

        /// <summary>
        /// People go to school
        /// </summary>
        /// <param name="limit">people limit overall</param>
        /// <param name="enroll">people limit for this year</param>
        public void GoToSchool(int limit, int enroll)
        {
            int i = 0;
            while (enroll > 0 && limit > 0 && i < people.Count)
            {
                School? temp = SearchForSchool();
                if (temp != null)
                {
                    if (people[i].Enroll(temp))
                    {
                        limit--;
                        enroll--;
                    }
                }
                else i = people.Count;
                i++;
            }
        }

        /// <summary>
        /// People go to university
        /// </summary>
        /// <param name="limit">people limit overall</param>
        /// <param name="enroll">people limit for this year</param>
        public void GoToUniversity(int limit, int enroll)
        {
            int i = 0;
            while (enroll > 0 && limit > 0 && i < people.Count)
            {
                University? temp = SearchForUniversity();
                if (temp != null)
                {
                    if (people[i].Enroll(temp))
                    {
                        limit--;
                        enroll--;
                    }
                }
                else i = people.Count;
                i++;
            }
        }

        #endregion

        #region Build connections

        /// <summary>
        /// Connects institution to this zone
        /// </summary>
        /// <param name="building">institution</param>
        public void ConnectTo(FieldData building, int distance)
        {
            if (building is WorkPlace work)
            {
                if (connectedWorkplaces.ContainsKey(work))
                {
                    if (distance < connectedWorkplaces[work])
                    {
                        connectedWorkplaces.Remove(work);
                        connectedWorkplaces.Add(work, distance);
                    }
                }
                else
                {
                    connectedWorkplaces.Add(work, distance);
                }

            }
            else if (building is Educational school)
            {
                if (connectedSchools.ContainsKey(school))
                {
                    if (distance < connectedSchools[school])
                    {
                        connectedSchools.Remove(school);
                        connectedSchools.Add(school, distance);
                    }
                }
                else
                {
                    connectedSchools.Add(school, distance);
                }
            }
        }

        #endregion

        #region Sever connections

        public void CutTiesWith(FieldData inst)
        {
            if (inst is WorkPlace work)
            {
                if (connectedWorkplaces.ContainsKey(work))
                {
                    connectedWorkplaces.Remove(work);
                }
            }
            if (inst is Educational edu)
            {
                if (connectedSchools.ContainsKey(edu))
                {
                    connectedSchools.Remove(edu);
                }
            }
        }

        #endregion

        #region Happiness Updates

        public void TaxHappinessForPeople(int h)
        {
            foreach (Person person in people)
            {
                person.TaxChange(h);
            }
        }

        public void SafetyHappinessUpdate()
        {
            foreach (Person person in people)
            {
                person.HouseSafetyChange(safety, GetOccupancy);
            }
        }

        public void SmogHappinessUpdate()
        {
            foreach (Person person in people)
            {
                person.SmogChange(closeIndustrial);
            }
        }

        public void MinusHappinessUpdate(int change)
        {
            foreach (Person person in people)
            {
                person.HappinessAdd(change);
            }
        }

        public void ForestBonusHappinessUpdate()
        {
            foreach (Person person in people)
            {
                person.ForestBonusChange();
            }
        }

        #endregion

    }
}
