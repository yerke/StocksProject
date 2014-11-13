using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Stocks.WpfClient.ValidationRules
{
    /// <summary>
    /// Validates that a string is convertible to a double.
    /// </summary>
    public class IsDoubleRule : ValidationRule
    {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double i;
            if ( value != null
                && value.ToString().Length > 0
                && !Double.TryParse(value.ToString(), out i))
                return new ValidationResult(false, "Value must numeric.");
            return ValidationResult.ValidResult;
        }
    }
}
