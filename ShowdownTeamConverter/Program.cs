using Newtonsoft.Json.Linq;
using System;

namespace ShowdownTeamConverer
{
    internal class Program
    {
        //Credits to https://stackoverflow.com/a/4470751
        //Use https://itsjavi.com/koffing/ to convert team to JSON.
        static void Main(string[] args) //TODO: Hidden Power Spreads, proper Abilities, work with Pokeemerald
        {
            dynamic team = null;
            Console.WriteLine("What level do you want the mons to be? Put 0 for the level in the json.");
            var userLevel = Int32.Parse(Console.ReadLine());
            try
            {
                team = JObject.Parse(File.ReadAllText("team.json"));
            }
            catch
            {
                Console.WriteLine("Couldn't find team.json");
                return;
            }
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./conv_team.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open conv_team.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);
            for (int t = 0; t < team.teams.Count; t++)
            {
                dynamic curTeam = team.teams[t];
                for (int p = 0; p < curTeam.pokemon.Count; p++)
                {
                    //I'm not sure if there is a better way to do indentation
                    Console.WriteLine("    {");
                    dynamic mon = curTeam.pokemon[p];
                    string name = mon.name;
                    string item = mon.item;
                    string nature = mon.nature;
                    name = name.ToUpper();
                    name = name.Replace(" ", "_");
                    name = name.Replace("-", "_");
                    name = name.Insert(0, "        .species = SPECIES_");
                    if(name.Contains("_MEGA")) //Not sure if the if statement is needed
                    {
                        name = name.Replace("_MEGA", "");
                    }
                    if (name.Contains("_PRIMAL")) //Not sure if the if statement is needed
                    {
                        name = name.Replace("_PRIMAL", "");
                    }
                    name += ",";
                    item = item.ToUpper();
                    item = item.Replace(" ", "_");
                    item = item.Replace("-", "_");
                    item = item.Insert(0, "        .item = ITEM_");
                    item += ",";
                    nature = nature.ToUpper();
                    nature = nature.Replace(" Nature", "");
                    nature = nature.Replace(" ", "_");
                    nature = nature.Insert(0, "        .nature = NATURE_");
                    nature += ",";

                    Console.WriteLine(name);
                    Console.WriteLine(item);
                    if(mon.ability == 0 || mon.ability == 1 || mon.ability == 2)
                    {
                        switch(mon.ability) //I don't think there is a better way to do this.
                        {
                            case 0:
                                Console.WriteLine("        .ability = FRONTIER_ABILITY_1,");
                                break;
                            case 1:
                                Console.WriteLine("        .ability = FRONTIER_ABILITY_2,");
                                break;
                            case 2:
                                Console.WriteLine("        .ability = FRONTIER_ABILITY_HIDDEN,");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("        .ability = FRONTIER_ABILITY_1, //" + mon.ability + " (Placeholder)"); //Placeholder, since the JSON uses actual ability names and not numbers
                    }
                    Console.WriteLine(nature);
                    if(mon.evs != null)
                    {
                        if (mon.evs.hp != null)
                        {
                            Console.WriteLine("        .hpEv = " + mon.evs.hp + ",");
                        }
                        if (mon.evs.atk != null)
                        {
                            Console.WriteLine("        .atkEv = " + mon.evs.atk + ",");
                        }
                        if (mon.evs.def != null)
                        {
                            Console.WriteLine("        .defEv = " + mon.evs.def + ",");
                        }
                        if (mon.evs.spa != null)
                        {
                            Console.WriteLine("        .spAtkEv = " + mon.evs.spa + ",");
                        }
                        if (mon.evs.spd != null)
                        {
                            Console.WriteLine("        .spDefEv = " + mon.evs.spd + ",");
                        }
                        if (mon.evs.spe != null)
                        {
                            Console.WriteLine("        .spdEv = " + mon.evs.spe + ",");
                        }
                    }
                    if (mon.ivs != null)
                    {
                        if (mon.ivs.hp != null)
                        {
                            Console.WriteLine("        .hpIv = " + mon.ivs.hp + ",");
                        }
                        if (mon.ivs.atk != null)
                        {
                            Console.WriteLine("        .atkIv = " + mon.ivs.atk + ",");
                        }
                        if (mon.ivs.def != null)
                        {
                            Console.WriteLine("        .defIv = " + mon.ivs.def + ",");
                        }
                        if (mon.ivs.spa != null)
                        {
                            Console.WriteLine("        .spAtkIv = " + mon.ivs.spa + ",");
                        }
                        if (mon.ivs.spd != null)
                        {
                            Console.WriteLine("        .spDefIv = " + mon.ivs.spd + ",");
                        }
                        if (mon.ivs.spe != null)
                        {
                            Console.WriteLine("        .spdIv = " + mon.ivs.spe + ",");
                        }
                    }
                    Console.WriteLine("        .moves = ");
                    Console.WriteLine("        {");
                    for(int m = 0; m < mon.moves.Count; m++)
                    {
                        string move = mon.moves[m];
                        move = move.ToUpper();
                        move = move.Replace(" ", "");
                        move = move.Insert(0, "            MOVE_");
                        if(move.Contains("HIDDENPOWER")) //Converts stuff like Hidden Power [FIRE]/[GRASS]/[ICE]
                        {
                            move = "            MOVE_HIDDENPOWER"; //CFRU
                        }
                        else if (move.Contains("HIDDEN_POWER"))
                        {
                            move = "            MOVE_HIDDEN_POWER"; //Decomp
                        }
                        move = move.Replace("-", "");
                        move += ",";
                        Console.WriteLine(move);

                    }
                    Console.WriteLine("        },");
                    Console.WriteLine("        .forSingles = TRUE,"); //?
                    Console.WriteLine("        .forDoubles = TRUE,"); //?
                    Console.WriteLine("        .modifyMovesDoubles = FALSE,"); 
                    if (userLevel == 0)
                    {
                        if(mon.level != null)
                        {
                            Console.WriteLine("        .level = " + mon.level + ",");
                        }
                        else
                        {
                            Console.WriteLine("        .level = 1, //Placeholder");
                        }
                    }
                    else
                    {
                        Console.WriteLine("        .level = " + userLevel + ",");
                    }
                    
                    Console.WriteLine("    },");

                }
            }
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
            Console.WriteLine("Converted.");
        }
    }
}