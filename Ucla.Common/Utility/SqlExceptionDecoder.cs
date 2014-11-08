using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucla.Common.Utility
{
    public static class SqlExceptionDecoder
    {
        public static string GetFriendlyMessage(string objectName, Exception ex)
        {
            var msg = new StringBuilder();
            if (ex.Message.Contains("DELETE") && ex.Message.Contains("REFERENCE"))
                msg.AppendFormat("You cannot delete this {0} as it is referenced by other items in the application.", objectName);
            else if (ex.Message.Contains("String or binary data would be truncated"))
                msg.AppendFormat("One or more text inputs is too long for the database to save.");
            else
                msg.AppendFormat("Save {0} failed. The database returned the error message {1}", objectName, ex.Message);

            return "Save failed. " + msg.ToString();
        }
    }

}
