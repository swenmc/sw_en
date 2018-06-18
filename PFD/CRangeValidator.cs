using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PFD
{
    public class CRangeValidator : ValidationRule
    {

        public int Min { get; set; }
        public int Max { get; set; }
        public bool CheckZeroValue { get; set; }

        public CRangeValidator() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int Value = 0;

            try
            {
                if (((string)value).Length > 0)
                    Value = Int32.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Wrong Character - " + e.Message);
            }

            if (CheckZeroValue && (Value == 0))
                return new ValidationResult(true, null);

            if ((Value < Min) || (Value > Max))
            {
                return new ValidationResult(false, String.Format("The range is: {0} - {1}", Min, Max));
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
