/* 
 * IndiansInc.MySqlBulklCopy
 * Helpful to copy a huge data set from one Mysql table to another.
 * Requirements: .NET Framework 3.5/Mono
 * Copyright (c) 2012 IndiansInc.MySqlBulkcopy (http://code.google.com/p/mysqlbulkcopy/)
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; version 2 of the license.
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 */

namespace IndiansInc.Internals
{
    /*
     * CommonFunctions
     * This class represents the common functions that are required for bulk copy
     * @author   Albert Arul Prakash<albertarulprakash@gmail.com>  
     */

    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Globalization;



    internal class CommonFunctions
    {

        /// <summary>
        /// Method to get the column names 
        /// </summary>
        /// <param name="mapItemCollection"></param>
        /// <param name="propertyToFetch"></param>
        /// <returns></returns>
        public string GetColumnNames(ColumnMapItemCollection mapItemCollection, ColumnProperty propertyToFetch)
        {
            if (mapItemCollection == null) { throw new ArgumentNullException("mapItemCollection"); }
            if (mapItemCollection.Count <= 0) { throw new ArgumentOutOfRangeException("mapItemCollection"); }

            StringBuilder builder = new StringBuilder();
            switch (propertyToFetch)
            {
                case ColumnProperty.Source:
                    foreach (ColumnMapItem columnMapItem in mapItemCollection)
                    {
                        builder.AppendFormat("{0},", columnMapItem.SourceColumn);
                    }
                    break;
                case ColumnProperty.Destination:
                    foreach (ColumnMapItem columnMapItem in mapItemCollection)
                    {
                        builder.AppendFormat("{0},", columnMapItem.DestinationColumn);
                    }
                    break;
                default:
                    builder.Append(",");
                    break;
            }
            return builder.ToString().Substring(0, builder.Length - 1);
        }

        public string ConstructSql(string tableName, DataRow row, ColumnMapItemCollection mapItemCollection)
        {
            // Get the column names that need to be used
            string columnNames = GetColumnNames(mapItemCollection, ColumnProperty.Destination);

            // construct the base Skeleton of the sql.
            string baseSql = string.Format("insert into {0}({1}) values({2})", tableName, columnNames, "{0}");

            // loop through the collection and construct the values string
            StringBuilder builder = new StringBuilder();
            foreach (ColumnMapItem columnMapItem in mapItemCollection)
            {
                string constructedValue = ConstructIndividualValue(columnMapItem.DataType,
                                                                   row[columnMapItem.SourceColumn].ToString());
                builder.Append(constructedValue);
            }
            return string.Format(baseSql, builder.ToString().Substring(0, builder.ToString().Length - 1));
        }

        /// <summary>
        /// Method that constructs the individual value. This method determines the quote model based on the datatype
        /// </summary>
        /// <param name="dataType">data type of destination column</param>
        /// <param name="value">Value that to be constructed</param>
        /// <returns>formatted value based on data type</returns>
        private string ConstructIndividualValue(string dataType, string value)
        {
            string returnValue = "";
            switch (dataType.ToUpper())
            {
                case "INT":
                case "TINYINT":
                case "SMALLINT":
                case "MEDIUMINT":
                case "BIGINT":
                case "FLOAT":
                case "DOUBLE":
                case "DECIMAL":
                    returnValue = string.Format("{0},", value);
                    break;
                case "CHAR":
                case "VARCHAR":
                case "BLOB":
                case "TEXT":
                case "TINYBLOB":
                case "TINYTEXT":
                case "MEDIUMBLOB":
                case "MEDIUMTEXT":
                case "LONGBLOB":
                case "LONGTEXT":
                case "ENUM":
                    returnValue = string.Format("'{0}',", MySql.Data.MySqlClient.MySqlHelper.EscapeString(value));
                    break;
                case "DATE":
                    returnValue = String.Format("'{0:yyyy-MM-dd}',", value);
                    //returnValue = string.Format(CultureInfo.InvariantCulture, "{0:dd-MM-yyyy}", value);
                    break;
                case "TIMESTAMP":
                case "DATETIME":
                    DateTime date = DateTime.Parse(value);
                    returnValue = String.Format("'{0:yyyy-MM-dd HH:mm:ss}',", date);
                    break;
                case "TIME":
                    returnValue = String.Format("'{0:HH:mm:ss}',", value);
                    break;

                case "YEAR2":
                    returnValue = String.Format("'{0:yy}',", value);
                    break;
                case "YEAR4":
                    returnValue = String.Format("'{0:yyyy}',", value);
                    break;
                default:
                    // we don't understand the format. to safegaurd the code, just enclose with ''
                    returnValue = string.Format("'{0}',", MySql.Data.MySqlClient.MySqlHelper.EscapeString(value));
                    break;
            }
            return returnValue;
        }
    }
}
