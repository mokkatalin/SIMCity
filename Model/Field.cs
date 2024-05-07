using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityBuilder.Model
{
    /// <summary>
    /// Field placeholder in map
    /// </summary>
    public class Field
    {
        private FieldData fieldObject;

        public new FieldType GetType { get { return fieldObject.GetType;} }

        public FieldData GetData { get { return fieldObject; } }

        public Field(FieldData fieldObject)
        {
            this.fieldObject = fieldObject;
        }
    }
}