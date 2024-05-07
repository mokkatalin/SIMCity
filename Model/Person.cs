using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Citizen
    /// </summary>
    public class Person
    {
        #region Private Data

        private static int num = 0;
        private int id;
        private int age;

        //0,1,2 - no qualification, school diploma, university diploma
        private int qualification;

        private int paidtax; //sum of paid taxes (between ages 45-65)
        private int unhappy; //number of consecutive unhappy years

        private Educational? school;
        private WorkPlace? work;
        private ResidentialZone residence;

        private int happiness; //overall 0-100
        private int th; //tax hapiness 0-100
        private int sh; //safety hapiness 0-100
        private int ws; //work safety 0-100
        private int wd; //work distance 0-100
        private int ci; //close industrial 0-100
        private int bonus; //bonus from stadium
        private int forestBonus; //bonus from forest

        #endregion

        #region Constructor

        public Person(int age, ResidentialZone residence)
        {
            id = ++num;
            qualification = 0;
            happiness = 400;
            paidtax = 0;
            th = 100;
            sh = 100;
            ws = 100;
            ci = 100;
            bonus = 0;
            forestBonus = 0;
            this.age = age;
            this.residence = residence;
            school = null;
        }

        #endregion

        #region Properties, Getters

        public int GetId { get { return id; } }

        public int GetAge { get { return age; } }

        public int GetQualification { get { return qualification; } }

        public int GetHappiness(){
            if (happiness <= 500) return happiness;
            else return 500;
        }

        public int GetUnhappy { get { return unhappy; } }

        public int GetSh { get { return sh; } }

        public int GetWs { get { return ws; } }

        public int GetCi { get { return ci; } }

        public int GetWdh { get { return wd; } }

        public ResidentialZone GetResidence { get { return residence; } }

        #endregion

        #region Modifiers

        public void IncUnHappy()
        {
            unhappy++;
        }

        public void UnHappyDefault()
        {
            unhappy = 0;
        }

        public void IncreaseAge()
        {
            age++;
            if (age == 65) Retire();
        }

        #endregion

        #region Happiness Modifiers

        public void HappinessAdd(int change)
        {
            happiness += change;
        }

        public void TaxChange(int h)
        {
            HappinessAdd(-th);
            th = h;
            HappinessAdd(th);
        }

        public void HouseSafetyChange(int safety, int occupancy)
        {
            HappinessAdd(-sh);
            switch (occupancy)
            {
                case int i when i < 10:
                    sh = 100;
                    break;
                case int i when i >= 10 && i < 35:
                    sh = (safety * 100) / 9;
                    break;
                case int i when i >= 35 && i < 70:
                    sh = (safety * 100) / 18;
                    break;
                case int i when i >= 70 && i <= 100:
                    sh = (safety * 100) / 27;
                    break;
            }
            if (sh > 100) sh = 100;
            HappinessAdd(sh);
        }

        public void WorkSafetyChange(int safety, int occupancy)
        {
            HappinessAdd(-ws);
            switch (occupancy)
            {
                case int i when i < 10:
                    ws = 100;
                    break;
                case int i when i >= 10 && i < 35:
                    ws = (safety * 100) / 9;
                    break;
                case int i when i >= 35 && i < 70:
                    ws = (safety * 100) / 18;
                    break;
                case int i when i >= 70 && i <= 100:
                    ws = (safety * 100) / 27;
                    break;
            }
            if (ws > 100) ws = 100;
            HappinessAdd(ws);
        }

        public void SmogChange(int closeIndustrial)
        {
            HappinessAdd(-ci);
            ci = 100 - ((closeIndustrial * 100) / 8);
            if (ci < 0) ci = 0;
            HappinessAdd(ci);
        }

        public void BonusChange()
        {
            int wbonus;
            if (work == null) wbonus = 0;
            else wbonus = work.GetBonus;
            if ((wbonus + residence.GetBonus) != bonus)
            {
                HappinessAdd(bonus * (-5));
                bonus = wbonus + residence.GetBonus;
                HappinessAdd(bonus * 5);
            }
        }

        public void ForestBonusChange()
        {
            if (residence.GetForestBonus != forestBonus)
            {
                HappinessAdd(forestBonus * (-5));
                forestBonus = residence.GetForestBonus;
                HappinessAdd(forestBonus * 5);
            }
        }

        #endregion

        #region Private Methods

        private void Retire()
        {
            paidtax = paidtax / 40; //calculate pension
            Quit();

            HappinessAdd(-wd);
            wd = 100;
            HappinessAdd(wd);

            HappinessAdd(-ws);
            ws = 80;
            HappinessAdd(ws);
        }

        /// <summary>
        /// Calculates happiness from distance
        /// </summary>
        private int HappinessFromDistance(int distance)
        {
            int workhappiness = 0;
            switch (distance)
            {
                case int i when i <= 5:
                    workhappiness = 100;
                    break;
                case int i when i > 5 && i <= 10:
                    workhappiness = 85;
                    break;
                case int i when i > 10 && i <= 15:
                    workhappiness = 70;
                    break;
                case int i when i > 15 && i <= 25:
                    workhappiness = 50;
                    break;
                case int i when i > 25:
                    workhappiness = 10;
                    break;
            }
            return workhappiness;
        }

        #endregion

        #region Public Methods

        public Boolean IsWorking() { return work != null; }


        /// <summary>
        /// Calculates the amount of tax money per person
        /// </summary>
        /// <param name="tax"> Scale of taxing </param>
        /// <returns> How much tax they pay or the negative value of pension </returns>
        public int Taxing(int tax)
        {
            if (age >= 65) //they get pension
            {
                return -1 * paidtax;
            }

            int mult = 0;
            if (work != null)
            {
                switch (qualification)
                {
                    case 0:
                        mult = 2;
                        break;
                    case 1:
                        mult = 4;
                        break;
                    case 2:
                        mult = 6;
                        break;
                }
            }
            else
            {
                switch (qualification)
                {
                    case 0:
                        mult = 1;
                        break;
                    case 1:
                        mult = 2;
                        break;
                    case 2:
                        mult = 3;
                        break;
                }
            }
            if (age >= 45) paidtax += (tax * mult);
            return (tax * mult);
        }

        #endregion

        #region Sever connections

        /// <summary>
        /// Sever connection from Person
        /// </summary>
        public void Quit()
        {
            if (work != null) work.Quit(this);
            work = null;
            HappinessAdd(-wd);
            wd = 0;
            HappinessAdd(-ws);
            ws = 0;
        }

        /// <summary>
        /// Sever connection from Workplace
        /// </summary>
        public void Fire()
        {
            work = null;
            HappinessAdd(-wd);
            wd = 0;
            HappinessAdd(-ws);
            ws = 0;
        }

        /// <summary>
        /// Sever connection from Person 
        /// </summary>
        public void DropOut()
        {
            if (school != null)
            {
                school.DropOut(this);
                school = null;
            }
        }

        /// <summary>
        /// Person dies in catastrophe, severs connections
        /// </summary>
        public void Accident()
        {
            Quit();
            DropOut();
        }


        /// <summary>
        /// Sever connection from Educational
        /// </summary>
        public void SchooCollapse()
        {
            school = null;
        }

        /// <summary>
        /// Graduation (called) from Educational, sever connection
        /// </summary>
        /// <param name="sc"> Calling Educational </param>
        /// <returns> Is graduation successfull </returns>
        public bool Graduate(Educational sc)
        {
            if (sc == this.school)
            {
                qualification++;
                school = null;
                return true;
            }
            return false;
        }

        #endregion

        #region Build Connections

        /// <summary>
        /// Enrollment (called) from ResidentialZone, build connection with Educational
        /// </summary>
        /// <param name="edu"> Educational to enroll in </param>
        /// <returns> Is enrollment successfull </returns>
        public bool Enroll(Educational edu)
        {
            if (school == null && age <= 61)
            {
                if (edu is School && qualification == 0)
                {
                    school = edu;
                    edu.Accept(this);
                    return true;
                }
                if (edu is University && qualification == 1)
                {
                    school = edu;
                    edu.Accept(this);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets workplace, sets distance happiness
        /// </summary>
        /// <param name="workplace"> Workplace to connect </param>
        /// <param name="distance"> Distance for happiness </param>
        /// <returns> Was getting a job successfull </returns>
        public bool GetJob(WorkPlace? workplace, int distance)
        {
            if (workplace == null) return false;

            if (workplace.HasSpot() && this.work == null)
            {
                this.work = workplace;
                workplace.Hire(this);

                WorkSafetyChange(workplace.GetSafety, workplace.GetOccupancy);

                wd = HappinessFromDistance(distance);
                HappinessAdd(wd);

                return true;
            }
            return false;
        }

        #endregion

    }
}
