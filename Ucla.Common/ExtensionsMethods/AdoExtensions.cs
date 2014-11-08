using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient; 

namespace Ucla.Common.ExtensionMethods
{
    public static class AdoExtensions
    {
        #region Constants

        private const string NoColumnMessage = "There is no column \"{0}\" in the result set.";
        private const string InvalidCastMessage = "The column \"{0}\" with value {1} cannot be cast to a {2}.";
        private const string UnsupportedDataTypeMessage = "Data type {0} is only supported for MS SQL Server.";
        private const string UnexpectedNullMessage = "Column \"{0}\" has unexpected null value. Fix the database data to not return null, or supply a null replacement value.";

        #endregion // Constants

        #region AsSqlParameterValue

        public static object AsSqlParameterValue(this string dotNetValue, string nullReplacement = null)
        {
            return dotNetValue ?? nullReplacement ?? System.Data.SqlTypes.SqlString.Null;
        }

        public static object AsSqlParameterValue(this Byte? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlByte.Null;
        }

        public static object AsSqlParameterValue(this Byte? dotNetValue, Byte nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Int16? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlInt16.Null;
        }

        public static object AsSqlParameterValue(this Int16? dotNetValue, Int16 nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Int32? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlInt32.Null;
        }

        public static object AsSqlParameterValue(this Int32? dotNetValue, Int32 nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Int64? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlInt64.Null;
        }

        public static object AsSqlParameterValue(this Int64? dotNetValue, Int64 nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Decimal? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlDecimal.Null;
        }

        public static object AsSqlParameterValue(this Decimal? dotNetValue, Decimal nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this DateTime? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlDateTime.Null;
        }

        public static object AsSqlParameterValue(this DateTime? dotNetValue, DateTime nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Boolean? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlBoolean.Null;
        }

        public static object AsSqlParameterValue(this Boolean? dotNetValue, Boolean nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Single? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlSingle.Null;
        }

        public static object AsSqlParameterValue(this Single? dotNetValue, Single nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        public static object AsSqlParameterValue(this Double? dotNetValue)
        {
            return dotNetValue ?? System.Data.SqlTypes.SqlDouble.Null;
        }

        public static object AsSqlParameterValue(this Double? dotNetValue, Double nullReplacement)
        {
            return dotNetValue ?? nullReplacement;
        }

        #endregion
        #region Strings

        /// <summary>
        /// Convert a database value to a System.String that is not null, 
        /// optionally replacing null database values with a null replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is any of the
        /// char or nchar, varchar, nvarchar types.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static string AsString(this IDataReader dr, string colName, string nullReplacement = null)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    if (nullReplacement == null)
                    {
                        var message = String.Format(UnexpectedNullMessage, colName);
                        throw new ApplicationException(message);
                    }
                    return nullReplacement;
                }
                return dr.GetString(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "String");
                throw new ApplicationException(message, exi);
            }
        }


        /// <summary>
        /// Convert a database value to a System.String.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is any of the
        /// char or nchar, varchar, nvarchar types.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static string AsNullableString(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (string)null : dr.GetString(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "String");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion  // String
        #region Byte

        /// <summary>
        /// Convert a database value to a System.Byte,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is tinyint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static byte AsByte(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetByte(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Byte");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Byte,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is tinyint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static byte AsByte(this IDataReader dr, string colName, byte nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetByte(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Byte");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Byte>.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is tinyint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static byte? AsNullableByte(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (byte?)null : dr.GetByte(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Byte");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion
        #region Int16

        /// <summary>
        /// Convert a database value to a System.Int16,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is smallint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int16 AsInt16(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetInt16(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int16");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Int16,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is smallint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static Int16 AsInt16(this IDataReader dr, string colName, Int16 nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetInt16(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int16");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Int16>.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is smallint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int16? AsNullableInt16(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (Int16?)null : dr.GetInt16(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int16");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion
        #region Int32

        /// <summary>
        /// Convert a database value to a System.Int32,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is int.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int32 AsInt32(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetInt32(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int32");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Int32,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is int.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static Int32 AsInt32(this IDataReader dr, string colName, Int32 nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetInt32(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int32");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Int32>.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is int.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int32? AsNullableInt32(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (Int32?)null : dr.GetInt32(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int32");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion
        #region Int64


        /// <summary>
        /// Convert a database value to a System.Int64,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is bigint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int64 AsInt64(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetInt64(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int64");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Int64,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is bigint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static Int64 AsInt64(this IDataReader dr, string colName, Int64 nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetInt64(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int64");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Int64>.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is bigint.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Int64? AsNullableInt64(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (Int64?)null : dr.GetInt64(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Int64");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Int64
        #region Decimal

        /// <summary>
        /// Convert a database value to a System.Decimal,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is decimal,
        /// numeric (deprecated) or money.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Decimal AsDecimal(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetDecimal(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Decimal");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Decimal,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is decimal,
        /// numeric (deprecated) or money.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static Decimal AsDecimal(this IDataReader dr, string colName, Decimal nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetDecimal(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Decimal");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Decimal>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is decimal,
        /// numeric (deprecated) or money.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static decimal? AsNullableDecimal(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (decimal?)null : dr.GetDecimal(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Decimal");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Decimal
        #region Boolean

        /// <summary>
        /// Convert a database value to a System.Boolean,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is bit.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static Boolean AsBoolean(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetBoolean(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Boolean");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.Boolean,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is bit.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static Boolean AsBoolean(this IDataReader dr, string colName, Boolean nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetBoolean(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Boolean");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.Boolean>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is decimal.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static bool? AsNullableBoolean(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (bool?)null : dr.GetBoolean(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Boolean");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Boolean
        #region DateTime

        /// <summary>
        /// Convert a database value to a System.DateTime,
        /// throwing an exception if the database value is null.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server there are several data types that can convert to
        /// a System.DateTime, including datetime, datetime2(n), datetimeoffset, date, etc.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static DateTime AsDateTime(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                if (dr.IsDBNull(ordinal))
                {
                    var message = String.Format(UnexpectedNullMessage, colName);
                    throw new ApplicationException(message);
                }
                return dr.GetDateTime(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "DateTime");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a System.DateTime,
        /// replacing null database values with a specified replacement value.
        /// </summary>
        /// <remarks>
        /// In MS SQL Server there are several data types that can convert to
        /// a System.DateTime, including datetime, datetime2(n), datetimeoffset, date, etc.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// /// <param name="nullReplacement">If database value is null, substitute this value.</param>
        /// <returns>The converted value</returns>
        public static DateTime AsDateTime(this IDataReader dr, string colName, DateTime nullReplacement)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? nullReplacement : dr.GetDateTime(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "DateTime");
                throw new ApplicationException(message, exi);
            }
        }

        /// <summary>
        /// Convert a database value to a Nullable<System.DateTime>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server there are several data types that can convert to
        /// a System.DateTime, including datetime, datetime2(n), datetimeoffset, date, etc.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static DateTime? AsNullableDateTime(this IDataReader dr, string colName)
        {
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (DateTime?)null : dr.GetDateTime(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "DateTime");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // DateTime
        #region Timespan

        /// <summary>
        /// Convert a database value to a Nullable<System.TimeSpan>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is ???
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static TimeSpan? AsNullableTimeSpan(this IDataReader dr, string colName)
        {
            try
            {
                var sdr = dr as SqlDataReader;
                if (sdr == null)
                {
                    var message = String.Format(UnsupportedDataTypeMessage, "TimeSpan");
                    throw new ApplicationException(message);
                }
                int ordinal = sdr.GetOrdinal(colName);
                return sdr.IsDBNull(ordinal) ? (TimeSpan?)null : sdr.GetTimeSpan(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "TimeSpan");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Timespan
        #region Single

        /// <summary>
        /// Convert a database value to a Nullable<System.Single>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is float,
        /// though there are differences in precision that can trip you up.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static System.Single? AsNullableSingle(this IDataReader dr, string colName)
        {
            // The closest data types to System.Single are DbType.Single and SqlDbType.Real,
            // though the match is not exact.
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                var s = dr.GetFieldType(ordinal);
                return dr.IsDBNull(ordinal) ? (float?)null : dr.GetFloat(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Single");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Single
        #region Double

        /// <summary>
        /// Convert a database value to a Nullable<System.Double>
        /// </summary>
        /// <remarks>
        /// In MS SQL Server the corresponding data type is float, though
        /// there are differences in precision, so the mapping is not exact.
        /// </remarks>
        /// <param name="dr">DataReader</param>
        /// <param name="colName">Column Name</param>
        /// <returns>The converted value</returns>
        public static System.Double? AsNullableDouble(this IDataReader dr, string colName)
        {
            // The closest data types to System.Double in the database world are
            // DbType.Double and SqlDbType.Float (which is really confusing since float is
            // the less precise floating point data type in .Net, but the more precise
            // floating point data type in SQL Server.
            try
            {
                int ordinal = dr.GetOrdinal(colName);
                return dr.IsDBNull(ordinal) ? (double?)null : dr.GetDouble(ordinal);
            }
            catch (IndexOutOfRangeException exo)
            {
                var message = String.Format(NoColumnMessage, colName);
                throw new ApplicationException(message, exo);
            }
            catch (InvalidCastException exi)
            {
                var message = String.Format(InvalidCastMessage,
                    colName, dr[dr.GetOrdinal(colName)], "Double");
                throw new ApplicationException(message, exi);
            }
        }

        #endregion // Double
    }
}
