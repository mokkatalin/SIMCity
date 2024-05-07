using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace CityBuilder.Model
{
    public class CityModel
    {
        #region Private Data

        private string cityName;

        private int balance;
        private int happiness;
        private int population;

        private int tax;

        private int time;

        //are we at the beginning of the game
        private bool beginning;

        private Field[,] map;
        private List<ResidentialZone> residences;
        private List<FieldData> institutions;
        private List<Forest> forests;

        private Queue<string> incomes;
        private Queue<string> expenses;

        private int schoolul; //school upper limit, it's a percentage
        private int universityul; //university upper limit, it's a percentage
        private int schooldiploma; //school diploma
        private int universitydiploma; //university diploma

        //number of roads and buildings for maintanence
        private int upkeepRoad;
        private int upkeepBuilding;

        //how many years in bankrupcy
        private int bankrupt;

        #endregion

        #region Constructor

        public CityModel()
        {
            cityName = "";
            map = new Field[100, 100];
            residences = new List<ResidentialZone>();
            institutions = new List<FieldData>();
            forests = new List<Forest>();
            incomes = new Queue<string>();
            expenses = new Queue<string>();
            forests = new List<Forest>();
        }

        #endregion

        #region Events

        public event EventHandler? GameAdvanced;

        public event EventHandler? GameOver;

        public event EventHandler? GameCreated;

        public event EventHandler? IncomeChanged;

        public event EventHandler? ExpenseChanged;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler? CatastropheChanged;

        #endregion

        #region Properties, Getters

        public string GetCityName { get { return cityName; } }

        public int GetBalance { get { return balance; } }

        public int GetHappiness { get { return happiness; } }

        public int GetPopulation { get { return population; } }

        public int GetTime { get { return time; } }

        public int GetTax { get { return tax; } }

        public FieldData GetField(int x, int y) { return map[x, y].GetData; }

        public int GetMapDimention { get { return map.GetLength(0); } }

        public Queue<String> GetIncomes { get { return incomes; } }

        public Queue<String> GetExpenses { get { return expenses; } }

        public int GetSchoolDiploma { get { return schooldiploma; } }
        
        public int GetUniversityDiploma { get { return universitydiploma; } }

        #endregion

        #region Setters

        public void SetTax(int value)
        {
            tax = value;
            TaxHappiness(tax);
            UpdateHappiness();
        }

        #endregion

        #region Private for TimeAdvanced

        /// <summary>
        /// Inner population changes
        /// </summary>
        private void DeathAndBirth()
        {
            int inner = 0;
            foreach (ResidentialZone house in residences)
            {
                if (time % 4 == 0)
                {
                    inner += house.InnerChange();
                }
            }
            while (inner > 0)
            {
                foreach (ResidentialZone house in residences)
                {
                    if (inner <= 0) break;
                    if (house.InnerBorn()) inner--;
                }
            }
        }

        /// <summary>
        /// Updates happiness for things that have affect yearly
        /// </summary>
        private void HappinessInYear()
        {
            int hchange = 0;
            if (balance < 0)
            {
                bankrupt++;
                hchange = bankrupt * 5 * (balance / 1000);
                happiness += hchange;

            }
            else
            {
                bankrupt = 0;
            }

            if (!IsBalance())
            {
                hchange += -30;
                happiness += hchange;
            }

            if (hchange != 0)
            {
                MinusHappinessUpdate(hchange);
            }

            foreach (Forest f in forests)
            {
                ForestBonusUpdate(f.GetX, f.GetY, f);
                BonusHappinessUpdate();
                UpdateHappiness();

                if (!f.IsGrown())
                {
                    f.Grow();
                }
            }
        }

        private void UpdateResidentialView(ResidentialZone house)
        {
            if (house.GetOccupancy <= 5)
            {
                OnFieldChanged(house.GetX, house.GetY, house.GetHeight(), house.GetWidth(), 1);
            }
            else if (house.GetOccupancy <= 10)
            {
                OnFieldChanged(house.GetX, house.GetY, house.GetHeight(), house.GetWidth(), 2);
            }
            else if (house.GetOccupancy <= 20)
            {
                OnFieldChanged(house.GetX, house.GetY, house.GetHeight(), house.GetWidth(), 3);
            }
        }

        /// <summary>
        /// Graduates people if they attended for 3 years, if not it increases the year
        /// </summary>
        /// <returns>number of people currently in school,
        /// number of people currently in university</returns>
        private (int, int) GraduateAllEducational()
        {
            int currents = 0;
            int currentu = 0;
            foreach (FieldData inst in institutions)
            {
                if (inst is Educational edu)
                {
                    if (edu is School)
                    {
                        schooldiploma += edu.Graduation();
                        currents += edu.GetOccupancy;
                    }
                    if (edu is University)
                    {
                        universitydiploma += edu.Graduation();
                        currentu += edu.GetOccupancy;
                    }
                }
            }
            return (currents, currentu);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initialization of data when starting game
        /// </summary>
        /// <param name="name">name of the city</param>
        public void CreateMap(string name)
        {
            cityName = name;
            balance = 25000;
            happiness = 500;
            population = 0;
            time = 0;
            bankrupt = 0;
            schoolul = 70;
            universityul = 55;
            upkeepRoad = 0;
            upkeepBuilding = 0;
            schooldiploma = 0;
            universitydiploma = 0;
            tax = 15;
            beginning = true;
            map = new Field[100, 100];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    FieldData tempobj = new FieldData(i, j, FieldType.Empty);
                    map[i, j] = new Field(tempobj);
                }
            }
            residences = new List<ResidentialZone>();
            institutions = new List<FieldData>();
            forests = new List<Forest>();
            incomes = new Queue<string>();
            expenses = new Queue<string>();
            InitForests();           

            OnGameCreated();
        }

        /// <summary>
        /// Each time a day passes it makes automatic updates
        /// </summary>
        public void TimeAdvanced()
        {
            if (happiness < 0) happiness = 0;
            time++;

            DeathAndBirth();

            int sumchange = 0;
            foreach (ResidentialZone house in residences)
            {
                sumchange += house.JoblessSeek();
            }

            //every two years
            if (time == 730)
            {
                time = 0;
                beginning = false;
            }

            //every two days
            if ((time % 2) == 0)
            {
                foreach (ResidentialZone house in residences)
                {
                    int change = 0;
                    change -= house.MoveOut();
                    change += house.MoveToCity(happiness, beginning && (time < 150));
                    if (change != 0)
                    {
                        UpdateResidentialView(house);
                        sumchange += change;
                        population += change;
                    }
                }
            }
            
            //every year
            if((time % 365) == 0)
            {
                int income = 0;
                foreach (ResidentialZone house in residences)
                {
                    //aging
                    house.IncreaseAge();
                    //taxing
                    income += house.TaxEveryone(tax);
                    //education
                    int currents, currentu;
                    (currents, currentu) = GraduateAllEducational();
                    int enrolls = (population * 2) / 10; //how many can enroll in one year (max)
                    int enrollu = population / 10;
                    if (enrolls > 0)
                    {
                        int scul = (population * schoolul) / 100;
                        house.GoToSchool(scul - schooldiploma - currents, enrolls);
                    }
                    if (enrollu > 0)
                    {
                        int unul = (population * universityul) / 100;
                        house.GoToUniversity(unul - universitydiploma - currentu, enrollu);
                    }
                }
                balance += income;
                incomes.Enqueue("Citizens' taxes: +" + income);
                OnIncomeChanged();

                //maintanance
                int spent = 0;
                spent += upkeepRoad * 50; //ertekek valtozhatnak
                spent += upkeepBuilding * 100;
                balance -= spent;
                if (spent != 0) expenses.Enqueue("Road and Building maintenance: -" + spent);
                OnExpenseChanged();

                int hchange = 0;
                if(balance < 0)
                {
                    bankrupt++;
                    hchange = bankrupt * 5 * (balance / 1000);
                    happiness += hchange;

                }
                else
                {
                    bankrupt = 0;
                }

                if (!IsBalance())
                {
                    hchange += -30;
                    happiness += hchange;
                }

                if(hchange != 0)
                {
                    MinusHappinessUpdate(hchange);
                }

                int forestMaintenance = 0;
                foreach (Forest f in forests)
                {
                    ForestBonusUpdate(f.GetX, f.GetY,f);
                    BonusHappinessUpdate();
                    UpdateHappiness();

                    
                    if (!f.IsGrown())
                    {
                        forestMaintenance++;
                    }
                }
                forestMaintenance *= 20;
                if (forestMaintenance > 0)
                {
                    balance -= forestMaintenance;
                    expenses.Enqueue("Forest maintenance: -" + forestMaintenance);
                }

                //happiness
                HappinessInYear();

            }

            //every 1000 days
            if (time % 1000 == 0)
            {
                Random r = new Random(DateTime.Now.Millisecond);
                Catastrophe(r.Next(100), r.Next(100));
            }

            if (sumchange != 0)
            {
                SmogHappinessUpdate();
                SafetyHappinessUpdate();
                BonusHappinessUpdate();
                UpdateHappiness();
            }

            OnGameAdvanced();
            if (happiness < 0) happiness = 0;
            if (happiness < 80) OnGameOver();
        }

        /// <summary>
        /// When we want to build to a spot in the map
        /// </summary>
        /// <param name="x">left up corner x</param>
        /// <param name="y">left up corner y</param>
        /// <param name="type">type of field to build</param>
        /// <exception cref="Exception">building exception</exception>
        public void Build(int x, int y, FieldType type)
        {
            FieldData tempobj;
            switch (type)
            {
                case FieldType.Road:
                    tempobj = new Road(x, y, type); 
                    break;
                case FieldType.ResidentialZone:
                    tempobj = new ResidentialZone(x, y);
                    break;
                case FieldType.IndustrialZone:
                    tempobj = new IndustrialZone(x, y);
                    break;
                case FieldType.CommercialZone:
                    tempobj = new CommertialZone(x, y);
                    break;
                case FieldType.University:
                    tempobj = new University(x, y);
                    break;
                case FieldType.School:
                    tempobj = new School(x, y);
                    break;
                case FieldType.Forest:
                    tempobj = new Forest(x, y,type);
                    break;
                default:
                    tempobj = new FieldData(x, y, type);
                    break;
            }
            int height = tempobj.GetHeight();
            int width = tempobj.GetWidth();
            if (isPlaceable(x, y, height, width) && (tempobj.GetType == FieldType.Road || isRoadNearby(x, y, height, width) || tempobj.GetType == FieldType.Forest) )
            {
                
                for (int i = x; i < x + height; i++)
                {
                    for (int j = y; j < y + width; j++)
                    {
                        map[i, j] = new Field(tempobj);
                        if (tempobj.GetType == FieldType.Road)
                        {
                            ((Road)tempobj).UpdateConnections(GetRoadConnections(x, y));
                        }                
                    }
                }
                int expense = GetField(x, y).GetExpense();
                balance -= expense;
                if (type != FieldType.Forest && type != FieldType.Road)
                {
                    expenses.Enqueue("Building constructed: -" + expense);
                    OnExpenseChanged();
                }
                OnFieldChanged(x, y, height, width, 0);
                switch (type)
                {
                    case FieldType.Road:
                        expenses.Enqueue("Road constructed: -" + expense);
                        OnExpenseChanged();
                        upkeepRoad++;
                        ConnectBuildings(ConnectRoads(x, y, true));
                        break;
                    case FieldType.Police:
                        SafetySpread(x,y,true);
                        SafetyHappinessUpdate();
                        UpdateHappiness();
                        upkeepBuilding++;
                        break;
                    case FieldType.Stadium:
                        upkeepBuilding++;
                        SpreadBonus(x,y,true);
                        BonusHappinessUpdate();
                        UpdateHappiness();
                        break;
                    case FieldType.School:
                    case FieldType.University:
                        upkeepBuilding++;
                        institutions.Add(tempobj);
                        ConnectServices(x, y);
                        break;
                    case FieldType.IndustrialZone:
                        upkeepBuilding++;
                        institutions.Add(tempobj);
                        SpreadSmog(x, y, true);
                        SmogHappinessUpdate();
                        SearchForSafety(x, y, height, width, (Zone)tempobj);
                        SafetyHappinessUpdate();
                        SearchForBonus(x,y,(Zone)tempobj);
                        BonusHappinessUpdate();
                        ConnectServices(x, y);
                        ((WorkPlace)tempobj).WorkplaceUpdate += new EventHandler(OnWorkplaceUpdate);
                        UpdateHappiness();
                        break;
                    case FieldType.CommercialZone:
                        upkeepBuilding++;
                        institutions.Add(tempobj);
                        SearchForSafety(x,y,height,width,(Zone)tempobj);
                        SafetyHappinessUpdate();
                        SearchForBonus(x, y, (Zone)tempobj);
                        BonusHappinessUpdate();
                        ConnectServices(x, y);
                        ((WorkPlace)tempobj).WorkplaceUpdate += new EventHandler(OnWorkplaceUpdate); 
                        UpdateHappiness();
                        break;
                    case FieldType.ResidentialZone:
                        residences.Add((ResidentialZone)tempobj);
                        SmogInhale(x,y,height,width,(ResidentialZone)tempobj);
                        SmogHappinessUpdate();
                        SearchForSafety(x, y,height,width,(Zone)tempobj);
                        SafetyHappinessUpdate();
                        SearchForBonus(x, y, (Zone)tempobj);
                        BonusHappinessUpdate();
                        ConnectResidents(x, y);
                        UpdateHappiness();
                        break;
                    case FieldType.Forest:
                        expenses.Enqueue("Tree planted: -" + expense);
                        OnExpenseChanged();
                        forests.Add((Forest)tempobj);
                        ForestBonusUpdate(x,y,(Forest)tempobj);
                        BonusHappinessUpdate();
                        UpdateHappiness();
                        break;
                    default:
                        break;
                }
            }
            else if(type != FieldType.Road && !isRoadNearby(x, y, height, width) && type != FieldType.Forest)
            {
                throw new Exception("You need to build near a road!");               
            }
            else
            {
                throw new Exception("You can not build to an occupied field!");
            }
        }

        /// <summary>
        /// When we want to unbuild something
        /// </summary>
        /// <param name="xi">one field of object x</param>
        /// <param name="yi">one field of object y</param>
        /// <exception cref="Exception">unbuild exception</exception>
        public void UnBuild(int xi, int yi)
        {
            int x = GetField(xi, yi).GetX;
            int y = GetField(xi, yi).GetY;
            bool destroyable = true;
            switch (GetField(x, y))
            {
                case Zone zona:
                    destroyable = zona.IsDestroyable();
                    break;
                case Educational edu:
                    destroyable = edu.IsDestroyable();
                    break;
                default:
                    destroyable = GetField(x, y).IsDestroyable();
                    break;
            }
            if (GetField(x, y) is Road road)
            {
                if (destroyable || road.ConnectsPolice || road.ConnectsStadium)
                {
                    List<FieldData> returnlist = ConnectRoads(x, y, false);
                    bool same;
                    bool candestroy = false;
                    road.Pass();
                    List<FieldData> templist;

                    //down
                    templist = ConnectRoads(x + 1, y, false);
                    same = templist.Count != 0;
                    foreach (FieldData ret in returnlist)
                    {
                        if (!templist.Contains(ret)) same = false;
                    }
                    if (same) candestroy = true;

                    //up
                    templist = ConnectRoads(x - 1, y, false);
                    same = templist.Count != 0;
                    foreach (FieldData ret in returnlist)
                    {
                        if (!templist.Contains(ret)) same = false;
                    }
                    if (same) candestroy = true;

                    //right
                    templist = ConnectRoads(x, y + 1, false);
                    same = templist.Count != 0;
                    foreach (FieldData ret in returnlist)
                    {
                        if (!templist.Contains(ret)) same = false;
                    }
                    if (same) candestroy = true;

                    //left
                    templist = ConnectRoads(x, y - 1, false);
                    same = templist.Count != 0;
                    foreach (FieldData ret in returnlist)
                    {
                        if (!templist.Contains(ret)) same = false;
                    }
                    if (same) candestroy = true;
                    road.UnPass();

                    if ((candestroy || !Connects(returnlist)) && !IsPoliceNearby(x, y) && !IsStadiumNearby(x, y))
                    {
                        FieldData tempobj = new FieldData(x, y, FieldType.Empty);
                        map[x, y] = new Field(tempobj);
                        OnFieldChanged(x, y, 1, 1, 0);

                        UpdateNeighbourRoadConnections(x, y);

                        if (IsPoliceNearby(x, y) || IsStadiumNearby(x, y))
                        {
                            tempobj = new Road(x, y, FieldType.Road);
                            map[x, y] = new Field(tempobj);
                        }
                        else
                        {
                            upkeepRoad--;
                        }

                        OnFieldChanged(x, y, 1, 1, 0);
                    }
                    else
                    {
                        bool ispolice = false;
                        bool isstadium = false;
                        foreach (FieldData ret in returnlist)
                        {
                            if (ret.GetType == FieldType.Police) ispolice = true;
                            if (ret.GetType == FieldType.Stadium) isstadium = true;
                        }
                        road.ConnectsPolice = ispolice;
                        road.ConnectsStadium = isstadium;
                        road.MakeUndestroyable();
                    }
                }
                else
                {
                    throw new Exception("You can not destroy this road!");
                }
            }
            else if (destroyable
                && GetField(x, y).GetType != FieldType.Empty)
            {
                int income = 0;
                switch (GetField(x, y).GetType)
                {
                    case FieldType.Police:
                        if (!GetField(x, y).IsDemolished)
                        {
                            SafetySpread(x, y, false);
                            SafetyHappinessUpdate();
                            upkeepBuilding--;
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Building demolished: +" + income);
                            OnIncomeChanged();
                            UpdateHappiness();
                        }
                        break;
                    case FieldType.Stadium:
                        if (!GetField(x, y).IsDemolished)
                        {
                            SpreadBonus(x, y, false);
                            BonusHappinessUpdate();
                            upkeepBuilding--;
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Building demolished: +" + income);
                            OnIncomeChanged();
                            UpdateHappiness();
                        }
                        break;
                    case FieldType.School:
                    case FieldType.University:
                        if (!GetField(x, y).IsDemolished)
                        {
                            upkeepBuilding--;
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Building demolished: +" + income);
                            OnIncomeChanged();
                        }
                        break;
                    case FieldType.IndustrialZone:
                        if (!GetField(x, y).IsDemolished)
                        {
                            SpreadSmog(x, y, false);
                            SmogHappinessUpdate();
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Zone unassigned: +" + income);
                            OnIncomeChanged();
                            UpdateHappiness();
                        }
                        break;
                    case FieldType.ResidentialZone:
                        if (!GetField(x, y).IsDemolished)
                        {
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Zone unassigned: +" + income);
                            OnIncomeChanged();
                        }
                        break;
                    case FieldType.CommercialZone:
                        if (!GetField(x, y).IsDemolished)
                        {
                            income = (int)(GetField(x, y).GetExpense() * 0.3);
                            incomes.Enqueue("Zone unassigned: +" + income);
                            OnIncomeChanged();
                        }
                        break;
                    default:
                        break;
                }
                balance += income;
                int height = GetField(x, y).GetHeight();
                int width = GetField(x, y).GetWidth();
                for (int i = x; i < x + height; i++)
                {
                    for (int j = y; j < y + width; j++)
                    {
                        FieldData tempobj = new FieldData(i, j, FieldType.Empty);
                        map[i, j] = new Field(tempobj);
                    }
                }
                OnFieldChanged(x, y, height, width, 0);
            }
            else
            {
                throw new Exception("Field is not destroyable!");
            }
        }

        /// <summary>
        /// When we want to upgrade a zone
        /// </summary>
        /// <param name="z">zone object</param>
        public void Upgrade(Zone z)
        {
            if (z.GetCurrentLevel != 3)
            {
                z.Upgrade();
                balance -= z.GetExpense();
                expenses.Enqueue("Zone upgraded: -" + z.GetExpense());
                OnExpenseChanged();
                OnFieldChanged(z.GetX, z.GetY, z.GetHeight(), z.GetWidth(), z.GetCurrentLevel);
            }
        }

        #endregion

        #region Update for view
        private void OnWorkplaceUpdate(object obj, EventArgs e)
        {
            if (obj is WorkPlace o)
            {
                if (o.GetOccupancy <= 5)
                {
                    OnFieldChanged(o.GetX, o.GetY, o.GetHeight(), o.GetWidth(), 1);
                }
                else if (o.GetOccupancy <= 10)
                {
                    OnFieldChanged(o.GetX, o.GetY, o.GetHeight(), o.GetWidth(), 2);
                }
                else if (o.GetOccupancy <= 20)
                {
                    OnFieldChanged(o.GetX, o.GetY, o.GetHeight(), o.GetWidth(), 3);
                }
            }
        }

        private void UpdateNeighbourRoadConnections(int x, int y)
        {
            if (x - 1 >= 0)
            {
                if (GetField(x - 1, y).GetType == FieldType.Road)
                {
                    ((Road)GetField(x - 1, y)).UpdateConnections(-4);
                    OnFieldChanged(x - 1, y, 1, 1, 0);
                }
            }
            if (y + 1 < GetMapDimention)
            {
                if (GetField(x, y + 1).GetType == FieldType.Road)
                {
                    ((Road)GetField(x, y + 1)).UpdateConnections(-8);
                    OnFieldChanged(x, y + 1, 1, 1, 0);
                }
            }
            if (x + 1 < GetMapDimention)
            {
                if (GetField(x + 1, y).GetType == FieldType.Road)
                {
                    ((Road)GetField(x + 1, y)).UpdateConnections(-1);
                    OnFieldChanged(x + 1, y, 1, 1, 0);
                }
            }
            if (y - 1 >= 0)
            {
                if (GetField(x, y - 1).GetType == FieldType.Road)
                {
                    ((Road)GetField(x, y - 1)).UpdateConnections(-2);
                    OnFieldChanged(x, y - 1, 1, 1, 0);
                }
            }
        }

        private int GetRoadConnections(int x, int y)
        {
            int num = 0;
            if (x - 1 >= 0)
            {
                if (GetField(x - 1, y).GetType == FieldType.Road)
                {
                    num += 1;
                    ((Road)GetField(x - 1, y)).UpdateConnections(4);
                    OnFieldChanged(x - 1, y, 1, 1, 0);
                }
            }
            if (y + 1 < GetMapDimention)
            {                
                if (GetField(x, y + 1).GetType == FieldType.Road)
                {
                    num += 2;
                    ((Road)GetField(x, y + 1)).UpdateConnections(8);
                    OnFieldChanged(x, y + 1, 1, 1, 0);
                }
            }
            if (x + 1 < GetMapDimention)
            {                
                if (GetField(x + 1, y).GetType == FieldType.Road)
                {
                    num += 4;
                    ((Road)GetField(x + 1, y)).UpdateConnections(1);
                    OnFieldChanged(x + 1, y, 1, 1, 0);
                }
            }
            if (y - 1 >= 0)
            {
                if (GetField(x, y - 1).GetType == FieldType.Road)
                {                   
                    num += 8;
                    ((Road)GetField(x, y - 1)).UpdateConnections(2);
                    OnFieldChanged(x, y - 1, 1, 1, 0);
                }
            }
            return num;
        }
                

        #endregion

        #region Private Methods
        private void InitForests()
        {
            FieldData temp = new Forest(20, 30, FieldType.Forest);
            map[20, 30] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(13, 15, FieldType.Forest);
            map[13, 15] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(1, 2, FieldType.Forest);
            map[1, 2] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(25, 30, FieldType.Forest);
            map[25, 30] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(5, 9, FieldType.Forest);
            map[5, 9] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(3, 15, FieldType.Forest);
            map[3, 15] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(5, 20, FieldType.Forest);
            map[5, 20] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(8, 3, FieldType.Forest);
            map[8, 3] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(7, 7, FieldType.Forest);
            map[7, 7] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(10, 10, FieldType.Forest);
            map[10, 10] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(28, 3, FieldType.Forest);
            map[28, 3] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(47, 7, FieldType.Forest);
            map[47, 7] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(50, 10, FieldType.Forest);
            map[50, 10] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(78, 93, FieldType.Forest);
            map[78, 93] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(27, 7, FieldType.Forest);
            map[27, 7] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(40, 30, FieldType.Forest);
            map[40, 30] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(37, 5, FieldType.Forest);
            map[37, 5] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(46, 10, FieldType.Forest);
            map[46, 10] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(30, 7, FieldType.Forest);
            map[30, 7] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(40, 15, FieldType.Forest);
            map[40, 15] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(25, 5, FieldType.Forest);
            map[25, 5] = new Field(temp);
            forests.Add((Forest)temp);

            temp = new Forest(50, 10, FieldType.Forest);
            map[50, 10] = new Field(temp);
            forests.Add((Forest)temp);
        }
        private double GetRadius(int x, int y, int i, int j)
        {
            return Math.Sqrt(Math.Pow(x - i, 2) + Math.Pow(y - j, 2));
        }

        private void MakeRoadsDestroyable(int x, int y, int height, int width)
        {
            int k = y - 1;
            int l = y + width;
            for (int i = x; i < x + height; i++)
            {

                if (i >= 0 && i <= 100)
                {
                    if (k >= 0 && k <= 100)
                    {
                        if (GetField(i, k) is Road road) road.MakeDestroyable();
                    }
                    if (l >= 0 && l <= 100)
                    {
                        if (GetField(i, l) is Road road) road.MakeDestroyable();
                    }
                }
            }
            k = x - 1;
            l = x + height;
            for (int j = y; j < y + width; j++)
            {

                if (j >= 0 && j <= 100)
                {
                    if (k >= 0 && k <= 100)
                    {
                        if (GetField(j, k) is Road road) road.MakeDestroyable();
                    }
                    if (l >= 0 && l <= 100)
                    {
                        if (GetField(j, l) is Road road) road.MakeDestroyable();
                    }
                }
            }
        }

        private void CutTies(FieldData institution)
        {
            foreach (ResidentialZone house in residences)
            {
                house.CutTiesWith(institution);
            }
        }
        private bool isPlaceable(int x, int y, int height, int width)
        {
            for (int i = x; i < x + height; i++)
            {
                for (int j = y; j < y + width; j++)
                {
                    if (GetField(i,j).GetType != FieldType.Empty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool isRoadNearby(int x, int y, int height, int width)
        {
            for (int i = 0; i < height; i++)
            {
                if ( y > 0 && GetField(x + i, y - 1).GetType == FieldType.Road)
                {
                    return true;
                }
                if (y + width < GetMapDimention && GetField(x + i, y + width).GetType == FieldType.Road)
                {
                    return true;
                }
            }
            for (int j = 0; j < width; j++)
            {
                if (x > 0 &&  GetField(x - 1, y + j).GetType == FieldType.Road)
                {
                    return true;
                }
                if (x + height < GetMapDimention && GetField(x + height, y + j).GetType == FieldType.Road)
                {
                    return true;
                }
            }
            return false;
        }

        private Boolean IsWithinRadius(int x, int y, int i, int j, int height, int width, double spread)
        {
            double px = x + (height - 1) / 2;
            double py = y + (width - 1) / 2;
            double distance = Math.Sqrt(Math.Pow(px - i, 2) + Math.Pow(py - j, 2));
            return distance < spread;
        }

        private bool IsBalance()
        {
            int indsum = 0;
            int comsum = 0;
            foreach (FieldData inst in institutions)
            {
                if (inst is IndustrialZone ind)
                {
                    indsum += ind.GetOccupancy;
                }
                else if (inst is CommertialZone com)
                {
                    comsum += com.GetOccupancy;
                }
            }

            if (indsum == 0 && comsum == 0) return true;
            else if (indsum == 0 || comsum == 0) return false;
            if (Math.Abs(population / indsum - population / comsum) > 0.3) return false;
            else return true;
        }

        #endregion

        #region Private for Unbuild

        private Boolean Connects(List<FieldData> buildings)
        {
            if (buildings.Count < 2) return false;
            bool residential = false;
            bool service = false;
            foreach (FieldData building in buildings)
            {
                if (building is ResidentialZone res && !res.IsDestroyable()) residential = true;
                if ((building is WorkPlace work) && !work.IsDestroyable()) service = true;
                if ((building is Educational edu) && !edu.IsDestroyable()) service = true;
                if (residential && service) return true;
            }
            return false;
        }

        private Boolean IsPoliceNearby(int x, int y)
        {
            bool police = false;
            if (x + 1 < GetMapDimention)
            {
                if (GetField(x + 1, y).GetType == FieldType.Police)
                {
                    police = true;
                    if (isRoadNearby(GetField(x + 1, y).GetX, GetField(x + 1, y).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (GetField(x - 1, y).GetType == FieldType.Police)
                {
                    police = true;
                    if (isRoadNearby(GetField(x - 1, y).GetX, GetField(x - 1, y).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (y + 1 < GetMapDimention)
            {
                if (GetField(x, y + 1).GetType == FieldType.Police)
                {
                    police = true;
                    if (isRoadNearby(GetField(x, y + 1).GetX, GetField(x, y + 1).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (y - 1 >= 0)
            {
                if (GetField(x, y - 1).GetType == FieldType.Police)
                {
                    police = true;
                    if (isRoadNearby(GetField(x, y - 1).GetX, GetField(x, y - 1).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }

            return police;
        }

        private Boolean IsStadiumNearby(int x, int y)
        {
            bool police = false;
            if (x + 1 < GetMapDimention)
            {
                if (GetField(x + 1, y).GetType == FieldType.Stadium)
                {
                    police = true;
                    if (isRoadNearby(GetField(x + 1, y).GetX, GetField(x + 1, y).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (GetField(x - 1, y).GetType == FieldType.Stadium)
                {
                    police = true;
                    if (isRoadNearby(GetField(x - 1, y).GetX, GetField(x - 1, y).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (y + 1 < GetMapDimention)
            {
                if (GetField(x, y + 1).GetType == FieldType.Stadium)
                {
                    police = true;
                    if (isRoadNearby(GetField(x, y + 1).GetX, GetField(x, y + 1).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            if (y - 1 >= 0)
            {
                if (GetField(x, y - 1).GetType == FieldType.Stadium)
                {
                    police = true;
                    if (isRoadNearby(GetField(x, y - 1).GetX, GetField(x, y - 1).GetY, 2, 2))
                    {
                        police = false;
                    }
                }
            }
            

            return police;
        }

        #endregion

        #region Private for Build (Connections)

        private void ConnectBuildings(List<FieldData> connected)
        {
            foreach(FieldData building in connected)
            {
                if (building is ResidentialZone house)
                {
                    ConnectResidents(house.GetX, house.GetY);
                }
            }
        }

        private List<FieldData> ConnectRoads(int x, int y, bool build)
        {
            if (x < 0 || x >= GetMapDimention || y < 0 || y >= GetMapDimention)
                return new List<FieldData>();

            List<FieldData> returnlist = new List<FieldData>();

            if (GetField(x, y) is Road road)
            {
                if (!road.IsPassed())
                {
                    road.Pass();
                    List<FieldData> templist;
                    //le
                    templist = ConnectRoads(x + 1, y, build);
                    foreach (FieldData temp in templist)
                    {
                        if (!returnlist.Contains(temp)) returnlist.Add(temp);
                    }
                    //fel
                    templist = ConnectRoads(x - 1, y, build);
                    foreach (FieldData temp in templist)
                    {
                        if (!returnlist.Contains(temp)) returnlist.Add(temp);
                    }
                    //jobbra
                    templist = ConnectRoads(x, y + 1, build);
                    foreach (FieldData temp in templist)
                    {
                        if (!returnlist.Contains(temp)) returnlist.Add(temp);
                    }
                    //balra
                    templist = ConnectRoads(x, y - 1, build);
                    foreach (FieldData temp in templist)
                    {
                        if (!returnlist.Contains(temp)) returnlist.Add(temp);
                    }
                    road.UnPass();
                   
                }
            }
            else
            {
                switch(map[x, y].GetData.GetType)
                {
                    case FieldType.ResidentialZone:
                    case FieldType.IndustrialZone:
                    case FieldType.CommercialZone:
                    case FieldType.School:
                    case FieldType.University:
                        if (!returnlist.Contains(GetField(x,y)) && !GetField(x,y).IsDemolished)
                        {
                            if (build)
                            {
                                returnlist.Add(GetField(x, y));
                            }
                            else
                            {
                                bool destroyable = true;
                                if (GetField(x,y) is Zone zona)
                                {
                                    destroyable = zona.IsDestroyable();
                                }
                                if(GetField(x,y) is Educational edu)
                                {
                                    destroyable = edu.IsDestroyable();
                                }
                                if (!destroyable)
                                {
                                    returnlist.Add(GetField(x, y));
                                }
                            }
                            
                        }
                        break;
                    case FieldType.Police:
                    case FieldType.Stadium:
                        if (!returnlist.Contains(GetField(x, y)) && !build && !GetField(x, y).IsDemolished)
                        {
                            returnlist.Add(GetField(x, y));
                        }
                        break;
                    default:
                        break;
                }
            }
            return returnlist;
        }

        private void DownRoadToResidential(int x, int y, FieldData service, int distance)
        {
            if (x < 0 || x >= GetMapDimention || y < 0 || y >= GetMapDimention)
                return;

            if(GetField(x,y) is Road road)
            {
                distance++;
                if (!road.IsPassed())
                {
                    road.Pass();
                    DownRoadToResidential(x + 1, y, service,distance); //le
                    DownRoadToResidential(x - 1, y, service,distance); //fel
                    DownRoadToResidential(x, y + 1, service,distance); //jobbra
                    DownRoadToResidential(x, y - 1, service,distance); //balra
                    road.UnPass();
                }
            }
            else if(GetField(x,y) is ResidentialZone house)
            {
                house.ConnectTo(service,distance);
            }
        }

        private void ConnectServices(int x, int y)
        {
            int height = GetField(x, y).GetHeight();
            int width = GetField(x, y).GetWidth();
            for (int i = 0; i < height; i++)
            {
                if (x + i >= GetMapDimention || y - 1 < 0)
                    return;
                if (GetField(x + i, y - 1).GetType == FieldType.Road)
                {
                    DownRoadToResidential(x + i, y - 1, GetField(x, y), 0);
                }
                if (y + width >= GetMapDimention)
                    return;
                if (GetField(x + i, y + width).GetType == FieldType.Road)
                {
                    DownRoadToResidential(x + i, y + width, GetField(x, y), 0);
                }
            }
            for (int j = 0; j < width; j++)
            {
                if (y + j >= GetMapDimention || x - 1 < 0)
                    return;
                if (GetField(x - 1, y + j).GetType == FieldType.Road)
                {
                    DownRoadToResidential(x - 1, y + j, GetField(x, y), 0);
                }
                if (x + height >= GetMapDimention || y + j > GetMapDimention)
                    return;
                if (GetField(x + height, y + j).GetType == FieldType.Road)
                {
                    DownRoadToResidential(x + height, y + j, GetField(x, y), 0);
                }
            }
        }

        private void DownRoadToServices(int x, int y, ResidentialZone house, int distance)
        {
            if (x < 0 || x >= GetMapDimention || y < 0 || y >= GetMapDimention)
                return;

            if (GetField(x, y) is Road road)
            {
                distance++;
                if (!road.IsPassed())
                {
                    road.Pass();
                    DownRoadToServices(x + 1, y, house, distance); //le
                    DownRoadToServices(x - 1, y, house, distance); //fel
                    DownRoadToServices(x, y + 1, house, distance); //jobbra
                    DownRoadToServices(x, y - 1, house, distance); //balra
                    road.UnPass();
                }
            }
            else if (GetField(x, y) is WorkPlace || GetField(x,y) is Educational)
            {
                house.ConnectTo(GetField(x,y),distance);
            }
        }

        private void ConnectResidents(int x, int y)
        {
            //keressuk az utat
            int height = GetField(x, y).GetHeight();
            int width = GetField(x, y).GetWidth();
            if(GetField(x,y) is ResidentialZone house)
            {
                for (int i = 0; i < height; i++)
                {
                    if (x + i >= GetMapDimention || y - 1 < 0)
                        return;
                    if (GetField(x + i, y - 1).GetType == FieldType.Road)
                    {
                        DownRoadToServices(x + i, y - 1, house, 0);
                    }
                    if (y + width >= GetMapDimention)
                        return;
                    if (GetField(x + i, y + width).GetType == FieldType.Road)
                    {
                        DownRoadToServices(x + i, y + width, house, 0);
                    }
                }
                for (int j = 0; j < width; j++)
                {
                    if (x - 1 < 0 || y + j >= GetMapDimention)
                        return;
                    if (GetField(x - 1, y + j).GetType == FieldType.Road)
                    {
                        DownRoadToServices(x - 1, y + j, house, 0);
                    }
                    if (x + height >= GetMapDimention)
                        return;
                    if (GetField(x + height, y + j).GetType == FieldType.Road)
                    {
                        DownRoadToServices(x + height, y + j, house, 0);
                    }
                }
            }
        }

        #endregion
           
        #region Catastrophe Function
        /// <summary>
        /// What happens when catastrophe strucks
        /// </summary>
        /// <param name="x">epicenter x</param>
        /// <param name="y">epicenter y</param>
        public void Catastrophe(int x, int y)
        {
            int startI = x - 10;
            int startJ = y - 10;
            for (int i = startI; i < x + 11; i++)
            {
                for (int j = startJ; j < y + 11; j++)
                {
                    if (i >= 0 && i < 100 && j >= 0 && j < 100)
                    {
                        if ((map[i, j].GetType == FieldType.ResidentialZone || map[i, j].GetType == FieldType.IndustrialZone || map[i, j].GetType == FieldType.CommercialZone ||
                            map[i, j].GetType == FieldType.School || map[i, j].GetType == FieldType.University || map[i, j].GetType == FieldType.Police ||
                            map[i, j].GetType == FieldType.Stadium) && map[i, j].GetData.IsDemolished == false)
                        {
                            Random r = new Random(DateTime.Now.Millisecond);
                            if (r.Next(1, 15) > GetRadius(x, y, i, j))
                            {
                                switch (map[i, j].GetData)
                                {
                                    case ResidentialZone house:
                                        population -= house.GetOccupancy;
                                        house.Collapse();
                                        break;
                                    case WorkPlace work:
                                        work.Collapse();
                                        CutTies(work);
                                        break;
                                    case Educational school:
                                        school.Collapse();
                                        CutTies(school);
                                        break;
                                    default:
                                        map[i, j].GetData.Collapse();
                                        break;
                                }
                                switch (map[i, j].GetData.GetType)
                                {
                                    case FieldType.Police:
                                        upkeepBuilding--;
                                        SafetySpread(GetField(i, j).GetX, GetField(i, j).GetY, false);
                                        SafetyHappinessUpdate();
                                        break;
                                    case FieldType.Stadium:
                                        upkeepBuilding--;
                                        SpreadBonus(GetField(i, j).GetX, GetField(i, j).GetY, false);
                                        BonusHappinessUpdate();
                                        break;
                                    case FieldType.School:
                                    case FieldType.University:
                                        upkeepBuilding--;
                                        break;
                                    case FieldType.IndustrialZone:
                                        SpreadSmog(x, y, false);
                                        SmogHappinessUpdate();
                                        break;
                                }
                                MakeRoadsDestroyable(GetField(i, j).GetX, GetField(i, j).GetY, map[i, j].GetData.GetHeight(), map[i, j].GetData.GetWidth());
                                UpdateHappiness();
                            }
                        }
                    }
                }
            }
            OnCatastropheChanged();
        }

        #endregion

        #region IndustrialZone Function - Smog
        
        /// <summary>
        /// Search for industrial zone nearby to decrease happiness
        /// </summary>
        /// <param name="obj">caller ResidentialZone</param>
        private void SmogInhale(int x, int y, int height, int width, ResidentialZone obj)
        {
            List<FieldData> found = new List<FieldData>();
            for (int i = x - 6; i < x + height + 6; i++)
            {
                for (int j = y - 6; j < y + width + 6; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (GetField(i, j).GetType == FieldType.IndustrialZone && !found.Contains(GetField(i, j)))
                        {
                            found.Add(GetField(i, j));
                            for (int k = x; k < x + height; k++)
                            {
                                for (int l = y; l < y + width; l++)
                                {
                                    if (IsWithinRadius(GetField(i, j).GetX, GetField(i, j).GetY, k, l, 3, 3, 8))
                                    {
                                        obj.SmogRise();
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Spread unhappiness from IndustrialZone
        /// </summary>
        /// <param name="x">source x</param>
        /// <param name="y">source y</param>
        /// <param name="build">unhappiness is increased or decreased</param>
        private void SpreadSmog(int x, int y, bool build)
        {
            for (int i = x - 6; i < x + 9; i++)
            {
                for (int j = y - 6; j < y + 9; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (IsWithinRadius(x, y, i, j, 3, 3, 8) && GetField(i, j) is ResidentialZone house)
                        {
                            if (build)
                            {
                                house.SmogRise();
                            }
                            else
                            {
                                house.SmogDrop();
                            }

                        }
                    }
                }
            }
        }

        #endregion

        #region Police Function - Safety

        /// <summary>
        /// Search for police station to increase happiness
        /// </summary>
        /// <param name="obj">caller zone</param>
        private void SearchForSafety(int x, int y, int height, int width, Zone obj)
        {
            List<FieldData> found = new List<FieldData>();
            for (int i = x - 6; i < x + height + 6; i++)
            {
                for (int j = y - 6; j < y + width + 6; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (GetField(i, j).GetType == FieldType.Police && !found.Contains(GetField(i, j)))
                        {
                            found.Add(GetField(i, j));
                            for (int k = x; k < x + height; k++)
                            {
                                for (int l = y; l < y + width; l++)
                                {
                                    if (IsWithinRadius(GetField(i, j).GetX, GetField(i, j).GetY, k, l, 2, 2, 6.5))
                                    {
                                        obj.SafetyRise();
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Spread happiness from police station
        /// </summary>
        /// <param name="x">source x</param>
        /// <param name="y">source y</param>
        /// <param name="build">increase or decrease happiness</param>
        private void SafetySpread(int x, int y, bool build)
        {
            for (int i = x - 6; i < x + 8; i++)
            {
                for (int j = y - 6; j < y + 8; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (IsWithinRadius(x, y, i, j, 2, 2, 6.5) && GetField(i, j) is Zone zona)
                        {
                            if (build)
                            {
                                zona.SafetyRise();
                            }
                            else
                            {
                                zona.SafetyDrop();
                            }

                        }
                    }
                }
            }
        }

        #endregion

        #region Stadium Function - Bonus

        /// <summary>
        /// Search for stadium to increase happiness
        /// </summary>
        /// <param name="obj">caller zone</param>
        private void SearchForBonus(int x, int y, Zone obj)
        {
            List<FieldData> found = new List<FieldData>();
            for (int i = x - 6; i < x + 9; i++)
            {
                for (int j = y - 6; j < y + 9; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (GetField(i, j).GetType == FieldType.Stadium && !found.Contains(GetField(i, j)))
                        {
                            found.Add(GetField(i, j));
                            for (int k = x; k < x + 3; k++)
                            {
                                for (int l = y; l < y + 3; l++)
                                {
                                    if (IsWithinRadius(GetField(i, j).GetX, GetField(i, j).GetY, k, l, 2, 2, 6.5))
                                    {
                                        obj.AddBonus();
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Spread happiness from stadium
        /// </summary>
        /// <param name="x">source x</param>
        /// <param name="y">source y</param>
        /// <param name="build">increase or decrease happiness</param>
        private void SpreadBonus(int x, int y, bool build)
        {
            for (int i = x - 6; i < x + 8; i++)
            {
                for (int j = y - 6; j < y + 8; j++)
                {
                    if (i > -1 && i < GetMapDimention && j > -1 && j < GetMapDimention)
                    {
                        if (IsWithinRadius(x, y, i, j, 2, 2, 6.5) && GetField(i, j) is Zone zona)
                        {
                            if (build)
                            {
                                zona.AddBonus();
                            }
                            else
                            {
                                zona.RemoveBonus();
                            }

                        }
                    }
                }
            }
        }

        #endregion

        #region Forest Function

        private bool ForestContinue(int x, int y)
        {
            if (x > -1 && x < GetMapDimention && y > -1 && y < GetMapDimention)
            {
                if (GetField(x, y).GetType == FieldType.Road) return true;
                if (GetField(x, y).GetType == FieldType.Empty) return true;
                if (GetField(x, y).GetType == FieldType.Forest) return true;
                if (GetField(x, y).GetType == FieldType.ResidentialZone) return true;
                return false;
            }
            return false;
        }

        private ResidentialZone? ForestFoundResidential(int x, int y)
        {
            if (x > -1 && x < GetMapDimention && y > -1 && y < GetMapDimention)
            {
                if (GetField(x, y) is ResidentialZone res) return res;
                else return null;
            }
            return null;
        }

        private IndustrialZone? ForestFoundIndustrial(int x, int y)
        {
            if (x > -1 && x < GetMapDimention && y > -1 && y < GetMapDimention)
            {
                if (GetField(x, y) is IndustrialZone ind) return ind;
                else return null;
            }
            return null;
        }

        /// <summary>
        /// Search for IndustrialZone in opposite direction that affects ResidentialZone
        /// </summary>
        /// <param name="startx">forest x</param>
        /// <param name="starty">forest y</param>
        /// <param name="xs">direction x multiplier</param>
        /// <param name="ys">direction y multiplier</param>
        /// <param name="f">residential zone distance from forest</param>
        /// <returns></returns>
        private bool ForestSearchIndustrial(int startx, int starty, int xs, int ys, int f)
        {
            int x;
            int y;
            for(int i = 1; i <= 6 - f; i++)
            {
                x = startx + i * xs;
                y = starty + i * ys;
                IndustrialZone? tempInd = ForestFoundIndustrial(x, y);
                if(tempInd != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Spread happiness from forest in a direction
        /// </summary>
        /// <param name="startx">source x</param>
        /// <param name="starty">source y</param>
        /// <param name="xs">direction x multiplier</param>
        /// <param name="ys">direction y multiplier</param>
        private void ForestSpread(int startx, int starty, int xs, int ys)
        {
            int x;
            int y;
            for(int i = 1; i <= 3; i++)
            {
                x = startx + xs * i;
                y = starty + ys * i;
                if (!ForestContinue(x, y)) break;
                ResidentialZone? tempRes = ForestFoundResidential(x, y);
                if (tempRes != null)
                {
                    tempRes.AddForestBonus();
                    if(ForestSearchIndustrial(startx, starty, (-1)*xs, (-1)*ys, i))
                    {
                        tempRes.AddForestBonus();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Spread happiness from forest if not fully grown
        /// </summary>
        /// <param name="x">source x</param>
        /// <param name="y">source y</param>
        /// <param name="tempobj">source forest</param>
        private void ForestBonusUpdate(int x, int y, Forest tempobj)
        {
            if (!tempobj.IsGrown())
            {
                ForestSpread(x, y, 0, 1); //right
                ForestSpread(x, y, 1, 0); //down
                ForestSpread(x, y, 0, -1); //left
                ForestSpread(x, y, -1, 0); //up
                ForestSpread(x, y, 1, 1); //down right
                ForestSpread(x, y, -1, -1); //up left
                ForestSpread(x, y, -1, 1); //up right
                ForestSpread(x, y, 1, -1); //down left
            }
        }
        #endregion

        #region Happiness Updates

        private void TaxHappiness(int tax)
        {
            int h;
            switch (tax)
            {
                case int i when i < 50:
                    h = 120 - tax;
                    break;
                default:
                    h = 105 - tax;
                    break;
            }
            if (h > 100) h = 100;

            foreach (ResidentialZone house in residences)
            {
                house.TaxHappinessForPeople(h);
            }
        }

        private void SafetyHappinessUpdate()
        {
            foreach (ResidentialZone residence in residences)
            {
                residence.SafetyHappinessUpdate();
            }
            foreach (FieldData ins in institutions)
            {
                if (ins is WorkPlace work)
                {
                    work.SafetyHappinessUpdate();
                }
            }
        }

        private void SmogHappinessUpdate()
        {
            foreach (ResidentialZone residence in residences)
            {
                residence.SmogHappinessUpdate();
            }
        }

        private void BonusHappinessUpdate()
        {
            foreach (ResidentialZone residence in residences)
            {
                residence.BonusHappinessUpdate();
                residence.ForestBonusHappinessUpdate();
            }
        }

        private void MinusHappinessUpdate(int change)
        {
            foreach (ResidentialZone residence in residences)
            {
                residence.MinusHappinessUpdate(change);
            }
        }

        private void UpdateHappiness()
        {
            int sum = 0;
            int r = 0;
            foreach (ResidentialZone residence in residences)
            {
                if (residence.GetOccupancy != 0)
                {
                    r++;
                    sum += residence.AllHappiness();
                }
            }
            if (r != 0)
            {
                happiness = sum / r;
            }
        }

        #endregion

        #region Event Triggers

        private void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, EventArgs.Empty);
        }

        private void OnIncomeChanged()
        {
            if (incomes.Count > 5) incomes.Dequeue();
            IncomeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnExpenseChanged()
        {
            if (expenses.Count > 5) expenses.Dequeue();
            ExpenseChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFieldChanged(int x, int y, int height, int width, int phase)
        {
            FieldChanged?.Invoke(this, new FieldChangedEventArgs(x, y, height, width, phase));
        }

        private void OnCatastropheChanged()
        {
            CatastropheChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }

}
