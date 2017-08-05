using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Changeling
{
    public class DbConn
    {

        public static void database_create()
        {
            SQLiteConnection sqlite;
            if (File.Exists("db.sqlite"))
            {
                sqlite = new SQLiteConnection("Data Source=db.sqlite");
            }
            else
            {
                SQLiteConnection.CreateFile("db.sqlite");
                sqlite = new SQLiteConnection("Data Source=db.sqlite");
                sqlite.Open();
                string sql_log = "CREATE TABLE watchList (id INTEGER PRIMARY KEY, folders VARCHAR(100), timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";
                string sql_log1 = "CREATE TABLE changes (id INTEGER PRIMARY KEY, folders VARCHAR(100), changes VARCHAR(100), timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";
                SQLiteCommand command = new SQLiteCommand(sql_log, sqlite);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                SQLiteCommand command1 = new SQLiteCommand(sql_log1, sqlite);
                try
                {
                    command1.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                sqlite.Close();
            }
        }
        //loops through the posted array folder and writes the dragged file's path to database
        public static void database_watchList_fill(string[] folders)
        {
            SQLiteConnection sqlite;
            sqlite = new SQLiteConnection("Data Source=db.sqlite");
            sqlite.Open();
            SQLiteCommand insertSQL_logging = new SQLiteCommand("INSERT INTO watchList (folders) VALUES (@folders)", sqlite);
            for (int i = 0; i < folders.Length; i++)
            {
                Console.WriteLine(folders[i]);
                insertSQL_logging.Parameters.Add(new SQLiteParameter("@folders", folders[i]));
                try
                {
                    insertSQL_logging.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            sqlite.Close();
        }
        public static void database_watchList_changes(string affectedFile, string change)
        {
            SQLiteConnection sqlite;
            sqlite = new SQLiteConnection("Data Source=db.sqlite");
            sqlite.Open();
            SQLiteCommand insertSQL_logging = new SQLiteCommand("INSERT INTO changes (folders, changes) VALUES (@folders, @changes)", sqlite);
            insertSQL_logging.Parameters.Add(new SQLiteParameter("@folders", affectedFile));
            insertSQL_logging.Parameters.Add(new SQLiteParameter("@changes", change));
            try
            {
                insertSQL_logging.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            sqlite.Close();
        }
        public static void database_watchList_return_folders()
        {
            SQLiteConnection sqlite;
            sqlite = new SQLiteConnection("Data Source=db.sqlite");
            sqlite.Open();
            SQLiteCommand returnFoldersSQL = new SQLiteCommand("SELECT DISTINCT folders FROM watchList", sqlite);
            SQLiteDataReader sqReader = returnFoldersSQL.ExecuteReader();
            List<string> return_folders = new List<string>();
            try
            {
                sqReader.Read();
                for (int i = 0; i < sqReader.StepCount; i++)
                {
                    return_folders.Add(sqReader.GetString(i).ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            sqReader.Close();
            sqlite.Close();
        }
        public static void database_watchList_return_changes(string affectedFile, string change)
        {
            SQLiteConnection sqlite;
            sqlite = new SQLiteConnection("Data Source=db.sqlite");
            sqlite.Open();
            SQLiteCommand insertSQL_logging = new SQLiteCommand("INSERT INTO changes (folders, changes) VALUES (@folders, @changes)", sqlite);
            insertSQL_logging.Parameters.Add(new SQLiteParameter("@folders", affectedFile));
            insertSQL_logging.Parameters.Add(new SQLiteParameter("@changes", change));
            try
            {
                insertSQL_logging.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            sqlite.Close();
        }
        public static void database_watchlist_remove_folders(string folder)
        {
            SQLiteConnection sqlite;
            sqlite = new SQLiteConnection("Data Source=db.sqlite");
            sqlite.Open();
            SQLiteCommand remove_folder = new SQLiteCommand("DELETE FROM watchList WHERE folders = @folders", sqlite);
            remove_folder.Parameters.Add(new SQLiteParameter("@folders", folder));
            try
            {
                remove_folder.ExecuteNonQuery();
                Console.WriteLine("Successfully removed the following folder from the watchlist" + folder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            sqlite.Close();
        }
        // public static void database_notification(string email)
        //  {
        //  SQLiteConnection sqlite;
        //  sqlite = new SQLiteConnection("Data Source=db.sqlite");
        //  sqlite.Open();
        // SQLiteCommand remove_folder = new SQLiteCommand("INSERT INTO email (email) VALUES (@email)", sqlite);
        // insertSQL_email.Parameters.Add(new SQLiteParameter("@email", email));
        //     try
        //     {
        //  insertSQL_email.ExecuteNonQuery();
        // }
        //     catch (Exception ex)
        //    {
        //         throw new Exception(ex.Message);
        //   }
        //   sqlite.Close();
        //   }

    }

}