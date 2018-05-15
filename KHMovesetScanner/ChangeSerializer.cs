using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
namespace KHMovesetMemory
{
    class ChangeSerializer
    {
        public static void SerializeChangelist(Changelog log)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "MSET RAM mod (*.msetram)|*.msetram";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = File.Open(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, log);
                }
            }
        }

        public static Changelog DeserializeChangelist()
        {
            Changelog list = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MSET RAM mod (*.msetram)|*.msetram";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = File.Open(ofd.FileName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    list = (Changelog)formatter.Deserialize(stream);
                }
            }
            return list;
        }
    }
}
