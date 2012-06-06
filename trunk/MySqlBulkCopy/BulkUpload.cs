/* 
 * IndiansInc.MySqlBulklCopy
 * A port of PHP IDS to the .NET Framework
 * Requirements: .NET Framework 2.0/Mono
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


namespace IndiansInc
{
/*
 * MySqlBulkCopy
 * This class represents a base class that should be used to copy data 
 * @author   Albert Arul Prakash<albertarulprakash@gmail.com>  
 */
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;
    using IndiansInc.Internals;

    public class MySqlBulkCopy
    {
        public event EventHandler OnUpdateProgress;

        public ColumnMapItemCollection ColumnMapItems
        {
            get;
            set;
        }

        public MySqlConnection DestinationDbConnection
        {
            get;
            set;
        }

        public string DestinationTableName
        {
            get;
            set;
        }

        public int BatchSize
        {
            get;
            set;
        }

        public void Upload(MySqlDataReader reader)
        {
            CommonFunctions functions = new CommonFunctions();

            string sql = "";

            if (reader.HasRows)
            {
                // there are rows. 
                System.Data.DataTable table = new System.Data.DataTable();
                table.Load(reader);
                int counter = 0;
                foreach (System.Data.DataRow item in table.Rows)
                {
                    sql = functions.ConstructSql(DestinationTableName, item, ColumnMapItems);

                    // SQL constructed. 
                    // Using the destination connection Execute the statement
                    Console.WriteLine(sql);
                    MySqlCommand command = new MySqlCommand(sql, DestinationDbConnection);
                    command.ExecuteNonQuery();
                    
                }
            }

        }

        public void Upload(System.Data.DataTable table)
        {
            int counter = 0;
            string sql ="";
            CommonFunctions functions = new CommonFunctions();
            foreach (System.Data.DataRow item in table.Rows)
            {
                sql = functions.ConstructSql(DestinationTableName, item, ColumnMapItems);

                // SQL constructed. 
                // Using the destination connection Execute the statement
                Console.WriteLine(sql);
                MySqlCommand command = new MySqlCommand(sql, DestinationDbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
