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
     * ColumnMapItemCollection
     * This class represents the collection of all ColumnMap information between source and destination columns
     * @author   Albert Arul Prakash<albertarulprakash@gmail.com>  
     */

    /*
     * Version 0.1: Has basic collection methods
     * Version 0.2: Added Replace method 
     */


    using System;
    using System.Collections;
    using System.Linq;

    public class ColumnMapItemCollection : CollectionBase
    {
        /// <summary>
        /// Method to retrieve a <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> from <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso>.
        /// </summary>
        /// <param name="index">item position in the collection</param>
        /// <returns>ColumnMapItem if present in the position</returns>
        /// <exception cref="System.IndexOutOfRangeException">Position provided is not within the <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso> range</exception>
        
        public ColumnMapItem Item(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("index");
            }
            if (index > Count - 1)
            {
                throw new IndexOutOfRangeException("index");
            }
            return (ColumnMapItem)List[index];
        }

        /// <summary>
        /// Method to add new <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> into the <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso>.
        /// </summary>
        /// <param name="item">Item that to be added to the collection</param>
        /// <exception cref="System.ArgumentNullException">Item is null</exception>
        
        public void Add(ColumnMapItem item)
        {
            // We need to do one more implementation on adding the item.
            // we need to verify whether the destination column is already present.
            // If it is present then we should not add this because no destination can have two source

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ColumnMapItem existing = Find(item.DestinationColumn);
            if (existing != null)
            {
                if (existing.DestinationColumn.ToUpper() == item.DestinationColumn.ToUpper())
                {
                    throw new DuplicateColumnMappingException("Duplicate destination column found");
                }
            }

            List.Add(item);
        }

        /// <summary>
        /// Method to reemove <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> from <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso>
        /// </summary>
        /// <param name="index">item position that to be removed</param>
        /// <exception cref="System.IndexOutOfRangeException">Occurs if the provided index is not within <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso> range</exception>
        /// 

        public void Remove(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("index");
            }
            if (index > Count - 1)
            {
                throw new IndexOutOfRangeException("index");
            }
            List.RemoveAt(index);
        }

        /// <summary>
        /// Method to find a <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> in the <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItem Collection</seealso>.
        /// </summary>
        /// <param name="destinationColumnName">The search destination column value</param>
        /// <returns>ColumnMapitem if found else null</returns>
        
        public ColumnMapItem Find(string destinationColumnName)
        {
            // Find a columnMap in the collection
            return List.Cast<ColumnMapItem>().FirstOrDefault(
                item => item.DestinationColumn.ToUpper() == destinationColumnName.ToUpper()
                );
        }

        /// <summary>
        /// replaces an existing <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> with a new <seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> element
        /// </summary>
        /// <param name="index">Item position in the <seealso cref="IndiansInc.Internals.ColumnMapItemCollection">ColumnMapItemCollection</seealso></param>
        /// <param name="item"><seealso cref="IndiansInc.Internals.ColumnMapItem">ColumnMapItem</seealso> item</param>
        /// <exception cref="System.IndexOutOfRangeException">if index does not contain in the range</exception>
        public void Replace(int index, ColumnMapItem item)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("index");
            }
            if (index > Count - 1)
            {
                throw new IndexOutOfRangeException("index");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (index == Count - 1)
            {
                List.RemoveAt(index);
                List.Add(item);
            }
            else
            {
                List.RemoveAt(index);
                if (index > Count)
                {
                    List.Add(item);
                }
                else
                {
                    List.Insert(index, item);
                }
            }

        }
    }
}
