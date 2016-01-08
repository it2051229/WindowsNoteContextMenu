using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteContextMenu
{
    public partial class FormMain : Form
    {
        private string NOTES_FILENAME = AppDomain.CurrentDomain.BaseDirectory + "\\notes.dat";
        private Dictionary<string, string> notes;

        /// <summary>
        /// Register for context menu
        /// </summary>
        /// <param name="args">Arguments</param>
        public FormMain(string[] args)
        {
            InitializeComponent();

            notes = new Dictionary<string, string>();
            loadNotes();

            
            if(args.Length > 0)
            {
                textBoxPath.Text = args[0];

                if (notes.ContainsKey(args[0]))
                {
                    textBoxNote.Text = notes[args[0]];
                }
            }
        }

        /// <summary>
        /// Save all notes to file
        /// </summary>
        private void saveNotes()
        {
            Stream stream = File.Open(NOTES_FILENAME, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, notes);
            stream.Close();
        }

        /// <summary>
        /// Load all notes from file
        /// </summary>
        private void loadNotes()
        {
            try
            {
                Stream stream = File.Open(NOTES_FILENAME, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                notes = (Dictionary<string, string>)formatter.Deserialize(stream);
                stream.Close();
            }
            catch { }
        }

        /// <summary>
        /// Unregister the context menu
        /// </summary>
        private void unregisterContextMenu()
        {
            FileShellExtension.Unregister("*", "note");
            FileShellExtension.Unregister("Directory", "note");
        }

        /// <summary>
        /// Save the note
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (textBoxPath.Text == string.Empty || textBoxNote.Text.Trim() == string.Empty)
                return;

            if (notes.ContainsKey(textBoxPath.Text))
            {
                // Update existing note
                notes[textBoxPath.Text] = textBoxNote.Text.Trim();
            }
            else
            {
                // Add new note
                notes.Add(textBoxPath.Text, textBoxNote.Text.Trim());
            }

            saveNotes();
            Close();
        }

        /// <summary>
        /// Add the note option to the windows context menu
        /// </summary>
        private void registerNoteToContextMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // full path to self, %L is placeholder for selected file
            string menuCommand = string.Format("\"{0}\" \"%L\"", Application.ExecutablePath);

            // register the context menu to directory, files, and drives
            FileShellExtension.Register("*", "note", "Note", menuCommand);
            FileShellExtension.Register("Directory", "note", "Note", menuCommand);

            MessageBox.Show("Ok, 'note' menu is now available to your files and folders.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Remove the note option from the windows context menu
        /// </summary>
        private void unregisterNoteFromContextMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileShellExtension.Unregister("*", "note");
            FileShellExtension.Unregister("Directory", "note");

            MessageBox.Show("Ok, 'note' menu has been removed from your files and folders.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Close the program
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Show about
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("www.it2051229.com", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Delete all notes that are unlinked (no related files)
        /// </summary>
        private void cleanNotesDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string, string> entry in notes)
            {
                if(Directory.Exists(entry.Key) || File.Exists(entry.Key))
                {
                    // Don't delete note if directory file exists
                    continue;
                }

                notes.Remove(entry.Key);
            }

            saveNotes();
            MessageBox.Show("All notes which has its related files moved/removed has been deleted.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
