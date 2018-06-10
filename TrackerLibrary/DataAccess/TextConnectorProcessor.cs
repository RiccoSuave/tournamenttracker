﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {

        //public static string FullFilePath(this string fileName)
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"]}\\{ fileName }";
        }
        public static List<string> LoadFile(this string filename)
        {
            if (!File.Exists(filename))
            {
                return new List<string>();
            }
            return File.ReadAllLines(filename).ToList();
        }
        public static List<PrizeModel> ConverToPrizeModels(this  List<string> lines)
        {
            List < PrizeModel > output = new List<PrizeModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
                
            }
            return output;
        }
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);
            }
            return output;
        }
        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            //id=0, TeamCompeting=1, Score=2, ParentMatchup=3
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel me = new MatchupEntryModel();
                me.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    me.TeamCompeting = null;
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }
                
                me.Score = double.Parse(cols[2]);
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }
                output.Add(me);
            }
            return output;
        }
        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();
            foreach (string id in ids)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');
                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
                output = matchingEntries.ConvertToMatchupEntryModels();
            }
            return output;
        }
        private static TeamModel LookupTeamById (int id)
        {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();
            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchinTeams = new List<string>();
                    matchinTeams.Add(team);
                    return matchinTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }
            return null;
        }
        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();
            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConverToMatchupModels().First();
                }
            }
            return null;
        }
        public static List<TeamModel> ConvertToTeamModels (this List<string> lines, string peopleFileName)
        //public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string fileName)
        {
            // id, team name, list of ids separated by the pipe 
            // 3, Tim's Team, 1|3|5
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();
            //List<PersonModel> people = fileName.FullFilePath().LoadFile().ConvertToPersonModels();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];
                string[] personIds = cols[2].Split('|');
                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }
        public static List <TournamentModel> ConvertToTournamentModels (this List<string> lines, string TeamFileName, string PeopleFileName, string PrizeFileName)
        {
            //As always we plan in advance what the file would look like
            //id, TournamentName, EntryFee, (id|id|id - Entered Teams), (id|id|id - prizes ), (Rounds id^id^id|id^id^id|id^id^id)
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = TeamFileName.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFileName);
            List<PrizeModel> prizes = PrizeFileName.FullFilePath().LoadFile().ConverToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConverToMatchupModels();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel tm = new TournamentModel();
                tm.id = int.Parse(cols[0]);
                //tm.Id = cols[0];  // error: can not implicitly convert string to int
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);
                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }
                if (cols[4].Length > 0)
                { 
                    string[] prizeIds = cols[4].Split('|');
                    foreach (string id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    }
                }
                // Capture the rounds information
                string [] rounds = cols[5].Split('|');
                List<MatchupModel> ms = new List<MatchupModel>();
                foreach (string round in rounds)
                {
                    string[] msText = round.Split('^');
                    foreach (string matchupModelTextId in msText)
                    {
                        ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }
                    tm.Rounds.Add(ms);
                }
                output.Add(tm);
            }
            return output;
        }
        public static void SaveToPrizeFile(this List<PrizeModel>models,string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveToPeopleFile (this List<PersonModel> models,string fileName)
        {
            List<string> lines = new List<string>();
            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellPhoneNumber }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id }, { t.TeamName }, {ConvertPeopleListToString(t.TeamMembers)}");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            // loop through each round 
            // loop through each matchup 
            // Get the id for the new matchup and save the record 
            // loop through each entry, get the id, and save it 
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    // load all of the matchups from file
                    // Get the top id and add one 
                    // Store the id 
                    // Save the matchup record 
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
     
                }
            }
        }

        public static List<MatchupModel> ConverToMatchupModels(this List<string> lines)
        {  // id=0, entries=1(pipe delimited by id), winner=2, matchupRounds=3
            List<MatchupModel> output = new List<MatchupModel>();
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel p = new MatchupModel();
                p.Id = int.Parse(cols[0]);
                p.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                if (cols[2].Length == 0)
                { p.Winner = null; }
                else
                {
                    p.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                
                p.MatchupRound = int.Parse(cols[3]);
                output.Add(p);

            }
            return output;
        }
        public static void SaveMatchupToFile (this MatchupModel matchup, string matchupFile, string matchupEntryFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath()
            .LoadFile().ConverToMatchupModels();
            int currentId = 1;
            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId;
            matchups.Add(matchup); 

            //List<string> lines = new List<string>();
            //// id=0, entries=1(pipe delimited by id), winner=2, matchupRounds=3
            //foreach (MatchupModel m in matchups)
            //{
            //    string winner = "";
            //    if (m.Winner != null)
            //    {
            //        winner = m.Winner.Id.ToString();
            //    }
            //    lines.Add($"{ m.Id },{ "" },{ m.Winner.Id },{ m.MatchupRound }");
            //}
            //File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile(matchupEntryFile);
            }
            List<string> lines = new List<string>();
            // id=0, entries=1(pipe delimited by id), winner=2, matchupRounds=3
            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ m.Winner.Id },{ m.MatchupRound }");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
             
        }
        
        public static void SaveEntryToFile (this MatchupEntryModel entry, string matchupEntryFile)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
            int currentId = 1;
            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }
            entry.Id = currentId;
            entries.Add(entry);
            List<string> lines = new List<string>();
            //id=0, TeamCompeting=1, Score=2, ParentMatchup=3
            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ e.ParentMatchup.Id }");
            }
            // TODO -- You may have an issue with the line below. Tim's file was GlobalConfig.MatchupEntryFile
            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }
        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName) {
            //id = 0
            // TournamentName = 1
            // EntryFee = 2 
            // EnteredTeams = 3 
            // Prizes = 4
            // Rounds = 5 
            // Rounds id^id^id|id^id^id|id^id^id
            List<string> lines = new List<string>();
            foreach (TournamentModel tm in models)
            {
                lines.Add($"{ tm.id },{ tm.TournamentName },{ tm.EntryFee },{ ConvertTeamListToString(tm.EnteredTeams) },{ ConvertPrizeListToString(tm.Prizes) },{ ConvertRoundListToString(tm.Rounds) }");
            }
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";
            if (rounds.Count == 0) { return ""; }
            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToString(r)}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";
            if (matchups.Count == 0) { return ""; }
            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}^";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";
            if (entries.Count == 0) { return ""; }
            foreach (MatchupEntryModel e in entries)
            {
                output += $"{e.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";
            if (prizes.Count == 0) { return ""; }
            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertTeamListToString (List<TeamModel> teams)
        {
            string output = "";
            if (teams.Count == 0) { return ""; }
            foreach (TeamModel p in teams)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertPeopleListToString (List<PersonModel> people)
        {
            string output = "";
            if (people.Count == 0) { return ""; }
            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
    }
 
}
