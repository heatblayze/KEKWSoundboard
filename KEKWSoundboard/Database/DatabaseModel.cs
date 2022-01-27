using System.Collections.Generic;

namespace KEKWSoundboard.Database
{
    public enum CaptureMixMode
    {
        LeftRight,
        Left,
        Right
    }

    public class DatabaseModel
    {
        public int LastId { get; set; } = -1;
        public int LastProfileIndex { get; set; } = 0;
        public DatabasePreferences Preferences { get; set; } = new DatabasePreferences();
        public List<DatabaseProfile> Profiles { get; set; } = new List<DatabaseProfile> { };
    }

    public class DatabasePreferences
    {
        public string PrimaryRenderDevice { get; set; }
        public float PrimaryRenderDeviceVolume { get; set; } = 1;

        public string SecondaryRenderDevice { get; set; }
        public float SecondaryRenderDeviceVolume { get; set; } = 1;

        public string CaptureDevice { get; set; }
        public float CaptureDeviceVolume { get; set; } = 1;
        public CaptureMixMode CaptureMixMode { get; set; }

        public DatabasePreferences Clone()
        {
            return (DatabasePreferences)this.MemberwiseClone();
        }
    }

    public class DatabaseProfile
    {
        public string Name { get; set; }
        public List<int> ChildIds { get; set; } = new List<int>();
        public List<DatabaseEntity> Entities { get; set; } = new List<DatabaseEntity>();
    }

    public class DatabaseEntity
    {
        public int Id { get; set; } = -1;
        public int? ParentId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string ImageFile { get; set; }

        public virtual DatabaseEntity Clone()
        {
            return (DatabaseEntity)this.MemberwiseClone();
        }
    }

    public class DatabaseFolder : DatabaseEntity
    {
        public List<int> ChildIds { get; set; } = new List<int>();

        public override DatabaseFolder Clone()
        {
            return (DatabaseFolder)this.MemberwiseClone();
        }
    }

    public class DatabaseSound : DatabaseEntity
    {
        public string SoundFile { get; set; }
        public float Volume { get; set; }

        public override DatabaseSound Clone()
        {
            return (DatabaseSound)this.MemberwiseClone();
        }
    }
}
