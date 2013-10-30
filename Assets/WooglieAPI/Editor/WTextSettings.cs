using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class WTextSettings
{
    public class WSettingNode
    {
        public Dictionary<string, string> settings = new Dictionary<string, string>();
        public bool loaded = false;
    }

    static Dictionary<string, WSettingNode> fileSettings = new Dictionary<string, WSettingNode>();

    Dictionary<string, string> settings
    {
        get
        {
            return fileSettings[fileName].settings;
        }
    }

    bool loaded
    {
        get
        {
            return fileSettings[fileName].loaded;
        }
        set
        {
            fileSettings[fileName].loaded = value;
        }
    }

    string fileName = "";


    public WTextSettings(string input)
    {
        // Debug.Log("NEW TXS ");
        fileName = input;
        if (!fileSettings.ContainsKey(fileName))
            fileSettings.Add(fileName, new WSettingNode());
        ReadSettings();
    }


    public string GetString(string title, string defaultValue)
    {
        string val = GetString(title);
        if (val == "")
            return defaultValue;
        else
            return val;
    }

    public string GetString(string title)
    {
        if (!loaded) ReadSettings();
        if (settings.ContainsKey(title))
        {
            return settings[title];
        }
        return "";
    }
    public void SetString(string title, string value)
    {
        if (!loaded) ReadSettings();
        settings[title] = value;
        SaveSettings();
    }

    //INT
    public int GetInt(string title, int defaultValue)
    {
        string val = GetString(title);
        if (val == "")
            return defaultValue;
        else
            return int.Parse(val);
    }
    public void SetInt(string title, int value)
    {
        if (!loaded) ReadSettings();
        settings[title] = value + "";
        SaveSettings();
    }

    //BOOL
    public bool GetBool(string title, bool defaultValue)
    {
        string val = GetString(title);
        if (val == "")
            return defaultValue;
        else
            return val == "1";
    }
    public void SetBool(string title, bool value)
    {
        if (!loaded) ReadSettings();
        settings[title] = value ? "1" : "0";
        SaveSettings();
    }

    void ReadSettings()
    {
        //Debug.Log("READ");
        if (File.Exists(fileName))
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            string lastElementName = "";
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        lastElementName = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        settings.Add(lastElementName, reader.Value);
                        // Debug.Log("Got " + lastElementName+"="+reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        //Debug.Log("</" + reader.Name + ">");
                        break;
                    default:
                        // Debug.Log("ELSE " + reader.Name);
                        break;
                }
            }
            reader.Close();
        }
        loaded = true;
    }


    void SaveSettings()
    {
        //  Debug.Log("save");
        XmlTextWriter writer = new XmlTextWriter(fileName, null);
        writer.WriteStartElement("settings");
        foreach (KeyValuePair<string, string> kvp in settings)
            writer.WriteElementString(kvp.Key, kvp.Value);
        writer.WriteEndElement();
        writer.Flush();
        writer.Close();
    }

}
