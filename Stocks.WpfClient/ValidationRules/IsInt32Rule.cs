using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Stocks.WpfClient.ValidationRules
{
    /// <summary>
    /// Validates that value is convertible to an Int32
    /// </summary>
    public class IsInt32Rule : ValidationRule
    {
        public override ValidationResult Validate(
            object value, System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (value != null 
                && value.ToString().Length > 0
                &&  !Int32.TryParse(value.ToString(), out i))
               return new ValidationResult(false, "Value must be an integer.");
            return ValidationResult.ValidResult;
        }
    }
}
