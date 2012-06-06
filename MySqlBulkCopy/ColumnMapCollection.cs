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


namespace IndiansInc.Internals
{
/*
 * ColumnMapItemCollection
 * This class represents the collection of all ColumnMap information between source and destination columns
 * @author   Albert Arul Prakash<albertarulprakash@gmail.com>  
 */

    
    using System;
    using System.Collections;
    using System.Linq;

    public class ColumnMapItemCollection : CollectionBase
    {
        public ColumnMapItem Item(int index)
        {
            if (index < 0) { throw new IndexOutOfRangeException("index"); }
            if (index > Count - 1) { throw new IndexOutOfRangeException("index"); }
            return (ColumnMapItem)List[index];
        }

        public void Add(ColumnMapItem item)
        {
            // We need to do one more implementation on adding the item.
            // we need to verify whether the destination column is already present.
            // If it is present then we should not add this because no destination can have two source

            if (item == null) { throw new ArgumentNullException("item"); }
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

        public void Remove(int index)
        {
            if (index < 0) { throw new IndexOutOfRangeException("index"); }
            if (index > Count - 1) { throw new IndexOutOfRangeException("index"); }
            List.RemoveAt(index);
        }

        public ColumnMapItem Find(string destinationColumnName)
        {
            // Find a columnMap in the collection
            return List.Cast<ColumnMapItem>().FirstOrDefault(item => item.DestinationColumn.ToUpper() == destinationColumnName.ToUpper());
        }
    }
}
