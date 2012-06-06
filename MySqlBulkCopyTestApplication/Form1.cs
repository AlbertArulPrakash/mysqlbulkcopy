using System;
using System.Data;
using System.Windows.Forms;
using IndiansInc;
using IndiansInc.Internals;

namespace MySqlBulkCopyTestApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection("Server=localhost;Port=3306;Database=destination;Uid=root;Pwd=12345;");
            MySql.Data.MySqlClient.MySqlConnection sourceConnection = new MySql.Data.MySqlClient.MySqlConnection("Server=localhost;Port=3306;Database=users;Uid=root;Pwd=12345;");
            try
            {
                connection.Open();
                sourceConnection.Open();
                DataTable table = new DataTable();
                MySqlBulkCopy upload = new MySqlBulkCopy();
                upload.DestinationTableName = "session";
                ColumnMapItemCollection collection = new ColumnMapItemCollection();
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

                collection.Add(sessionId);
                collection.Add(userId);
                collection.Add(dateLogged);
                collection.Add(loggedFrom);
                collection.Add(active);

                upload.ColumnMapItems = collection;
                upload.DestinationDbConnection = connection;

                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("select idsession,userid,datelogged,loggedfrom,active from session", sourceConnection);
                MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();
                upload.Upload(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
            connection.Dispose();
            sourceConnection.Close();
            sourceConnection.Dispose();
        }
    }
}
