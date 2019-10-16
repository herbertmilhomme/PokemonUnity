using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using PokemonUnity;
using PokemonUnity.Monster;
using PokemonUnity.Inventory;
using PokemonUnity.Attack;
using PokemonUnity.Saving;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
//using System.Data.SqlClient;
using System.IO;
//using System.Security.Cryptography.MD5;

namespace PokemonUnity
{
/// <summary>
/// Variables that are stored when game is saved, and other temp values used for gameplay.
/// This class should be called once, when the game boots-up.
/// During boot-up, game will check directory for save files and load data.
/// Game class will overwrite all the other class default values when player triggers a load state.
/// </summary>
public partial class Game 
{
	const string FilePokemonXML = "";
	//ToDo: ReadOnly Immutable Dictionaries...
	public static Dictionary<Pokemons, PokemonUnity.Monster.Data.PokemonData> PokemonData { get; private set; }
	//public static Dictionary<Moves,Move.MoveData> MoveData { get; private set; }
	//public static Dictionary<Items,Item> ItemData { get; private set; }
	//public static Dictionary<Natures,Nature> NatureData { get; private set; }
	//public static Dictionary<Berries,Item.Berry> BerryData { get; private set; }
	//ability
	//forms
	//held items
	//evolutions
	//locations
	//location encounters

	public static bool InitPokemons(bool sql = true)
	{
		PokemonData = new Dictionary<Pokemons, Monster.Data.PokemonData>();
		if (sql) using (con) return GetPokemonsFromSQL(con);
		else return GetPokemonsFromXML();
	}

	static bool GetPokemonsFromXML()
	{
		PokemonData = new Dictionary<Pokemons, PokemonUnity.Monster.Data.PokemonData>();

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(LockFileStream(FilePokemonXML));

		var pokemons = xmlDocument.SelectNodes("/Pokemons");
		if (pokemons == null || pokemons.Count <= 0)
		{
			//throw new Exception("Xml must include "Pokemons" as root node.");
			return false;
		}

		var dublicateNames = new List<string>();

		var pokemonNodes = xmlDocument.SelectSingleNode("/Pokemons").ChildNodes;// /text
		if (pokemonNodes != null)
		{
			//foreach value in xml
			foreach (XmlNode node in pokemonNodes)
			{
				//XmlAttribute sngl = node.GetAttributeValueOrNull("SingleGender");
				PokemonData.Add((Pokemons)int.Parse(node.GetAttributeValueOrNull("Id")),
					new PokemonUnity.Monster.Data.PokemonData(
						Id: (Pokemons)int.Parse(node.GetAttributeValueOrNull("Id"))
						,regionalDex: new int[] { int.Parse(node.GetAttributeValueOrNull("ReDex")) }
						,type1: (Types)int.Parse(node.GetAttributeValueOrNull("Type1"))
						,type2: (Types)int.Parse(node.GetAttributeValueOrNull("Type2"))
						,ability1: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability1"))
						,ability2: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability2"))
						,hiddenAbility: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability3"))
						,genderRatio: (PokemonUnity.Monster.GenderRatio)int.Parse(node.GetAttributeValueOrNull("Gender"))
						//,maleRatio: float.Parse(node.GetAttributeValueOrNull("Gender"))
						,catchRate: int.Parse(node.GetAttributeValueOrNull("Catch"))
						,eggGroup1: (PokemonUnity.Monster.EggGroups)int.Parse(node.GetAttributeValueOrNull("Egg1"))
						,eggGroup2: (PokemonUnity.Monster.EggGroups)int.Parse(node.GetAttributeValueOrNull("Egg2"))
						,hatchTime: int.Parse(node.GetAttributeValueOrNull("Hatch"))
						,height: float.Parse(node.GetAttributeValueOrNull("Height"))
						,weight: float.Parse(node.GetAttributeValueOrNull("Weight"))
						,baseExpYield: int.Parse(node.GetAttributeValueOrNull("Exp"))
						,levelingRate: (PokemonUnity.Monster.LevelingRate)int.Parse(node.GetAttributeValueOrNull("Growth"))
						,evHP:  int.Parse(node.GetAttributeValueOrNull("eHP") )
						,evATK: int.Parse(node.GetAttributeValueOrNull("eATK"))
						,evDEF: int.Parse(node.GetAttributeValueOrNull("eDEF"))
						,evSPA: int.Parse(node.GetAttributeValueOrNull("eSPA"))
						,evSPD: int.Parse(node.GetAttributeValueOrNull("eSPD"))
						,evSPE: int.Parse(node.GetAttributeValueOrNull("eSPE"))
						,pokedexColor: (Color)int.Parse(node.GetAttributeValueOrNull("Color"))
						,baseFriendship: int.Parse(node.GetAttributeValueOrNull("Friend"))
						,baseStatsHP:  int.Parse(node.GetAttributeValueOrNull("HP" ))
						,baseStatsATK: int.Parse(node.GetAttributeValueOrNull("ATK"))
						,baseStatsDEF: int.Parse(node.GetAttributeValueOrNull("DEF"))
						,baseStatsSPA: int.Parse(node.GetAttributeValueOrNull("SPA"))
						,baseStatsSPD: int.Parse(node.GetAttributeValueOrNull("SPD"))
						,baseStatsSPE: int.Parse(node.GetAttributeValueOrNull("SPE"))
						,rarity: (Rarity)int.Parse(node.GetAttributeValueOrNull("Rarity"))
						//,luminance: Data.luminance
						//,movesetmoves: Data.movesetmoves
						//,movesetLevels: Data.movesetLevels
						//,movesetMoves: Data.movesetMoves
						//,tmList: Data.tmList
						//,evolution: Data.evolution
						//,evolutionID: Data.EvolutionTO
						//,evolutionLevel: Data.evolutionLevel
						//,evolutionMethod: Data.evolutionMethod						
						//,evolutionFROM: Data.EvolutionFROM
						//,evolutionTO: Data.EvolutionTO
						,evoChainId: int.Parse(node.GetAttributeValueOrNull("EvoChainId"))
						,generationId: (byte)int.Parse(node.GetAttributeValueOrNull("GenerationId"))
						,isDefault:		node.GetAttributeValueOrNull("IsBaseForm") == "1"
						,isBaby:			node.GetAttributeValueOrNull("IsBaby") == "1"
						,formSwitchable:	node.GetAttributeValueOrNull("SwitchableForm") == "1"
						,hasGenderDiff:	node.GetAttributeValueOrNull("GenderDiff") == "1"
						,habitatId: (PokemonUnity.Monster.Habitat)int.Parse(node.GetAttributeValueOrNull("HabitatId"))
						,shapeId: (PokemonUnity.Monster.Shape)int.Parse(node.GetAttributeValueOrNull("ShapeId"))
						//,baseForm: Data.baseForm
						//,heldItem: Data.heldItem
					)
				);
			}
		}

		//dictionaries cant store duplicate values...
		//if (dublicateNames.Count > 0)
		//{
		//	//throw new Exception("A dictionary can not contain same key twice. There are some duplicated names: " + dublicateNames.JoinAsString(", "));//string.Join(", ",dublicateNames.ToArray())
		//}

		return true; //dictionary;
	}

	static bool GetPokemonsFromSQL(SQLiteConnection con)
	{
		try
		{
			//for(int n = 1; n <= Enum.GetValues(typeof(Pokemons)).Length; n++)
			//{ 
				//Step 3: Running a Command
				SQLiteCommand stmt = con.CreateCommand();

				#region DataReader
				stmt.CommandText = "select * from pokemon_views --order by id ASC";
					//	@"select pokemon.id, pokemon.species_id, pokemon.identifier, pokemon.height, pokemon.weight, pokemon.base_experience, --pokemon.""order""
					//pokemon_abilities_view.ability1, pokemon_abilities_view.ability2, pokemon_abilities_view.ability3, 
					//pokemon_egg_groups_view.egg_group1, pokemon_egg_groups_view.egg_group2,
					//pokemon_stats_view.bhp, pokemon_stats_view.batk, pokemon_stats_view.bdef, pokemon_stats_view.bspa, pokemon_stats_view.bspd, pokemon_stats_view.bspe, pokemon_stats_view.ehp, pokemon_stats_view.eatk, pokemon_stats_view.edef, pokemon_stats_view.espa, pokemon_stats_view.espd, pokemon_stats_view.espe,
					//pokemon_types_view.type1, pokemon_types_view.type2,
					//pokemon_color_names.name as color,
					//--pokemon_species.base_happiness, pokemon_species.capture_rate, pokemon_species.gender_rate, pokemon_species.hatch_counter, pokemon_species.shape_id, pokemon_species.growth_rate_id,
					//pokemon_species.generation_id, pokemon_species.evolves_from_species_id, pokemon_species.evolution_chain_id, pokemon_species.color_id, pokemon_species.shape_id, pokemon_species.habitat_id, pokemon_species.gender_rate, pokemon_species.capture_rate, pokemon_species.base_happiness, pokemon_species.is_baby, pokemon_species.hatch_counter, pokemon_species.has_gender_differences, pokemon_species.growth_rate_id, pokemon_species.forms_switchable, pokemon_species.""order"",
					//pokemon_species_names.name,pokemon_species_names.genus,
					//pokemon_species_flavor_text.flavor_text
					//from pokemon
					//left join pokemon_abilities_view on pokemon.id = pokemon_abilities_view.pokemon_id
					//left join pokemon_egg_groups_view on pokemon_egg_groups_view.pokemon_id = pokemon.id
					//left join pokemon_stats_view on pokemon_stats_view.pokemon_id = pokemon.id
					//left join pokemon_types_view on pokemon_types_view.pokemon_id = pokemon.id
					//left join pokemon_species on pokemon_species.id = pokemon.id
					//left join pokemon_colors on pokemon_colors.id = pokemon_species.color_id
					//left join pokemon_color_names on pokemon_color_names.pokemon_color_id = pokemon_colors.id AND pokemon_color_names.local_language_id = 9
					//left join pokemon_species_names on pokemon_species_names.pokemon_species_id = pokemon.id AND pokemon_species_names.local_language_id = 9
					//left join pokemon_species_flavor_text on pokemon_species_flavor_text.species_id = pokemon.id AND pokemon_species_flavor_text.version_id = 26 AND pokemon_species_flavor_text.language_id = 9
					//order by pokemon.id ASC;";
				SQLiteDataReader reader = stmt.ExecuteReader();

				//Step 4: Read the results
				using(reader)
				{ int i = 0;
					while(reader.Read()) //if(reader.Read())
					{ i++;
						/*Debug.Log("Pokemon DB Test Value:" + reader["name"] as string);
						ID = System.Convert.ToInt16(reader["id"]);// is int ? (int)reader["id"] : -1;// as System.UInt16;
						//pokemonOrder = (System.UInt16)reader["order"];// numerical order of pokemon evolution
						NAME = reader["name"] as string;
						TYPE1 = reader["type1"] is DBNull ? PokemonData.Type.NONE : (PokemonData.Type)Convert.ToInt16(reader["type1"]);//if correct this should convert int to enum string 
						TYPE2 = reader["type2"] is DBNull ? PokemonData.Type.NONE : (PokemonData.Type)Convert.ToInt16(reader["type2"]);
						Ability1 = reader["ability1"] is DBNull ? (int?)null : (System.UInt16)Convert.ToInt16(reader["ability1"]);
						Ability2 = reader["ability2"] is DBNull ? (int?)null : (System.Int16)Convert.ToInt16(reader["ability2"]);//if null, needs to be a number... possibly -1 or 0
						HiddenAbility = reader["hidden"] is DBNull ? (int?)null : (System.Int16)Convert.ToInt16(reader["hidden"]);//if null, needs to be a number... possibly -1 or 0
						maleRatio = (float)Convert.ToInt16(reader["gender_rate"]);
						catchRate = (System.UInt16)Convert.ToInt16(reader["capture_rate"]);
						eggGroup1 = reader["egg_group1"] is DBNull ? PokemonData.EggGroup.NONE : (PokemonData.EggGroup)Convert.ToInt16(reader["egg_group1"]);
						eggGroup2 = reader["egg_group1"] is DBNull ? PokemonData.EggGroup.NONE : (PokemonData.EggGroup)Convert.ToInt16(reader["egg_group2"]);
						hatchTime = (System.UInt16)Convert.ToInt16(reader["hatch_counter"]);
						height = (System.Int32)Convert.ToInt32(reader["height"]);
						weight = (System.Int32)Convert.ToInt32(reader["weight"]);
						baseExpYield = (System.UInt16)Convert.ToInt16(reader["base_experience"]);
						levelingRate = (PokemonData.LevelingRate)Convert.ToInt16(reader["growth_rate_id"]);
						//evYieldHP = (System.UInt16)reader["evhp"];
						//evYieldATK = (System.UInt16)reader["evatk"];
						//evYieldDEF = (System.UInt16)reader["evdef"];
						//evYieldSPA = (System.UInt16)reader["evspa"];
						//evYieldSPD = (System.UInt16)reader["evspd"];
						//evYieldSPE = (System.UInt16)reader["evspd"];
						species = (System.UInt16)Convert.ToInt16(reader["species_id"]);//pokedex-id
						pokedexEntry = reader["flavor_text"] as string;
						baseStatsHP = (System.UInt16)Convert.ToInt16(reader["bhp"]);
						baseStatsATK = (System.UInt16)Convert.ToInt16(reader["batk"]);
						baseStatsDEF = (System.UInt16)Convert.ToInt16(reader["bdef"]);
						baseStatsSPA = (System.UInt16)Convert.ToInt16(reader["bspa"]);
						baseStatsSPD = (System.UInt16)Convert.ToInt16(reader["bspd"]);
						baseStatsSPE = (System.UInt16)Convert.ToInt16(reader["bspe"]);
						luminance = 0;//(float)reader["lumi"]; //i dontknow what this is...
						lightColor = !string.IsNullOrEmpty((string)reader["color"]) ? (Color)StringToColor[Convert.ToString(reader["color"])] : Color.clear;
						//pokedexColor = (PokemonData.PokedexColor)Convert.ToInt16(reader["color"]); //lightColor; no db on pokedex-colers per pokemon...
						baseFriendship = (System.UInt16)Convert.ToInt16(reader["base_happiness"]);
						//shape id*/
						PokemonData.Add((Pokemons)int.Parse((string)reader["id"].ToString()),
							new PokemonUnity.Monster.Data.PokemonData(
								Id: (Pokemons)int.Parse((string)reader["id"].ToString())
								,baseForm: (Pokemons)int.Parse((string)reader["species_id"].ToString())
								//,regionalDex: new int[].ToString() { int.Parse((string)reader["ReDex"].ToString()) }
								,type1: (Types)int.Parse(string.IsNullOrEmpty((string)reader["type1"].ToString()) ? "0" : (string)reader["type1"].ToString())
								,type2: (Types)int.Parse(string.IsNullOrEmpty((string)reader["type2"].ToString()) ? "0" : (string)reader["type2"].ToString())
								,ability1: (Abilities)int.Parse(string.IsNullOrEmpty((string)reader["ability1"].ToString()) ? "0" : (string)reader["ability1"].ToString())
								,ability2: (Abilities)int.Parse(string.IsNullOrEmpty((string)reader["ability2"].ToString()) ? "0" : (string)reader["ability2"].ToString())
								,hiddenAbility: (Abilities)int.Parse(string.IsNullOrEmpty((string)reader["ability3"].ToString()) ? "0" : (string)reader["ability3"].ToString())
								,genderRatio: (PokemonUnity.Monster.GenderRatio)int.Parse(string.IsNullOrEmpty((string)reader["gender_rate"].ToString()) ? "0" : (string)reader["gender_rate"].ToString())
								//,maleRatio: float.Parse((string)reader["Gender"].ToString())
								,catchRate: int.Parse(string.IsNullOrEmpty((string)reader["capture_rate"].ToString()) ? "0" : (string)reader["capture_rate"].ToString())
								,eggGroup1: (PokemonUnity.Monster.EggGroups)int.Parse(string.IsNullOrEmpty((string)reader["egg_group1"].ToString()) ? "0" : (string)reader["egg_group1"].ToString())
								,eggGroup2: (PokemonUnity.Monster.EggGroups)int.Parse(string.IsNullOrEmpty((string)reader["egg_group2"].ToString()) ? "0" : (string)reader["egg_group2"].ToString())
								,hatchTime: int.Parse(string.IsNullOrEmpty((string)reader["hatch_counter"].ToString()) ? "0" : (string)reader["hatch_counter"].ToString())
								,height: float.Parse((string)reader["height"].ToString())
								,weight: float.Parse((string)reader["weight"].ToString())
								,baseExpYield: int.Parse(string.IsNullOrEmpty((string)reader["base_experience"].ToString()) ? "0" : (string)reader["base_experience"].ToString())
								,levelingRate: (PokemonUnity.Monster.LevelingRate)int.Parse(string.IsNullOrEmpty((string)reader["growth_rate_id"].ToString()) ? "0" : (string)reader["growth_rate_id"].ToString())
								,evHP:  int.Parse((string)reader["ehp"].ToString())
								,evATK: int.Parse((string)reader["eatk"].ToString())
								,evDEF: int.Parse((string)reader["edef"].ToString())
								,evSPA: int.Parse((string)reader["espa"].ToString())
								,evSPD: int.Parse((string)reader["espd"].ToString())
								,evSPE: int.Parse((string)reader["espe"].ToString())
								,pokedexColor: (Color)int.Parse(string.IsNullOrEmpty((string)reader["color_id"].ToString()) ? "0" : (string)reader["color_id"].ToString())
								,baseFriendship: int.Parse(string.IsNullOrEmpty((string)reader["base_happiness"].ToString()) ? "0" : (string)reader["base_happiness"].ToString())
								,baseStatsHP:  int.Parse((string)reader["bhp"].ToString())
								,baseStatsATK: int.Parse((string)reader["batk"].ToString())
								,baseStatsDEF: int.Parse((string)reader["bdef"].ToString())
								,baseStatsSPA: int.Parse((string)reader["bspa"].ToString())
								,baseStatsSPD: int.Parse((string)reader["bspd"].ToString())
								,baseStatsSPE: int.Parse((string)reader["bspe"].ToString())
								//,rarity: (Rarity)int.Parse((string)reader["Rarity"].ToString())
								//,luminance: Data.luminance
								//,movesetmoves: Data.movesetmoves
								//,movesetLevels: Data.movesetLevels
								//,movesetMoves: Data.movesetMoves
								//,tmList: Data.tmList
								//,evolution: Data.evolution
								//,evolutionID: Data.EvolutionTO
								//,evolutionLevel: Data.evolutionLevel
								//,evolutionMethod: Data.evolutionMethod						
								//,evolutionFROM: Data.EvolutionFROM
								//,evolutionTO: Data.EvolutionTO
								,evoChainId: int.Parse(string.IsNullOrEmpty((string)reader["evolution_chain_id"].ToString()) ? "0" : (string)reader["evolution_chain_id"].ToString())
								,generationId: (byte)int.Parse(string.IsNullOrEmpty((string)reader["generation_id"].ToString()) ? "0" : (string)reader["generation_id"].ToString())
								//,isDefault: (string)reader["IsBaseForm"].ToString() == "1"
								,isBaby: (string)reader["is_baby"].ToString() == "1"
								,formSwitchable: (string)reader["forms_switchable"].ToString() == "1"
								,hasGenderDiff: (string)reader["has_gender_differences"].ToString() == "1"
								,habitatId: (PokemonUnity.Monster.Habitat)int.Parse(string.IsNullOrEmpty((string)reader["habitat_id"].ToString()) ? "0" : (string)reader["habitat_id"].ToString())
								,shapeId: (PokemonUnity.Monster.Shape)int.Parse(string.IsNullOrEmpty((string)reader["shape_id"].ToString()) ? "0" : (string)reader["shape_id"].ToString())
								//,heldItem: Data.heldItem
							)
						);
					}
				//}
				//Step 5: Closing up
				reader.Close();
                reader.Dispose();
				#endregion
			}
			return true;
		} catch (SQLiteException e) {
			//Debug.Log("SQL Exception Message:" + e.Message);
			//Debug.Log("SQL Exception Code:" + e.ErrorCode.ToString());
			//Debug.Log("SQL Exception Help:" + e.HelpLink);
			return false;
		}
	}

	public bool GetPokemonsFromSQL1(SQLiteConnection con)
	{
		try
		{
			string sqlQuery;
			for(int n = 1; n <= Enum.GetValues(typeof(Pokemons)).Length; n++)//couldnt figure out a way to automate the loop, so i did it manually ("end at 721"... the db has 800...)
			{ 
				//Step 3: Running a Command
				SQLiteCommand stmt = con.CreateCommand();

				#region DataReader
				sqlQuery = string.Format("select * from pokemonDB where id={0} order by id ASC",n);//[order] = {0} order by [order] ASC",n);//for the sake of debugging
				stmt.CommandText = sqlQuery;//"select * --pokemon.*,abilities_view.ability1,abilities_view.ability2,abilities_view.hidden, egg_group_view.egg_group1, egg_group_view.egg_group2" +
				//	"from pokemon" +
				//	"join abilities_view on pokemon.id = abilities_view.pokemon_id"+
				//	"join egg_group_view on egg_group_view.species_id = pokemon.id"+
				//	"join pokemon_species on pokemon_species.id = pokemon.id"+
				//	"join pokemon_species_names on pokemon_species_names.pokemon_species_id = pokemon.id AND pokemon_species_names.local_language_id = 9"+
				//	"join pokemon_species_flavor_text on pokemon_species_flavor_text.species_id = pokemon.id AND pokemon_species_flavor_text.version_id = 26 AND pokemon_species_flavor_text.language_id = 9 ";
				//"SELECT * FROM StoredProcedure/View ORDER BY ID ASC";
				SQLiteDataReader reader = stmt.ExecuteReader();
				//if(!((System.Data.Common.DbDataReader)reader).HasRows) { Debug.Log("DB is Empty"); }//break for-loop

				//Step 4: Read the results
				using(reader)
				{
					while(reader.Read())
					{
						/*Debug.Log("Pokemon DB Test Value:" + reader["name"] as string);
						ID = System.Convert.ToInt16(reader["id"]);// is int ? (int)reader["id"] : -1;// as System.UInt16;
						//pokemonOrder = (System.UInt16)reader["order"];// numerical order of pokemon evolution
						NAME = reader["name"] as string;
						TYPE1 = reader["type1"] is DBNull ? PokemonData.Type.NONE : (PokemonData.Type)Convert.ToInt16(reader["type1"]);//if correct this should convert int to enum string 
						TYPE2 = reader["type2"] is DBNull ? PokemonData.Type.NONE : (PokemonData.Type)Convert.ToInt16(reader["type2"]);
						Ability1 = reader["ability1"] is DBNull ? (int?)null : (System.UInt16)Convert.ToInt16(reader["ability1"]);
						Ability2 = reader["ability2"] is DBNull ? (int?)null : (System.Int16)Convert.ToInt16(reader["ability2"]);//if null, needs to be a number... possibly -1 or 0
						HiddenAbility = reader["hidden"] is DBNull ? (int?)null : (System.Int16)Convert.ToInt16(reader["hidden"]);//if null, needs to be a number... possibly -1 or 0
						maleRatio = (float)Convert.ToInt16(reader["gender_rate"]);
						catchRate = (System.UInt16)Convert.ToInt16(reader["capture_rate"]);
						eggGroup1 = reader["egg_group1"] is DBNull ? PokemonData.EggGroup.NONE : (PokemonData.EggGroup)Convert.ToInt16(reader["egg_group1"]);
						eggGroup2 = reader["egg_group1"] is DBNull ? PokemonData.EggGroup.NONE : (PokemonData.EggGroup)Convert.ToInt16(reader["egg_group2"]);
						hatchTime = (System.UInt16)Convert.ToInt16(reader["hatch_counter"]);
						height = (System.Int32)Convert.ToInt32(reader["height"]);
						weight = (System.Int32)Convert.ToInt32(reader["weight"]);
						baseExpYield = (System.UInt16)Convert.ToInt16(reader["base_experience"]);
						levelingRate = (PokemonData.LevelingRate)Convert.ToInt16(reader["growth_rate_id"]);
						//evYieldHP = (System.UInt16)reader["evhp"];
						//evYieldATK = (System.UInt16)reader["evatk"];
						//evYieldDEF = (System.UInt16)reader["evdef"];
						//evYieldSPA = (System.UInt16)reader["evspa"];
						//evYieldSPD = (System.UInt16)reader["evspd"];
						//evYieldSPE = (System.UInt16)reader["evspd"];
						species = (System.UInt16)Convert.ToInt16(reader["species_id"]);//pokedex-id
						pokedexEntry = reader["flavor_text"] as string;
						baseStatsHP = (System.UInt16)Convert.ToInt16(reader["bhp"]);
						baseStatsATK = (System.UInt16)Convert.ToInt16(reader["batk"]);
						baseStatsDEF = (System.UInt16)Convert.ToInt16(reader["bdef"]);
						baseStatsSPA = (System.UInt16)Convert.ToInt16(reader["bspa"]);
						baseStatsSPD = (System.UInt16)Convert.ToInt16(reader["bspd"]);
						baseStatsSPE = (System.UInt16)Convert.ToInt16(reader["bspe"]);
						luminance = 0;//(float)reader["lumi"]; //i dontknow what this is...
						lightColor = !string.IsNullOrEmpty((string)reader["color"]) ? (Color)StringToColor[Convert.ToString(reader["color"])] : Color.clear;
						//pokedexColor = (PokemonData.PokedexColor)Convert.ToInt16(reader["color"]); //lightColor; no db on pokedex-colers per pokemon...
						baseFriendship = (System.UInt16)Convert.ToInt16(reader["base_happiness"]);
						//shape id*/
						PokemonData.Add((Pokemons)int.Parse((string)reader["Id"]),
							new PokemonUnity.Monster.Data.PokemonData(
								Id: (Pokemons)int.Parse((string)reader["Id"])
								, regionalDex: new int[] { int.Parse((string)reader["ReDex"]) }
								, type1: (Types)int.Parse((string)reader["Type1"])
								, type2: (Types)int.Parse((string)reader["Type2"])
								, ability1: (Abilities)int.Parse((string)reader["Ability1"])
								, ability2: (Abilities)int.Parse((string)reader["Ability2"])
								, hiddenAbility: (Abilities)int.Parse((string)reader["Ability3"])
								, genderRatio: (PokemonUnity.Monster.GenderRatio)int.Parse((string)reader["Gender"])
								//, maleRatio: float.Parse((string)reader["Gender"])
								, catchRate: int.Parse((string)reader["Catch"])
								, eggGroup1: (PokemonUnity.Monster.EggGroups)int.Parse((string)reader["Egg1"])
								, eggGroup2: (PokemonUnity.Monster.EggGroups)int.Parse((string)reader["Egg2"])
								, hatchTime: int.Parse((string)reader["Hatch"])
								, height: float.Parse((string)reader["Height"])
								, weight: float.Parse((string)reader["Weight"])
								, baseExpYield: int.Parse((string)reader["Exp"])
								, levelingRate: (PokemonUnity.Monster.LevelingRate)int.Parse((string)reader["Growth"])
								, evHP: int.Parse((string)reader["eHP"])
								, evATK: int.Parse((string)reader["eATK"])
								, evDEF: int.Parse((string)reader["eDEF"])
								, evSPA: int.Parse((string)reader["eSPA"])
								, evSPD: int.Parse((string)reader["eSPD"])
								, evSPE: int.Parse((string)reader["eSPE"])
								, pokedexColor: (Color)int.Parse((string)reader["Color"])
								, baseFriendship: int.Parse((string)reader["Friend"])
								, baseStatsHP: int.Parse((string)reader["HP"])
								, baseStatsATK: int.Parse((string)reader["ATK"])
								, baseStatsDEF: int.Parse((string)reader["DEF"])
								, baseStatsSPA: int.Parse((string)reader["SPA"])
								, baseStatsSPD: int.Parse((string)reader["SPD"])
								, baseStatsSPE: int.Parse((string)reader["SPE"])
								, rarity: (Rarity)int.Parse((string)reader["Rarity"])
								//,luminance: Data.luminance
								//,movesetmoves: Data.movesetmoves
								//,movesetLevels: Data.movesetLevels
								//,movesetMoves: Data.movesetMoves
								//,tmList: Data.tmList
								//,evolution: Data.evolution
								//,evolutionID: Data.EvolutionTO
								//,evolutionLevel: Data.evolutionLevel
								//,evolutionMethod: Data.evolutionMethod						
								//,evolutionFROM: Data.EvolutionFROM
								//,evolutionTO: Data.EvolutionTO
								, evoChainId: int.Parse((string)reader["EvoChainId"])
								, generationId: (byte)int.Parse((string)reader["GenerationId"])
								, isDefault: (string)reader["IsBaseForm"] == "1"
								, isBaby: (string)reader["IsBaby"] == "1"
								, formSwitchable: (string)reader["SwitchableForm"] == "1"
								, hasGenderDiff: (string)reader["GenderDiff"] == "1"
								, habitatId: (PokemonUnity.Monster.Habitat)int.Parse(string.IsNullOrEmpty((string)reader["HabitatId"]) ? "0" : (string)reader["HabitatId"])
								, shapeId: (PokemonUnity.Monster.Shape)int.Parse((string)reader["ShapeId"])
							//,baseForm: Data.baseForm
							//,heldItem: Data.heldItem
							)
						);
					}
				}
				//Step 5: Closing up
				reader.Close();
                reader.Dispose();
				#endregion

				//#region DataReader
				////Step 3: Running a Command
				//sqlQuery = string.Format(@"SELECT distinct a.move_id, a.pokemon_id as idA, a.pokemon_move_method_id as methodA, c.pLv FROM pokemon_moves as a 
				//	join(SELECT distinct[level] as pLv,move_id,pokemon_id as id,pokemon_move_method_id as methodB FROM pokemon_moves as b WHERE ID = {0} AND methodB = 1 ORDER BY[move_id] ASC, [level] ASC) as c 
				//	on a.move_id = c.move_id AND idA = c.id AND c.id = {1} AND c.methodB = 1 AND methodA = methodB 
				//	group by a.move_id 
				//	--ORDER BY c.[pLv] ASC,a.[move_id] ASC",n,n);//(System.UInt16)Convert.ToInt16(reader["id"]));//select distinct move_id, pokemon_id, [level] from pokemon_moves where pokemon_id={0} ORDER BY [move_id] ASC, [level] DESC
				//stmt.CommandText = sqlQuery;
				////select * from openquery(veekun, 'select distinct move_id, pokemon_id, [level] from pokemon_moves where pokemon_id=1 ORDER BY pokemon_id ASC, [level] ASC--, [order] ASC')
				//Debug.Log("Opening Pokemon MoveArray DB");
				//IDataReader reader2 = stmt.ExecuteReader();
				//Debug.Log("Opened Pokemon MoveArray DB");

				//List<int> lvArray = new List<int>();
				//List<int> moveArray = new List<int>();

				////Step 4: Read the results
				//using(reader2)
				//{
				//	Debug.Log("Reading Pokemon MoveArrary DB");
				//	while(reader2.Read())
				//	{
				//		lvArray.Add((System.UInt16)Convert.ToInt16(reader2["pLv"])); //level where pokemon learns move
				//		moveArray.Add((System.UInt16)Convert.ToInt16(reader2["move_id"])); //move_id
				//		//strictly added moves that can be learned thru leveling-up.
				//		//methodArray.Add((System.UInt16)reader["methodA"]); //method on how pokemon learns move (tm, tutor, lvl-up...)
				//	}
				//	//reader2.NextResult();
				//}
				////Step 5: Closing up
				//reader2.Close();
				//#endregion

				//#region DataReader
				////Step 3: Running a Command
				//sqlQuery = string.Format("select t1.*, t1.id as pokemon, t2.id as ParamID from pokemon_species as t1 inner Join pokemon_evolution as t2 on t1.id=t2.evolved_species_id where t1.evolves_from_species_id = {0}",n);//(System.UInt16)Convert.ToInt16(reader["id"]));
				//stmt.CommandText = sqlQuery;//"SELECT evolutionID, evoRequirements FROM Table WHERE ID = @0 ORDER BY evolutionID ASC";
				//						    //select * from openquery(veekun, 'select t1.*, t1.id as pokemon, t2.id as ParamID from pokemon_species as t1 inner Join pokemon_evolution as t2 on t1.id=t2.evolved_species_id where t1.evolves_from_species_id = @')
				//Debug.Log("Opening Pokemon EvolutionArray DB");
				//IDataReader reader3 = stmt.ExecuteReader();
				//Debug.Log("Opened Pokemon EvolutionArray DB");

				//List<int> evoIDarray = new List<int>();
				//List<string> evoReqArray = new List<string>();

				////Step 4: Read the results
				//using(reader3)
				//{
				//	Debug.Log("Reading Pokemon EvArray DB");
				//	while(reader3.Read())
				//	{//not all pokemons evolve... wasnt sure how to null a int[], but we'll see on debug
				//		if(!reader3.IsDBNull(1/*colum index*/))
				//		{ //if not null, add value, else skip it..
				//			//null;
				//			//else { 
				//			evoIDarray.Add((System.UInt16)Convert.ToInt16(reader3["id"])); //pokemon id
				//			evoReqArray.Add(reader3["paramid"] as string); //requirements id
				//		}
				//	}
				//}
				////Step 5: Closing up
				//reader3.Close();
				//#endregion

				//#region DataReader
				////Step 3: Running a Command
				//sqlQuery = string.Format("select distinct pokemon_id, item_id, rarity from pokemon_items where pokemon_id = {0}",n);// (System.UInt16)reader["id"]);
				//stmt.CommandText = sqlQuery;//
				//Debug.Log("Opening Pokemon HeldItemArray DB");
				//IDataReader reader5 = stmt.ExecuteReader();
				//Debug.Log("Opened Pokemon HeldItemArray DB");

				////List<List<int>> holdArray = new List<List<int>>();
				//List<int[]> holdArray = new List<int[]>();// works better with unknown size
				////holdArray.Add(new List<int>());

				////Step 4: Read the results
				//using(reader5)
				//{
				//	Debug.Log("Reading Pokemon Item DB");
				//	while(reader5.Read())
				//	{//not all pokemons evolve... wasnt sure how to null a int[], but we'll see on debug
				//		List<int> itemList = new List<int>();
				//		if(reader5.IsDBNull(1/*colum index*/))
				//		{ //if not null, add value, else skip it..
				//			holdArray.Add(new int[2] { -1,100 });//null held item, with value of 100%;;
				//			//holdArray[0].Add(100);//value of 100%;
				//		} else
				//		{
				//			holdArray.Add(new int[2] { (System.UInt16)Convert.ToInt16(reader5["item_id"]),(System.UInt16)Convert.ToInt16(reader5["rarity"]) }); //pokemon id
				//																																				//rarityArray.Add((System.UInt16)Convert.ToInt16(reader["rarity"])); //requirements id
				//		}
				//	}
				//}
				////Step 5: Closing up
				//reader5.Close();
				//#endregion

				//#region DataReader
				////Step 3: Running a Command
				//sqlQuery = string.Format("select distinct tm_views.move_id, tm_views.itemNo, pokemon_moves.pokemon_id, pokemon_moves.pokemon_move_method_id " +
				//	"from tm_views " +
				//	"join pokemon_moves on tm_views.move_id = pokemon_moves.move_id " +
				//	"where pokemon_moves.pokemon_move_method_id = 4 AND pokemon_moves.pokemon_id = {0} " +
				//	"order by pokemon_moves.pokemon_id ASC,tm_views.move_id Asc",n);// (System.UInt16)Convert.ToInt16(reader["id"]));
				//stmt.CommandText = sqlQuery;//"SELECT tmID FROM Table WHERE ID = @0 ORDER BY tmID ASC";
				//Debug.Log("Opening Pokemon Technical Machine DB");
				//IDataReader reader4 = stmt.ExecuteReader();
				//Debug.Log("Opened Pokemon TM DB");

				//List<int> tmArray = new List<int>();

				////Step 4: Read the results
				//using(reader4)
				//{
				//	Debug.Log("Reading Pokemon Machine DB");
				//	while(reader4.Read())
				//	{
				//		tmArray.Add((System.UInt16)Convert.ToInt16(reader4["itemNo"]));
				//	}
				//}
				////Step 5: Closing up
				//reader4.Close();
				//#endregion
			}
			return true;
		} catch (SQLiteException e) {
			//Debug.Log("SQL Exception Message:" + e.Message);
			//Debug.Log("SQL Exception Code:" + e.ErrorCode.ToString());
			//Debug.Log("SQL Exception Help:" + e.HelpLink);
			return false;
		}
	}
}
}