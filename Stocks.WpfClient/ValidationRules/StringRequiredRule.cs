using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Stocks.WpfClient.ValidationRules
{
    class StringRequiredRule : ValidationRule
    {
        public override ValidationResult Validate(
            object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null || value.ToString().Length == 0)
                return new ValidationResult(false, "Field is required.");
            return ValidationResult.ValidResult;
        }
    }
}
