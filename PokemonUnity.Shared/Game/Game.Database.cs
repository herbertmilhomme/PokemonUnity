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
//using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
//using System.Security.Cryptography.MD5;

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

	public bool InitPokemons(SqlConnection con, bool sql = true)
	{
		if (sql) return GetPokemonsFromSQL(con);
		else return GetPokemonsFromXML();
	}

	public bool GetPokemonsFromXML()
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
						, regionalDex: new int[] { int.Parse(node.GetAttributeValueOrNull("ReDex")) }
						, type1: (Types)int.Parse(node.GetAttributeValueOrNull("Type1"))
						, type2: (Types)int.Parse(node.GetAttributeValueOrNull("Type2"))
						, ability1: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability1"))
						, ability2: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability2"))
						, hiddenAbility: (Abilities)int.Parse(node.GetAttributeValueOrNull("Ability3"))
						, genderRatio: (PokemonUnity.Monster.GenderRatio)int.Parse(node.GetAttributeValueOrNull("Gender"))
						//, maleRatio: float.Parse(node.GetAttributeValueOrNull("Gender"))
						, catchRate: int.Parse(node.GetAttributeValueOrNull("Catch"))
						, eggGroup1: (PokemonUnity.Monster.EggGroups)int.Parse(node.GetAttributeValueOrNull("Egg1"))
						, eggGroup2: (PokemonUnity.Monster.EggGroups)int.Parse(node.GetAttributeValueOrNull("Egg2"))
						, hatchTime: int.Parse(node.GetAttributeValueOrNull("Hatch"))
						, height: float.Parse(node.GetAttributeValueOrNull("Height"))
						, weight: float.Parse(node.GetAttributeValueOrNull("Weight"))
						, baseExpYield: int.Parse(node.GetAttributeValueOrNull("Exp"))
						, levelingRate: (PokemonUnity.Monster.LevelingRate)int.Parse(node.GetAttributeValueOrNull("Growth"))
						, evHP:  int.Parse(node.GetAttributeValueOrNull("eHP") )
						, evATK: int.Parse(node.GetAttributeValueOrNull("eATK"))
						, evDEF: int.Parse(node.GetAttributeValueOrNull("eDEF"))
						, evSPA: int.Parse(node.GetAttributeValueOrNull("eSPA"))
						, evSPD: int.Parse(node.GetAttributeValueOrNull("eSPD"))
						, evSPE: int.Parse(node.GetAttributeValueOrNull("eSPE"))
						, pokedexColor: (Color)int.Parse(node.GetAttributeValueOrNull("Color"))
						, baseFriendship: int.Parse(node.GetAttributeValueOrNull("Friend"))
						, baseStatsHP:  int.Parse(node.GetAttributeValueOrNull("HP" ))
						, baseStatsATK: int.Parse(node.GetAttributeValueOrNull("ATK"))
						, baseStatsDEF: int.Parse(node.GetAttributeValueOrNull("DEF"))
						, baseStatsSPA: int.Parse(node.GetAttributeValueOrNull("SPA"))
						, baseStatsSPD: int.Parse(node.GetAttributeValueOrNull("SPD"))
						, baseStatsSPE: int.Parse(node.GetAttributeValueOrNull("SPE"))
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
						, evoChainId: int.Parse(node.GetAttributeValueOrNull("EvoChainId"))
						, generationId: (byte)int.Parse(node.GetAttributeValueOrNull("GenerationId"))
						, isDefault:		node.GetAttributeValueOrNull("IsBaseForm") == "1"
						, isBaby:			node.GetAttributeValueOrNull("IsBaby") == "1"
						, formSwitchable:	node.GetAttributeValueOrNull("SwitchableForm") == "1"
						, hasGenderDiff:	node.GetAttributeValueOrNull("GenderDiff") == "1"
						, habitatId: (PokemonUnity.Monster.Habitat)int.Parse(node.GetAttributeValueOrNull("HabitatId"))
						, shapeId: (PokemonUnity.Monster.Shape)int.Parse(node.GetAttributeValueOrNull("ShapeId"))
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

	public bool GetPokemonsFromSQL(SqlConnection con)
	{
		try
		{
			for(int n = 1; n <= Enum.GetValues(typeof(Pokemons)).Length; n++)
			{ 
				//Step 3: Running a Command
				IDbCommand stmt = con.CreateCommand();

				#region DataReader
				stmt.CommandText = @"select pokemon.id, pokemon.species_id, pokemon.height, pokemon.weight, pokemon.base_experience,
					abilities_view.ability1, abilities_view.ability2, abilities_view.hidden, 
					egg_group_view.egg_group1, egg_group_view.egg_group2,
					pokemon_stats_view.batk, pokemon_stats_view.bdef, pokemon_stats_view.bhp, pokemon_stats_view.bspa, pokemon_stats_view.bspd, pokemon_stats_view.bspe,
					pokemon_type_view.type1, pokemon_type_view.type2,
					--pokemon_color_names.name as color,
					pokemon_species.generation_id, pokemon_species.evolves_from_species_id, pokemon_species.evolution_chain_id, pokemon_species.color_id, pokemon_species.shape_id, pokemon_species.habitat_id, pokemon_species.gender_rate, pokemon_species.capture_rate, pokemon_species.base_happiness, pokemon_species.is_baby, pokemon_species.hatch_counter, pokemon_species.has_gender_differences, pokemon_species.growth_rate_id, pokemon_species.forms_switchable, pokemon_species.`order`
					--pokemon_species_names.name,pokemon_species_names.genus,
					--pokemon_species_flavor_text.flavor_text
					from pokemon 
					join abilities_view on pokemon.id = abilities_view.pokemon_id 
					join egg_group_view on egg_group_view.species_id = pokemon.id 
					join pokemon_stats_view on pokemon_stats_view.pokemon_id = pokemon.id 
					join pokemon_type_view on pokemon_type_view.pokemon_id = pokemon.id 
					join pokemon_species on pokemon_species.id = pokemon.id
					--left join pokemon_colors on pokemon_colors.id = pokemon_species.color_id
					--left join pokemon_color_names on pokemon_color_names.pokemon_color_id=pokemon_colors.id AND pokemon_color_names.local_language_id=9
					--join pokemon_species_names on pokemon_species_names.pokemon_species_id = pokemon.id AND pokemon_species_names.local_language_id=9
					--join pokemon_species_flavor_text on pokemon_species_flavor_text.species_id = pokemon.id AND pokemon_species_flavor_text.version_id=26 AND pokemon_species_flavor_text.language_id=9
					order by pokemon.id ASC";
				IDataReader reader = stmt.ExecuteReader();
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
								//, baseForm: Data.baseForm
								//, heldItem: Data.heldItem
							)
						);
					}
				}
				//Step 5: Closing up
				reader.Close();
                reader.Dispose();
				#endregion
			}
			return true;
		} catch (SqlException e) {
			//Debug.Log("SQL Exception Message:" + e.Message);
			//Debug.Log("SQL Exception Code:" + e.ErrorCode.ToString());
			//Debug.Log("SQL Exception Help:" + e.HelpLink);
			return false;
		}
	}
}
