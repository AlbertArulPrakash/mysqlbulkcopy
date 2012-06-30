
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


namespace IndiansInc
{
    /*
     * MySqlBulkCopy
     * This class represents a base class that should be used to copy data 
     * @author   Albert Arul Prakash<albertarulprakash@gmail.com>  
     */
    /*
     * Version Information:
     * Version 0.1: Base version of upload is implemented
     * Version 0.2: Issue 2: Does not support Batch sizes like in SqlBulkCopy is completed
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
        /// <summary>
        /// Delegate to subscribe notification from assembly
        /// </summary>
        /// <param name="e">Event arguments </param>
        public delegate void OnBatchSizeCompletedDelegate(BatchSizeCompletedEventArgs e);
        /// <summary>
        /// All column mappings between source column and destination table columns
        /// </summary>
        public ColumnMapItemCollection ColumnMapItems
        {
            get;
            set;
        }
        /// <summary>
        /// The connection that to be used while connecting destination column
        /// </summary>
        public MySqlConnection DestinationDbConnection
        {
            get;
            set;
        }

        /// <summary>
        /// The destination table name that need to be updated.
        /// </summary>
        public string DestinationTableName
        {
            get;
            set;
        }

        /// <summary>
        /// Size of the batch that need to be completed before notifying caller
        /// </summary>
        public int BatchSize
        {
            get;
            set;
        }

        /// <summary>
        /// Delegate that need to invoked once the assembly uploads the specified BatchSize
        /// </summary>
        public OnBatchSizeCompletedDelegate OnBatchSizeCompleted { get; set; }

        /// <summary>
        /// Method that uploads the data from the MySqlDataReader that contains the data.
        /// </summary>
        /// <param name="reader">Data reader that contains the source data that to be uploaded</param>
        public void Upload(MySqlDataReader reader)
        {

            if (reader.HasRows)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                table.Load(reader);
                Upload(table);
            }
        }

        /// <summary>
        /// Method that uploads the data from the <see cref="System.Data.DataTable">DataTable</see> that contains the data.
        /// </summary>
        /// <param name="table">Data table that contains source data that to be uploaded</param>
        public void Upload(System.Data.DataTable table)
        {
            CommonFunctions functions = new CommonFunctions();
            string sql = "";
            int counter = 0;
            BatchSizeCompletedEventArgs eventArgs = new BatchSizeCompletedEventArgs();
            eventArgs.ErrorDataRows = new List<System.Data.DataRow>();
            foreach (System.Data.DataRow item in table.Rows)
            {
                try
                {
                    // SQL constructed. 
                    // Using the destination connection Execute the statement
                    sql = functions.ConstructSql(DestinationTableName, item, ColumnMapItems);
                    Console.WriteLine(sql);
                    MySqlCommand command = new MySqlCommand(sql, DestinationDbConnection);
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    eventArgs.ErrorDataRows.Add(item);
                }
                
                counter++;

                /*
                 * Issue 2:	Does not support Batch sizes like in SqlBulkCopy
                 * When the bulkupload code uploads the batch size that is specified by the caller, we need to notify the caller that
                 * The batch size is done uploading. This will help the caller to make their decisions.
                 */
                if (counter == BatchSize && counter > 0)
                {
                    // batch size is completed. invoke the OnBatchSizeCompletedDelegate to alert the caller
                    if (OnBatchSizeCompleted != null)
                    {
                        // create the event arguments
                        
                        eventArgs.CompletedRows = counter.ToString();
                        // invoke the delegate
                        OnBatchSizeCompleted(eventArgs);
                    }
                    eventArgs.CompletedRows = "";
                    eventArgs.ErrorDataRows.Clear();
                    counter = 0;
                }
            }

            // A final raise from the code. this is to catch the arbitary values that does not meet the batch size limit
            if (counter > 0)
            {
                // batch size is completed. invoke the OnBatchSizeCompletedDelegate to alert the caller
                if (OnBatchSizeCompleted != null)
                {
                    // create the event arguments
                    eventArgs.CompletedRows = counter.ToString();
                    // invoke the delegate
                    OnBatchSizeCompleted(eventArgs);
                }
            }
        }


    }
}
