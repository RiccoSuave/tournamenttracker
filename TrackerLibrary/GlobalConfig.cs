using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizesModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamFile = "TeamModels.csv";
        public const string TournamentFile = "TournamentFile.csv";
        public const string MatchupFile = "MatchupModels.csv";
        public const string MatchupEntriesFile = "MatchupEntryModel.csv";
        public static IDataConnection Connection { get; private set; }
        
        public static void InitializeConnections (DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                
                SqlConnector sql = new SqlConnector();
                //Connections.Add(sql);
                Connection = sql;
            }
             else if (db == DatabaseType.TextFile)
            {
                
                TextConnector text = new TextConnector();
                //Connections.Add(text);
                Connection = text;
            }
            else if (db == DatabaseType.All)
            {
                TextConnector text = new TextConnector();
                Connection = text;
                SqlConnector sql = new SqlConnector();
                Connection = sql;
            }
        }
        public static string CnnString (string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
