using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FUT
{
    class Program
    {
        static FUTEntities db = new FUTEntities();

        static void Main(string[] args)
        {
            int currentChemistry = 0;
            for(int i = 0; i < 1000000000; i++)
            {
                var team = GenerateTeam("3-4-1-2");
                int Chemistry = CheckChemistry("3-4-1-2", team);
                if(Chemistry > currentChemistry)
                {
                    Console.WriteLine("Found new highest chem value: " + Chemistry);
                    currentChemistry = Chemistry;
                }
                
                if(Chemistry == 33)
                {
                    Console.WriteLine("Found perfect chem");
                }
            }
        }

        public void ImportPlayers()
        {
            int pageCount = GetPageCount();
            List<Items> masterList = new List<Items>();
            for (int i = 544; i < pageCount + 1; i++)
            {
                List<Items> players = GetJsonString(i);
                foreach (var player in players)
                {
                    Item item = new Item();
                    item.club = player.club.name;
                    item.firstName = player.firstName;
                    item.itemId = player.BaseId;
                    item.lastName = player.lastName;
                    item.league = player.league.name;
                    item.nation = player.nation.name;
                    item.position = player.position;
                    db.Items.Add(item);
                    db.SaveChanges();
                    masterList.Add(player);
                }
            }
        }

        public static int GetPageCount()
        {
            var pageCount = new { pageCount = 0 };
            var json = new WebClient().DownloadString("https://www.easports.com/fifa/ultimate-team/api/fut/item?page=1");
            PageInfo info = JsonConvert.DeserializeObject<PageInfo>(json);

            return info.totalPages;
        }

        public static List<Items> GetJsonString(int page)
        {
            return GetItems(new WebClient().DownloadString("https://www.easports.com/fifa/ultimate-team/api/fut/item?page=" + page));
        }

        public static List<Items> GetItems(string json)
        {
            PageInfo info = JsonConvert.DeserializeObject<PageInfo>(json);
            List<Items> players = new List<Items>();
            foreach(var item in info.items)
            {
                players.Add(item);
            }

            return players;
        }

        public class PageInfo
        {
            public int page { get; set; }
            public int totalPages { get; set; }
            public int totalResults { get; set; }
            public string type { get; set; }
            public int count { get; set; }
            public List<Items> items { get; set; }
        }

        public class BaseItem
        {
            public string name { get; set; }
        }

        public class League : BaseItem
        {
            
        }

        public class Club : BaseItem
        {

        }

        public class Nation : BaseItem
        {

        }

        public class Items
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public League league { get; set; }
            public Club club { get; set; }
            public Nation nation { get; set; }
            public int BaseId { get; set; }
            public string position { get; set; }
        }

        public class Team
        {
            public List<Item> players { get; set; }
        }

        public static Team GenerateTeam(string formation)
        {
            Team newTeam = new Team();
            newTeam.players = new List<Item>();
            if(formation == "3-4-1-2")
            {
                // ST : 3 - ST, 2 - CF, LF / RF - 1
                newTeam.players.Add(db.Items.Where(u => u.position == "ST" || u.position == "CF" || u.position == "LF" || u.position == "RF").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // ST : 3 - ST, 2 - CF, LF / RF - 1
                newTeam.players.Add(db.Items.Where(u => u.position == "ST" || u.position == "CF" || u.position == "LF" || u.position == "RF").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // LM : 3 - LM, 2 - LW, 1 - LF, CM, LWB, LB, RM
                newTeam.players.Add(db.Items.Where(u => u.position == "LM" || u.position == "LW" || u.position == "LF" || u.position == "CM" || u.position == "LWB" || u.position == "LB" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CAM : 3 - CAM, 2 - CF, CM, 1 - CDM
                newTeam.players.Add(db.Items.Where(u => u.position == "CAM" || u.position == "CF" || u.position == "CM" || u.position == "CDM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // RM : 3 - RM, 2 - RW, 1 - RF, CM, RWB, RB, LM
                newTeam.players.Add(db.Items.Where(u => u.position == "RM" || u.position == "RW" || u.position == "RF" || u.position == "CM" || u.position == "RWB" || u.position == "RB" || u.position == "LM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                newTeam.players.Add(db.Items.Where(u => u.position == "CM" || u.position == "CAM" || u.position == "CDM" || u.position == "LM" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                newTeam.players.Add(db.Items.Where(u => u.position == "CM" || u.position == "CAM" || u.position == "CDM" || u.position == "LM" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(db.Items.Where(u => u.position == "CB" || u.position == "CDM" || u.position == "LB" || u.position == "RB" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(db.Items.Where(u => u.position == "CB" || u.position == "CDM" || u.position == "LB" || u.position == "RB" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(db.Items.Where(u => u.position == "CB" || u.position == "CDM" || u.position == "LB" || u.position == "RB" || u.position == "RM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // GK : 3 - GK
                newTeam.players.Add(db.Items.Where(u => u.position == "CB").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
            }

            return newTeam;
        }

        public static int CheckChemistry(string formation, Team team)
        {
            int teamChemistry = 0;

            if (formation == "3-4-1-2")
            {

                // ST : 3 - ST, 2 - CF, LF/RF - 1
                if (team.players[0].position == "ST")
                {
                    teamChemistry += 3;
                }
                else if (team.players[0].position == "CF")
                {
                    teamChemistry += 2;
                }
                else if (team.players[0].position == "LF" || team.players[0].position == "RF")
                {
                    teamChemistry += 1;
                }

                // ST : 3 - ST, 2 - CF, LF/RF - 1
                if (team.players[1].position == "ST")
                {
                    teamChemistry += 3;
                }
                else if (team.players[1].position == "CF")
                {
                    teamChemistry += 2;
                }
                else if (team.players[1].position == "LF" || team.players[1].position == "RF")
                {
                    teamChemistry += 1;
                }

                // LM : 3 - LM, 2 - LW, 1 - LF, CM, LWB, LB, RM
                if (team.players[2].position == "LM")
                {
                    teamChemistry += 3;
                }
                else if (team.players[2].position == "LW")
                {
                    teamChemistry += 2;
                }
                else if (team.players[2].position == "LF" || team.players[2].position == "CM" || team.players[2].position == "LWB" || team.players[2].position == "LB" || team.players[2].position == "RM")
                {
                    teamChemistry += 1;
                }

                // CAM : 3 - CAM, 2 - CF, CM, 1 - CDM
                if (team.players[3].position == "CAM")
                {
                    teamChemistry += 3;
                }
                else if (team.players[3].position == "CF" || team.players[3].position == "CM")
                {
                    teamChemistry += 2;
                }
                else if (team.players[3].position == "CDM")
                {
                    teamChemistry += 1;
                }

                // RM : 3 - RM, 2 - RW, 1 - RF, CM, RWB, RB, LM
                if (team.players[4].position == "RM")
                {
                    teamChemistry += 3;
                }
                else if (team.players[4].position == "RW")
                {
                    teamChemistry += 2;
                }
                else if (team.players[4].position == "RF" || team.players[4].position == "CM" || team.players[4].position == "RWB" || team.players[4].position == "RB" || team.players[4].position == "LM")
                {
                    teamChemistry += 1;
                }

                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                if (team.players[5].position == "CM")
                {
                    teamChemistry += 3;
                }
                else if (team.players[5].position == "CAM" || team.players[5].position == "CDM")
                {
                    teamChemistry += 2;
                }
                else if (team.players[5].position == "LM" || team.players[5].position == "RM")
                {
                    teamChemistry += 1;
                }
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                if (team.players[6].position == "CM")
                {
                    teamChemistry += 3;
                }
                else if (team.players[6].position == "CAM" || team.players[6].position == "CDM")
                {
                    teamChemistry += 2;
                }
                else if (team.players[6].position == "LM" || team.players[6].position == "RM")
                {
                    teamChemistry += 1;
                }

                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[7].position == "CB")
                {
                    teamChemistry += 3;
                }
                else if (team.players[7].position == "CDM" || team.players[7].position == "LB" || team.players[7].position == "RB")
                {
                    teamChemistry += 1;
                }

                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[8].position == "CB")
                {
                    teamChemistry += 3;
                }
                else if (team.players[8].position == "CDM" || team.players[8].position == "LB" || team.players[8].position == "RB")
                {
                    teamChemistry += 1;
                }

                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[9].position == "CB")
                {
                    teamChemistry += 3;
                }
                else if (team.players[9].position == "CDM" || team.players[9].position == "LB" || team.players[9].position == "RB")
                {
                    teamChemistry += 1;
                }

                // GK : 3 - GK
                if (team.players[10].position == "GK")
                {
                    teamChemistry += 3;
                }
                
            }

            return teamChemistry;
        }
    }
}
