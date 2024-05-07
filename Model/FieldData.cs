using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Field object
    /// </summary>
    public class FieldData
    {
        #region Private Data

        private int x;
        private int y;

        private FieldType type;

        protected Boolean destroyable;
        protected bool demolished;

        #endregion

        #region Constructor

        public FieldData(int x, int y, FieldType type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            destroyable = true;
            demolished = false;
        }

        #endregion

        #region Properties, Getters

        public new FieldType GetType { get { return type; } }

        public int GetX { get { return x; } }

        public int GetY { get { return y; } }

        public Boolean IsDestroyable() { return destroyable; }
        public bool IsDemolished { get { return demolished; } }


        #endregion

        #region Public methods
        public int GetExpense()
        {
            switch (type)
            {
                case FieldType.Empty: return 0;
                case FieldType.Road: return 200;
                case FieldType.ResidentialZone:
                case FieldType.IndustrialZone:
                case FieldType.CommercialZone:
                case FieldType.School:
                    return 1000;
                case FieldType.Police:
                case FieldType.Stadium:
                case FieldType.University:
                    return 1500;
                case FieldType.Forest:
                    return 2000;
                default:
                    return 0;
            }
        }

        public int GetWidth()
        {
            switch (type)
            {
                case FieldType.Empty:
                case FieldType.Road:
                case FieldType.Forest:
                    return 1;
                case FieldType.ResidentialZone:
                case FieldType.IndustrialZone:
                case FieldType.CommercialZone:
                    return 3;
                case FieldType.Police:
                case FieldType.Stadium:
                case FieldType.University:
                    return 2;
                case FieldType.School:
                    return 2;
                default:
                    return 0;
            }
        }

        public int GetHeight()
        {
            switch (type)
            {
                case FieldType.Empty:
                case FieldType.Road:
                case FieldType.Forest:
                    return 1;
                case FieldType.ResidentialZone:
                case FieldType.IndustrialZone:
                case FieldType.CommercialZone:
                    return 3;
                case FieldType.Police:
                case FieldType.Stadium:
                case FieldType.University:
                    return 2;
                case FieldType.School:
                    return 1;
                default:
                    return 0;

            }
        }

        public void Collapse()
        {
            destroyable = true;
            demolished = true;
        }
        public virtual String GetInfo()
        {
            return "";
        }

        #endregion

    }
}
