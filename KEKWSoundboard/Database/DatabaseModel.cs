using System.Collections.Generic;

namespace KEKWSoundboard.Database
{
    public class DatabaseModel
    {
        public int LastId { get; set; } = -1;
        public int LastProfileIndex { get; set; } = 0;
        public List<DatabaseProfile> Profiles { get; set; } = new List<DatabaseProfile> { };
    }

    public class DatabaseProfile
    {
        public string Name { get; set; }
        public List<int> ChildIds { get; set; } = new List<int>();
        public List<DatabaseEntity> Entities { get; set; } = new List<DatabaseEntity>();
    }

    public class DatabaseEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string ImageFile { get; set; }

        public DatabaseEntity Clone()
        {
            return (DatabaseEntity)this.MemberwiseClone();
        }
    }

    public class DatabaseFolder : DatabaseEntity
    {
        public List<int> ChildIds { get; set; } = new List<int>();

        new public DatabaseFolder Clone()
        {
            return (DatabaseFolder)this.MemberwiseClone();
        }
    }

    public class DatabaseSound : DatabaseEntity
    {
        public string SoundFile { get; set; }
        public float Volume { get; set; }

        new public DatabaseSound Clone()
        {
            return (DatabaseSound)this.MemberwiseClone();
        }
    }
}
