using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

public static class SaveSystem
{

    public static void SaveOptionsData(MainMenuUIController mc)
    {

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/options.savesettings";
        FileStream stream = new FileStream(path, FileMode.Create);

        OptionsData data = new OptionsData(mc);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static OptionsData LoadOptionsData()
    {

        string path = Application.persistentDataPath + "/options.savesettings";
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            OptionsData data = formatter.Deserialize(stream) as OptionsData;
            stream.Close();

            return data;

        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return new OptionsData();
        }

    }

}
