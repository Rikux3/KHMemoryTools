using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;


namespace KHTriggerRunner
{
    public class TriggerSerializer
    {
        public static void SerializeTrigger(ObservableCollection<ITrigger> triggers)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Trigger file (*.kht)|*.kht";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = File.Open(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, triggers);
                }
            }
        }

        public static ObservableCollection<ITrigger> DeserializeTrigger()
        {
            ObservableCollection<ITrigger> list = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trigger file (*.kht)|*.kht";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = File.Open(ofd.FileName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    list = (ObservableCollection<ITrigger>)formatter.Deserialize(stream);
                }
            }
            return list;
        }
        //Hat hier nichts zu suchen, bei gelegenheit verschieben
        public static string[] ReadCodeList()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text file (*.txt)|*.txt";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return File.ReadAllLines(ofd.FileName);
            }
            return new string[0];
        }
    }
}
