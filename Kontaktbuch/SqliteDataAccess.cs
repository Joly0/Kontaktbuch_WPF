using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Kontaktbuch
{
    public class SqliteDataAccess
    {
        public static List<PersonModel> LoadPeople()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<PersonModel>("select * from Person", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SavePerson(PersonModel person)
        {
            IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
            if (person.Id != -1)
            {
                cnn.ExecuteAsync("insert or replace into Person (Id, VorName, NachName, Straße, Hausnummer, Postleitzahl, Ort, LieblingsLehrer, LieblingsDino) values (@Id, @VorName, @NachName, @Straße, @Hausnummer, @Postleitzahl, @Ort, @LieblingsLehrer, @LieblingsDino)", person);
            }
            else
            {
                cnn.ExecuteAsync("insert or replace into Person (VorName, NachName, Straße, Hausnummer, Postleitzahl, Ort, LieblingsLehrer, LieblingsDino) values (@VorName, @NachName, @Straße, @Hausnummer, @Postleitzahl, @Ort, @LieblingsLehrer, @LieblingsDino)", person);
            }
        }

        public static void DeletePerson(PersonModel person)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.ExecuteAsync("DELETE FROM Person WHERE Id=@Id", person);
            }
        }

        private static string LoadConnectionString()
        {
            string currentConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString.ToString();
            string removeString = ".";
            currentConnectionString = currentConnectionString.Remove(currentConnectionString.IndexOf(removeString), removeString.Length);
            string kontaktbuchConnectionString = currentConnectionString.Insert(12, Directory.GetCurrentDirectory().ToString());

            return kontaktbuchConnectionString;
        }
    }
}
