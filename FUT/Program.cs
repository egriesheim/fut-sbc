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
            FindBestSBC();
            //ImportPlayers();
        }

        public static void FindBestSBC()
        {
            int currentChemistry = 0;
            List<Item> playerPool = db.Items.Where(u => u.rating >= 75 && u.rating <= 79 && (u.nation == "Spain" || u.nation == "Germany" || u.nation == "Brazil" || u.nation == "England" || u.nation == "Italy" || u.nation == "Argentina" || u.nation == "France")).ToList();
            for (int i = 0; i < 1000000000; i++)
            {
                var team = GenerateTeam(playerPool, "3-4-1-2");
                int nationCount = team.players.Select(u => u.nation).Distinct().Count();
                int squadRating = (int)team.players.Select(u => u.rating).Sum()/11;
                int rareCount = (int)team.players.Where(u => u.color.Contains("rare")).Count();
                int distinctCount = team.players.Distinct().Count();
                if(distinctCount < 11)
                {
                    continue;
                }
                if(rareCount < 4)
                {
                    continue;
                }
                if(nationCount != 4)
                {
                    continue;
                }
                if(squadRating < 75)
                {
                    continue;
                }
                int Chemistry = CheckChemistry("3-4-1-2", team);
                if (Chemistry > currentChemistry)
                {
                    Console.WriteLine();
                    Console.WriteLine("Found new highest chem value: " + Chemistry);
                    currentChemistry = Chemistry;
                    foreach(var player in team.players)
                    {
                        Console.WriteLine(player.firstName + " " + player.lastName + ", " + player.league + ", " + player.club + ", " + player.nation + ", " + player.rating);
                    }
                }

                if (Chemistry >= 80)
                {
                    Console.WriteLine("Found perfect chem");
                    foreach (var player in team.players)
                    {
                        Console.WriteLine(player.firstName + " " + player.lastName + ", " + player.league + ", " + player.club + ", " + player.nation + ", " + player.rating);
                    }
                }
            }
        }

        public static void ImportPlayers()
        {
            int pageCount = GetPageCount();
            List<Items> masterList = new List<Items>();
            int playerCount = 0;
            for (int i = 1; i < pageCount + 1; i++)
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
                    item.rating = player.rating;
                    item.quality = player.quality;
                    item.color = player.color;
                    db.Items.Add(item);
                    db.SaveChanges();
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
            foreach (var item in info.items)
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
            public int rating { get; set; }
            public string quality { get; set; }
            public string color { get; set; }
        }

        public class Team
        {
            public List<Item> players { get; set; }
        }

        public static Team GenerateTeam(List<Item> playerPool, string formation)
        {
            Team newTeam = new Team();
            newTeam.players = new List<Item>();
            if (formation == "3-4-1-2")
            {
                // ST : 3 - ST, 2 - CF, LF / RF - 1
                newTeam.players.Add(playerPool.Where(u => u.position == "ST" || u.position == "CF").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // ST : 3 - ST, 2 - CF, LF / RF - 1
                newTeam.players.Add(playerPool.Where(u => u.position == "ST" || u.position == "CF").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // LM : 3 - LM, 2 - LW, 1 - LF, CM, LWB, LB, RM
                newTeam.players.Add(playerPool.Where(u => u.position == "LM" || u.position == "LW").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CAM : 3 - CAM, 2 - CF, CM, 1 - CDM
                newTeam.players.Add(playerPool.Where(u => u.position == "CAM" || u.position == "CF" || u.position == "CM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // RM : 3 - RM, 2 - RW, 1 - RF, CM, RWB, RB, LM
                newTeam.players.Add(playerPool.Where(u => u.position == "RM" || u.position == "RW").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                newTeam.players.Add(playerPool.Where(u => u.position == "CM" || u.position == "CAM" || u.position == "CDM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                newTeam.players.Add(playerPool.Where(u => u.position == "CM" || u.position == "CAM" || u.position == "CDM").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(playerPool.Where(u => u.position == "CB").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(playerPool.Where(u => u.position == "CB").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // CB : 3 - CB, 1 - CDM, LB, RB
                newTeam.players.Add(playerPool.Where(u => u.position == "CB").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
                // GK : 3 - GK
                newTeam.players.Add(playerPool.Where(u => u.position == "GK").OrderBy(r => Guid.NewGuid()).FirstOrDefault());
            }

            return newTeam;
        }

        public static int CheckChemistry(string formation, Team team)
        {
            int teamChemistry = 0;

            if (formation == "3-4-1-2")
            {
                var link1 = CalculateChemistry(team.players[0], team.players[1], team.players[2], team.players[3]);
                // ST : 3 - ST, 2 - CF, LF/RF - 1
                if (team.players[0].position == "ST")
                {
                    teamChemistry += GetChemCoefficient(3, link1);
                }
                else if (team.players[0].position == "CF")
                {
                    teamChemistry += GetChemCoefficient(2, link1);
                }
                else if (team.players[0].position == "LF" || team.players[0].position == "RF")
                {
                    teamChemistry += GetChemCoefficient(1, link1);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link1);
                }

                // 3 1,2,3

                var link2 = CalculateChemistry(team.players[1], team.players[0], team.players[3], team.players[4]);
                // ST : 3 - ST, 2 - CF, LF/RF - 1
                if (team.players[1].position == "ST")
                {
                    teamChemistry += GetChemCoefficient(3, link2);
                }
                else if (team.players[1].position == "CF")
                {
                    teamChemistry += GetChemCoefficient(2, link2);
                }
                else if (team.players[1].position == "LF" || team.players[1].position == "RF")
                {
                    teamChemistry += GetChemCoefficient(1, link2);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link2);
                }

                // 3 0,3,4

                var link3 = CalculateChemistry(team.players[2], team.players[0], team.players[5], team.players[7]);
                // LM : 3 - LM, 2 - LW, 1 - LF, CM, LWB, LB, RM
                if (team.players[2].position == "LM")
                {
                    teamChemistry += GetChemCoefficient(3, link3);
                }
                else if (team.players[2].position == "LW")
                {
                    teamChemistry += GetChemCoefficient(2, link3);
                }
                else if (team.players[2].position == "LF" || team.players[2].position == "CM" || team.players[2].position == "LWB" || team.players[2].position == "LB" || team.players[2].position == "RM")
                {
                    teamChemistry += GetChemCoefficient(1, link3);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link3);
                }

                // 3 0,5,7

                var link4 = CalculateChemistry(team.players[3], team.players[0], team.players[1], team.players[5], team.players[6]);
                // CAM : 3 - CAM, 2 - CF, CM, 1 - CDM
                if (team.players[3].position == "CAM")
                {
                    teamChemistry += GetChemCoefficient(3, link4);
                }
                else if (team.players[3].position == "CF" || team.players[3].position == "CM")
                {
                    teamChemistry += GetChemCoefficient(2, link4);
                }
                else if (team.players[3].position == "CDM")
                {
                    teamChemistry += GetChemCoefficient(1, link4);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link4);
                }

                // 4 0,1,5,6

                var link5 = CalculateChemistry(team.players[4], team.players[1], team.players[6], team.players[9]);
                // RM : 3 - RM, 2 - RW, 1 - RF, CM, RWB, RB, LM
                if (team.players[4].position == "RM")
                {
                    teamChemistry += GetChemCoefficient(3, link5);
                }
                else if (team.players[4].position == "RW")
                {
                    teamChemistry += GetChemCoefficient(2, link5);
                }
                else if (team.players[4].position == "RF" || team.players[4].position == "CM" || team.players[4].position == "RWB" || team.players[4].position == "RB" || team.players[4].position == "LM")
                {
                    teamChemistry += GetChemCoefficient(1, link5);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link5);
                }

                // 3 1,6,9

                var link6 = CalculateChemistry(team.players[5], team.players[2], team.players[3], team.players[6], team.players[8]);
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                if (team.players[5].position == "CM")
                {
                    teamChemistry += GetChemCoefficient(3, link6);
                }
                else if (team.players[5].position == "CAM" || team.players[5].position == "CDM")
                {
                    teamChemistry += GetChemCoefficient(2, link6);
                }
                else if (team.players[5].position == "LM" || team.players[5].position == "RM")
                {
                    teamChemistry += GetChemCoefficient(1, link6);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link6);
                }

                // 4 2,3,6,8

                var link7 = CalculateChemistry(team.players[6], team.players[3], team.players[4], team.players[5], team.players[8]);
                // CM : 3 - CM, 2 - CAM, CDM, 1 - LM, RM
                if (team.players[6].position == "CM")
                {
                    teamChemistry += GetChemCoefficient(3, link7);
                }
                else if (team.players[6].position == "CAM" || team.players[6].position == "CDM")
                {
                    teamChemistry += GetChemCoefficient(2, link7);
                }
                else if (team.players[6].position == "LM" || team.players[6].position == "RM")
                {
                    teamChemistry += GetChemCoefficient(1, link7);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link7);
                }

                // 4 3,4,5,7

                var link8 = CalculateChemistry(team.players[7], team.players[2], team.players[8], team.players[10]);
                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[7].position == "CB")
                {
                    teamChemistry += GetChemCoefficient(3, link8);
                }
                else if (team.players[7].position == "CDM" || team.players[7].position == "LB" || team.players[7].position == "RB")
                {
                    teamChemistry += GetChemCoefficient(1, link8);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link8);
                }

                // 3 2,8,10

                var link9 = CalculateChemistry(team.players[8], team.players[5], team.players[6], team.players[7], team.players[9], team.players[10]);
                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[8].position == "CB")
                {
                    teamChemistry += GetChemCoefficient(3, link9);
                }
                else if (team.players[8].position == "CDM" || team.players[8].position == "LB" || team.players[8].position == "RB")
                {
                    teamChemistry += GetChemCoefficient(1, link9);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link9);
                }

                // 5 5,6,7,9,10

                var link10 = CalculateChemistry(team.players[9], team.players[4], team.players[8], team.players[10]);
                // CB : 3 - CB, 1 - CDM, LB, RB
                if (team.players[9].position == "CB")
                {
                    teamChemistry += GetChemCoefficient(3, link10);
                }
                else if (team.players[9].position == "CDM" || team.players[9].position == "LB" || team.players[9].position == "RB")
                {
                    teamChemistry += GetChemCoefficient(1, link10);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link10);
                }

                // 3 4,8,10

                var link11 = CalculateChemistry(team.players[10], team.players[7], team.players[8], team.players[9]);
                // GK : 3 - GK
                if (team.players[10].position == "GK")
                {
                    teamChemistry += GetChemCoefficient(3, link11);
                }
                else
                {
                    teamChemistry += GetChemCoefficient(0, link11);
                }

                // 3 7,8,9

            }

            return teamChemistry;
        }

        public static float CalculateChemistry(Item basePlayer, Item player2, Item player3, Item player4)
        {
            int links = 0;
            int chemTotal = 0;
            List<Item> players = new List<Item>();
            players.Add(player2);
            players.Add(player3);
            players.Add(player4);

            foreach (var player in players)
            {
                int chem = 0;
                if (player.club == basePlayer.club)
                {
                    chem++;
                }
                if (player.league == basePlayer.league)
                {
                    chem++;
                }
                if (player.nation == basePlayer.nation)
                {
                    chem++;
                }
                links += chem;
            }
            float chemistry = links / 3;

            return chemistry;
        }

        public static float CalculateChemistry(Item basePlayer, Item player2, Item player3, Item player4, Item player5)
        {
            int links = 0;
            int chemTotal = 0;
            List<Item> players = new List<Item>();
            players.Add(player2);
            players.Add(player3);
            players.Add(player4);
            players.Add(player5);

            foreach (var player in players)
            {
                int chem = 0;
                if (player.club == basePlayer.club)
                {
                    chem++;
                }
                if (player.league == basePlayer.league)
                {
                    chem++;
                }
                if (player.nation == basePlayer.nation)
                {
                    chem++;
                }
                links += chem;
            }
            float chemistry = links / 4;

            return chemistry;
        }

        public static float CalculateChemistry(Item basePlayer, Item player2, Item player3, Item player4, Item player5, Item player6)
        {
            int links = 0;
            int chemTotal = 0;
            List<Item> players = new List<Item>();
            players.Add(player2);
            players.Add(player3);
            players.Add(player4);
            players.Add(player5);
            players.Add(player6);

            foreach (var player in players)
            {
                int chem = 0;
                if (player.club == basePlayer.club)
                {
                    chem++;
                }
                if (player.league == basePlayer.league)
                {
                    chem++;
                }
                if (player.nation == basePlayer.nation)
                {
                    chem++;
                }
                links += chem;
            }
            float chemistry = links / 5;

            return chemistry;
        }

        public static int GetChemCoefficient(int chem, double links)
        {
            if (chem == 3)
            {
                if (links <= 1/3)
                {
                    return 3;
                }
                else if (links < 1)
                {
                    return 6;
                }
                else if (links <= 5/3)
                {
                    return 9;
                }
                else
                {
                    return 10;
                }
            }
            else if (chem == 2)
            {
                if (links <= 1/3)
                {
                    return 2;
                }
                else if (links < 1)
                {
                    return 5;
                }
                else if (links <= 5/3)
                {
                    return 8;
                }
                else
                {
                    return 9;
                }
            }
            else if (chem == 1)
            {
                if (links <= 1/3)
                {
                    return 1;
                }
                else if (links < 1)
                {
                    return 3;
                }
                else if (links <= 5/3)
                {
                    return 5;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                if (links <= 1/3)
                {
                    return 0;
                }
                else if (links < 1)
                {
                    return 1;
                }
                else if (links <= 5/3)
                {
                    return 2;
                }
                else
                {
                    return 2;
                }
            }
        }

    }
}
