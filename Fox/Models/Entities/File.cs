using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fox.Models.Entities
{
    public class File
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FullPath { get; set; }
        public string PathToDirectory { get; set; }
        public List<string> Tags { get; set; }
        public bool Changed { get; set; }

        public File(string filePath)
        {
            this.Name = this.GetFileNameWithoutTagsAndExtension(filePath);
            this.Extension = Path.GetExtension(filePath);
            this.FullPath = filePath;
            this.PathToDirectory = Path.GetDirectoryName(FullPath);
            this.Tags = GetTagsFromFile(filePath);
        }

        private List<string> GetTagsFromFile(string filePath) => Path.GetFileNameWithoutExtension(filePath).Split('_')?.Skip(1).ToList();

        private string GetFileNameWithoutTagsAndExtension(string filePath) => Path.GetFileNameWithoutExtension(filePath).Split('_')[0];

        public void AddTag(string tag)
        {
            if (!this.Tags.Contains(tag))
                this.Tags.Add(tag);

            this.Changed = true;
        }

        public void RemoveTag(string tag)
        {
            if (this.Tags.Contains(tag))
                this.Tags.Remove(tag);

            this.Changed = true;
        }

        public void Save()
        {
            var newPath = this.ToString();

            System.IO.File.Move(this.FullPath, this.ToString());

            this.Name = this.Name = this.GetFileNameWithoutTagsAndExtension(newPath);
            this.FullPath = newPath;

            this.Changed = false;
        }

        public override string ToString()
        {
            this.Tags.Sort(); // Sort the tags

            if (this.Tags.Count() != 0) 
                return $"{this.PathToDirectory}\\{this.Name}_{string.Join("_", this.Tags.ToArray())}{this.Extension}";
            else
                return $"{this.PathToDirectory}\\{this.Name}{this.Extension}";
        }
    }
}
