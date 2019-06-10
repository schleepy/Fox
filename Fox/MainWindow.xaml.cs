using Fox.Models;
using Fox.Models.Entities;
using Fox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Fox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> _tags;
        public ObservableCollection<TagItem> _tagItems;
        private File _file;

        /*public MainWindow()
        {
            InitializeComponent();
        }*/

        public MainWindow(string filePath)
        {
            // Get file as a workable entity
            this._file = new File(filePath);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            InitializeComponent();

            // Set title of window to the name of the file
            this.Title = $"Tagging {_file.Name}";

            PopulateTagList();
        }

        public void PopulateTagList()
        {
            if (!System.IO.File.Exists(Properties.Settings.Default.TagStorage))
            {
                using (var tagFile = System.IO.File.Create((Properties.Settings.Default.TagStorage)))
                {
                    tagFile.Close();
                }
            }

            this._tags = System.IO.File.ReadAllLines(Properties.Settings.Default.TagStorage).ToList();

            var difference = _file.Tags.Except(this._tags);

            if (difference.Count() != 0)
            {
                if (MessageBox.Show($"This file contains tags that are not in your tag library:\n{string.Join("\n", difference.ToArray())}\n\nWould you like to add them to your library?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    this._tags.AddRange(difference);
                    SaveTags();
                }

                this._tags.AddRange(difference);
            }

            this._tagItems = new ObservableCollection<TagItem>(this._tags.Select(x => new TagItem() { Tag = x, Checked = this._file.Tags.Contains(x) }));

            this._tagItems.Sort();

            TagListBox.ItemsSource = this._tagItems;
        }

        /// <summary>
        /// On environment exit
        /// </summary>
        private void OnProcessExit(object sender, EventArgs e)
        {
            //SaveTags();
        }

        /// <summary>
        /// Saves the tags to the tag file library
        /// </summary>
        private void SaveTags()
        {
            // Sort the tags
            this._tags.Sort();

            // Save the tags to the tag library
            System.IO.File.WriteAllLines(Properties.Settings.Default.TagStorage, this._tags.ToArray());
        }

        private void AddNewTagButton_Click(object sender, RoutedEventArgs e)
        {
            var newTag = NewTagTextBox.Text;

            if (!string.IsNullOrEmpty(newTag))
            {
                if (!this._tags.Contains(newTag))
                {
                    this._tags.Add(newTag);

                    SaveTags();

                    this._tagItems.Add(new TagItem
                    {
                        Tag = newTag,
                        Checked = true
                    });

                    this._tagItems.Sort();
                }
                else
                {
                    this._tagItems.First(x => x.Tag == newTag).Checked = true;

                    this._tagItems.Sort();
                }

                this._file.AddTag(newTag);
            }
        }

        private void AddNewTag(string tag)
        {

        }

        private void TagItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var item = (TagItem)((System.Windows.Controls.CheckBox)sender).DataContext;

            this._file.AddTag(item.Tag);
        }

        private void TagItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var item = (TagItem)((System.Windows.Controls.CheckBox)sender).DataContext;

            this._file.RemoveTag(item.Tag);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this._tagItems.Sort();

            this._file.Save();
        }

        private void NewTagTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewTagButton_Click(sender, e);
            }
        }
    }
}
