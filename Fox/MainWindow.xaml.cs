﻿using Fox.Models;
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
using System.Reflection;
using System.Diagnostics;

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
        private string _tagLibraryPath;
        private Process _photoViewer;

        public MainWindow(string filePath)
        {
            // Start photo viewer
            this._photoViewer = Process.Start(filePath);

            // Get file as a workable entity
            this._file = new File(filePath);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            this._tagLibraryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Hivemind Software\\Fox\\{Properties.Settings.Default.TagStorage}";

            InitializeComponent();

            // Set title of window to the name of the file
            this.Title = $"Fox - Tagging {_file.Name}{_file.Extension}";

            PopulateTagList();
        }

        /// <summary>
        /// Populates the list of tags in the view
        /// </summary>
        public void PopulateTagList()
        {
            if (!System.IO.File.Exists(this._tagLibraryPath))
            {
                using (var tagFile = System.IO.File.Create((this._tagLibraryPath)))
                {
                    tagFile.Close();
                }
            }

            this._tags = System.IO.File.ReadAllLines(this._tagLibraryPath).ToList();

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
        
        }

        /// <summary>
        /// Saves the tags to the tag file library
        /// </summary>
        private void SaveTags()
        {
            // Sort the tags
            this._tags.Sort();

            // Save the tags to the tag library
            System.IO.File.WriteAllLines(this._tagLibraryPath, this._tags.ToArray());
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

                ToggleSaveButton();
            }

            NewTagTextBox.Text = string.Empty;
        }

        private void TagItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var item = (TagItem)((System.Windows.Controls.CheckBox)sender).DataContext;

            this._file.AddTag(item.Tag);

            ToggleSaveButton();
        }

        private void TagItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var item = (TagItem)((System.Windows.Controls.CheckBox)sender).DataContext;

            this._file.RemoveTag(item.Tag);

            ToggleSaveButton();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Sorts the tags
            this._tagItems.Sort();

            // Saves the file
            this._file.Save();

            ToggleSaveButton();
        }

        /// <summary>
        /// Toggles the save button depending on if changes have been made
        /// </summary>
        public void ToggleSaveButton()
        {
            SaveButton.IsEnabled = this._file.Changed;
            SaveButton.Content = this._file.Changed ? "Save changes" : "Changes have been saved";
        }

        /// <summary>
        /// When the user presses enter on the new tag input it calls the same method as if you'd have pressed the add new tag button
        /// </summary>
        private void NewTagTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewTagButton_Click(sender, e);
            }
        }
    }
}
