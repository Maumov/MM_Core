using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace MM.CollectionEnum
{
	public static class CollectionEnumCreator
	{
		private const string FILE_TEMPLATE = "/{name}Enum.cs";
		private const string FIELD_TEMPLATE = "\t\tpublic static {name}Value {field} = new {name}Value(\"{id}\");";
		private const string EXTENDS_TEMPLATE = "{name}EnumTemplate";

		public static void WriteFile(string i_Namespace, string i_Name, List<string> i_Ids, string i_FolderName, bool i_IncludeNone = true)
		{
			ScriptCreator creator = new ScriptCreator()
			{
				Namespace = i_Namespace,
				Name = i_Name + "Enum",
				LocalDirectory = i_FolderName,
				ScriptType = ScriptCreator.Type.Class,
			};

			creator.AddUsing("MM.CollectionEnum");
			creator.AddExtend(EXTENDS_TEMPLATE.Replace("{name}", i_Name));

			string field = FIELD_TEMPLATE.Replace("{name}", i_Name);

			if (i_IncludeNone)
			{
				creator.AddField(FormatField(field, CollectionValue.NULL_VALUE));
			}

			foreach (string id in i_Ids)
			{
				creator.AddField(FormatField(field, id));
			}

			creator.Create();
		}

		public static void CreateInstance(string i_ClassName, string i_Folder)
		{
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/Scripts/Generated/{i_Folder}/{i_ClassName}.asset");
			if (File.Exists(Path.Combine(Application.dataPath, assetPathAndName.Replace("Assets", ""))))
			{
				return;
			}
			var asset = ScriptableObject.CreateInstance(i_ClassName);
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();
		}
		private static string FormatField(string i_Field, string i_ID)
		{
			return i_Field.Replace("{field}", CleanUp(i_ID)).Replace("{id}", i_ID);
		}

		private static string CleanUp(string i_ID)
		{
			string value = Regex.Replace(i_ID, "[^A-Za-z0-9_]", "_");
			if (char.IsDigit(value[0]))
			{
				value = "_" + value;
			}

			return value;
		}
	}
}
