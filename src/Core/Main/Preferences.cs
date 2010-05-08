/*
 * Copyright (C) 2008 Jordi Mas i Hernàndez <jmas@softcatala.org>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace gbrainy.Core.Main
{
	public static class Preferences
	{
		static string file, config_path;
		static SerializableDictionary <string, string > properties;

		public const string MemQuestionWarnKey = "MemQuestionWarn";
		public const string MemQuestionTimeKey = "MemQuestionTime";
		public const string DifficultyKey = "Difficulty";
		public const string MinPlayedGamesKey = "MinPlayedGames";
		public const string MaxStoredGamesKey = "MaxStoredGames";
		public const string ToolbarKey = "Toolbar";
		public const string ColorBlindKey = "ColorBlind";

		const string element_item = "item";
		const string element_key = "key";
		const string element_value = "value";
		const string element_collection = "collection";

	    	public class SerializableDictionary <TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
		{
			public System.Xml.Schema.XmlSchema GetSchema ()
			{
				return null;
			}

			public void ReadXml (System.Xml.XmlReader reader)
			{
				XmlSerializer key_serializer = new XmlSerializer (typeof (TKey));
				XmlSerializer value_serializer = new XmlSerializer (typeof (TValue));
		 		bool wasEmpty = reader.IsEmptyElement;

			    	reader.Read ();

				if (wasEmpty)
					return;
			
				reader.ReadStartElement (element_collection);
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					reader.ReadStartElement (element_item);

					reader.ReadStartElement (element_key);
					TKey key = (TKey) key_serializer.Deserialize (reader);
					reader.ReadEndElement ();

					reader.ReadStartElement (element_value);
					TValue value = (TValue) value_serializer.Deserialize (reader);
					reader.ReadEndElement();

					this[key] = value; // already created in DefaultValues
					reader.ReadEndElement();

					reader.MoveToContent();
				}
				reader.ReadEndElement();
			}

			public void WriteXml (System.Xml.XmlWriter writer)
			{
				XmlSerializer key_serializer = new XmlSerializer (typeof(TKey));
				XmlSerializer value_serializer = new XmlSerializer (typeof(TValue));

				writer.WriteStartElement (element_collection);
				foreach (TKey key in this.Keys)
				{
					writer.WriteStartElement (element_item);
					writer.WriteStartElement (element_key);

					key_serializer.Serialize (writer, key);
					writer.WriteEndElement ();
					writer.WriteStartElement (element_value);

					TValue value = this[key];
					value_serializer.Serialize (writer, value);
					writer.WriteEndElement ();
					writer.WriteEndElement ();
				}
				writer.WriteEndElement ();
			}

	    	}

		static Preferences ()
		{
			properties = new SerializableDictionary <string, string> ();
			config_path = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			config_path = Path.Combine (config_path, Defines.CONFIG_DIR);
			file = Path.Combine (config_path, "Preferences.xml");
			Load ();
		}

		public static void Save ()
		{
			try {
				if (!Directory.Exists (config_path))
					Directory.CreateDirectory (config_path);

				XmlTextWriter writer = new XmlTextWriter (file, Encoding.UTF8);
				writer.Formatting = Formatting.Indented;

				properties.WriteXml (writer);
				writer.Close ();
			}		
			catch (Exception)
			{
			}
		}
	
		public static int GetIntValue (string key)
		{
			return Int32.Parse (properties [key]);
		}

		public static bool GetBoolValue (string key)
		{
			return Boolean.Parse (properties [key]);
		}

		public static void SetIntValue (string key, int value)
		{
			properties[key] = value.ToString ();
		}

		public static void SetBoolValue (string key, bool value)
		{
			properties [key] = value.ToString ();
		}

		public static void LoadDefaultValues ()
		{
			properties.Clear ();
			properties.Add (MemQuestionWarnKey, true.ToString ());
			properties.Add (MemQuestionTimeKey, "4");
			properties.Add (DifficultyKey, ((int)(Game.Difficulty.Medium)).ToString ());
			properties.Add (MinPlayedGamesKey, "5");
			properties.Add (MaxStoredGamesKey, "20");
			properties.Add (ToolbarKey, true.ToString ());
			properties.Add (ColorBlindKey, false.ToString ());
		}

		static void Load ()
		{
			try {
				LoadDefaultValues ();
				XmlTextReader reader = new XmlTextReader (file);
				properties.ReadXml (reader);
				reader.Close ();
			}
			catch (Exception)
			{
			}
		}
	
	}
}
