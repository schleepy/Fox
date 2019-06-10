using System;

namespace Fox.Models
{
    public class TagItem : IComparable
    {
        public string Tag { get; set; }
        public bool Checked { get; set; }

        public int CompareTo(object obj)
        {
            TagItem a = this;
            TagItem b = (TagItem)obj;

            if (a == b)
                return 0;

            if ((a.Checked && b.Checked) || (!a.Checked && !b.Checked))
            {
                return string.Compare(b.Tag, a.Tag);
            }

            if (a.Checked && !b.Checked)
                return 1;
            else
                return -1;
        }
    }
}
