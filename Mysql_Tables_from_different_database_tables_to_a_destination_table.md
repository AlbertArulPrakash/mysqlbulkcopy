# Introduction #

In many scenarios, we would be copying data from a backup database that is stored in some cloud storage which is accessible only using an Admin screen. MySqlBulkCopy helps you to restore the data

# How to achieve this #

When you want to copy data from one table to another, you can implement the scenario as below.

**Creating Connection**
```c#

MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection("Server=myProductionServer;Port=3306;Database=destination;Uid=root;Pwd=12345;");
MySql.Data.MySqlClient.MySqlConnection sourceConnection = new MySql.Data.MySqlClient.MySqlConnection("Server=myBackupServer;Port=3306;Database=users;Uid=root;Pwd=12345;");
```

**Mapping columns between tables**
```c#

ColumnMapItem sessionId = new ColumnMapItem();
ColumnMapItem userId = new ColumnMapItem();
ColumnMapItem dateLogged = new ColumnMapItem();
ColumnMapItem loggedFrom = new ColumnMapItem();
ColumnMapItem active = new ColumnMapItem();

sessionId.DataType = "text";
sessionId.DestinationColumn = "IdSession";
sessionId.SourceColumn = "IdSession";

userId.DataType = "int";
userId.DestinationColumn = "userid";
userId.SourceColumn = "userid";

dateLogged.DataType = "datetime";
dateLogged.DestinationColumn = "dateLogged";
dateLogged.SourceColumn = "dateLogged";

loggedFrom.DataType = "text";
loggedFrom.DestinationColumn = "loggedFrom";
loggedFrom.SourceColumn = "loggedFrom";

active.DataType = "int";
active.DestinationColumn = "active";
active.SourceColumn = "active";

ColumnMapItemCollection collection = new ColumnMapItemCollection();
collection.Add(sessionId);
collection.Add(userId);
collection.Add(dateLogged);
collection.Add(loggedFrom);
collection.Add(active);
```
[ColumnMapItem](ColumnMapItem.md) helps you to map the source table columns with the destination table column. Without a column mapping between tables, MySqlBulkCopy will not be able to upload data between tables.

[ColumnMapItemCollection](ColumnMapItemCollection.md) is the container that will group all the [ColumnMapItem](ColumnMapItem.md) information for [MySqlBulkCopy](MySqlBulkCopy.md) to upload the data.

**Calling data Upload**
```c#

MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("select idsession,userid,datelogged,loggedfrom,active from session", sourceConnection);
MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();

MySqlBulkCopy upload = new MySqlBulkCopy();
upload.DestinationTableName = "session";
upload.Upload(reader);
reader.Close();
connection.Close();
connection.Dispose();
sourceConnection.Close();
sourceConnection.Dispose();
```