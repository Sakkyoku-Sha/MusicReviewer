using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
   
    public enum TagType
    {
        Language = 'L',
        Genre = 'G',
        Instrument = 'I',
        Invalid = 'X'
    };
    public class TagObject
    {
        public static TagType ConvertToTagType(char type)
        {
            switch (type)
            {
                case 'L':
                    return TagType.Language;
                case 'G':
                    return TagType.Genre;
                case 'I':
                    return TagType.Instrument;
            }

            return TagType.Invalid;
        }

        public string Name { get; set; }
        public TagType Type {get; set; }

        public TagObject(string Name, char Type)
        {
            this.Name = Name;
            this.Type = ConvertToTagType(Type); 
        }
        public TagObject(string Name, TagType Type)
        {
            this.Name = Name;
            this.Type = Type; 
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(TagObject)) { return false; }
            if((obj as TagObject).Name.Equals(this.Name) && (obj as TagObject).Type == this.Type) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
