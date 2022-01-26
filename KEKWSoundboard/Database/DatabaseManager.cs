using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KEKWSoundboard.Database
{
    internal class DatabaseManager
    {
        public static DatabaseManager Instance { get; private set; }

        string _rootDirectory;
        DatabaseModel _data;
        string _dataPath;
        int _profileIndex;

        public DatabaseManager(string rootDirectory)
        {
            Instance = this;

            // Ensure the root directory exists
            _rootDirectory = rootDirectory;
            if (!Directory.Exists(_rootDirectory))
            {
                Directory.CreateDirectory(_rootDirectory);
            }

            // Try to load existing data
            _dataPath = Path.Combine(rootDirectory, "data.json");
            if (File.Exists(_dataPath))
            {
                var rawData = File.ReadAllText(_dataPath);
                try
                {
                    _data = JsonConvert.DeserializeObject<DatabaseModel>(rawData);
                }
                catch (Exception e)
                {
                    // TODO: prompt user there was an error
                    Console.WriteLine(e);
                }
            }

            if (_data == null)
            {
                _data = new DatabaseModel();
                _data.Profiles.Add(new DatabaseProfile()
                {
                    Name = "Default"
                });
            }

            _profileIndex = _data.LastProfileIndex;
        }

        public List<DatabaseEntity> GetEntitiesInFolder(int? folderId)
        {
            List<int> ids;

            if (folderId == null)
            {
                // Use the root children (no folder)
                ids = _data.Profiles[_profileIndex].ChildIds;
            }
            else
            {
                // Try to find the folder, then find its contents
                var folder = _data.Profiles[_profileIndex].Entities.FirstOrDefault(x => x.Id == folderId);
                if (folder == null || folder is not DatabaseFolder)
                    return null;
                ids = (folder as DatabaseFolder).ChildIds;
            }

            // Create a copy of all of the entities (to avoid editing)
            List<DatabaseEntity> entities = new List<DatabaseEntity>();
            foreach (var id in ids)
            {
                entities.Add(_data.Profiles[_profileIndex].Entities.First(x => x.Id == id).Clone());
            }

            return entities;
        }

        #region Add Entity

        public bool AddFolder(DatabaseFolder folder)
        {
            if (!TryAddEntity(folder))
                return false;
            SaveData();
            return true;
        }

        public bool AddSound(DatabaseSound sound)
        {
            if (!TryAddEntity(sound))
                return false;

            // Copy the sound to the entity directory
            var entityFolder = Path.Combine(_rootDirectory, sound.Id.ToString());
            if (!string.IsNullOrEmpty(sound.SoundFile))
            {
                var ext = Path.GetExtension(sound.SoundFile);
                var updatedSoundPath = Path.Combine(entityFolder, "sound" + ext);
                File.Copy(sound.SoundFile, updatedSoundPath, true);

                // Sound now points to copied file
                sound.SoundFile = updatedSoundPath;
            }
            SaveData();
            return true;
        }

        bool TryAddEntity(DatabaseEntity entity)
        {
            // Assign it a new ID
            entity.Id = ++_data.LastId;
            if (entity.ParentId == null)
            {
                // Add to root
                _data.Profiles[_profileIndex].ChildIds.Add(entity.Id);
            }
            else
            {
                // Try to find the correct folder
                var folder = _data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.ParentId);
                if (folder == null || folder is not DatabaseFolder)
                    return false;
                (folder as DatabaseFolder).ChildIds.Add(entity.Id);
            }
            _data.Profiles[_profileIndex].Entities.Add(entity);

            // Create the folder for the entity
            var entityFolder = Path.Combine(_rootDirectory, entity.Id.ToString());
            if (!Directory.Exists(entityFolder))
            {
                Directory.CreateDirectory(entityFolder);
            }

            // Copy the image to the folder
            if (!string.IsNullOrEmpty(entity.ImageFile))
            {
                var ext = Path.GetExtension(entity.ImageFile);
                var updatedImagePath = Path.Combine(entityFolder, "icon" + ext);
                File.Copy(entity.ImageFile, updatedImagePath, true);

                // Image now points to copied file
                entity.ImageFile = updatedImagePath;
            }
            return true;
        }

        #endregion

        #region Update Entity

        public bool UpdateFolder(DatabaseFolder folderEntity)
        {
            var folder = folderEntity.Clone();

            if (!TryUpdateEntityInfo(folder))
                return false;
            UpdateEntityInDatabase(folder);
            SaveData();
            return true;
        }

        public bool UpdateSound(DatabaseSound soundEntity)
        {
            var sound = soundEntity.Clone();

            if (!TryUpdateEntityInfo(sound))
                return false;

            var oldEntity = _data.Profiles[_profileIndex].Entities.First(x => x.Id == sound.Id);
            if (oldEntity.ImageFile != sound.ImageFile)
            {
                // Copy the image to the folder
                if (!string.IsNullOrEmpty(sound.SoundFile))
                {
                    var entityFolder = Path.Combine(_rootDirectory, sound.Id.ToString());

                    var ext = Path.GetExtension(sound.SoundFile);
                    var updatedSoundPath = Path.Combine(entityFolder, "sound" + ext);
                    File.Copy(sound.SoundFile, updatedSoundPath, true);

                    // Sound now points to copied file
                    sound.SoundFile = updatedSoundPath;
                }
            }

            UpdateEntityInDatabase(sound);
            SaveData();
            return true;
        }

        bool TryUpdateEntityInfo(DatabaseEntity entity)
        {
            var oldEntity = _data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.Id);

            if (oldEntity.ParentId != entity.ParentId)
            {
                // Entity has been moved

                // Update the new parent
                if (entity.ParentId == null)
                {
                    // Add to root
                    _data.Profiles[_profileIndex].ChildIds.Add(entity.Id);
                }
                else
                {
                    // Try to find the correct folder
                    var folder = _data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.ParentId);
                    if (folder == null || folder is not DatabaseFolder)
                        return false;
                    (folder as DatabaseFolder).ChildIds.Add(entity.Id);
                }

                // Update the old parent
                if (oldEntity.ParentId == null)
                {
                    // Add to root
                    _data.Profiles[_profileIndex].ChildIds.Remove(oldEntity.Id);
                }
                else
                {
                    // Try to find the correct folder
                    var folder = _data.Profiles[_profileIndex].Entities.First(x => x.Id == oldEntity.ParentId);
                    if (folder == null || folder is not DatabaseFolder)
                        return false;
                    (folder as DatabaseFolder).ChildIds.Remove(oldEntity.Id);
                }
            }

            // Create the folder for the entity
            var entityFolder = Path.Combine(_rootDirectory, entity.Id.ToString());
            if (!Directory.Exists(entityFolder))
            {
                Directory.CreateDirectory(entityFolder);
            }

            if (oldEntity.ImageFile != entity.ImageFile)
            {
                // Copy the image to the folder
                if (!string.IsNullOrEmpty(entity.ImageFile))
                {
                    var ext = Path.GetExtension(entity.ImageFile);
                    var updatedImagePath = Path.Combine(entityFolder, "icon" + ext);
                    File.Copy(entity.ImageFile, updatedImagePath, true);

                    // Image now points to copied file
                    entity.ImageFile = updatedImagePath;
                }
            }

            return true;
        }

        void UpdateEntityInDatabase(DatabaseEntity entity)
        {
            // Find the index of the matching entity ID and update it with the new one
            var entityIndex = _data.Profiles[_profileIndex].Entities.IndexOf(_data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.Id));
            _data.Profiles[_profileIndex].Entities[entityIndex] = entity;
        }

        #endregion

        #region Delete Entity

        public void DeleteEntity(DatabaseEntity entity)
        {
            // Remove from main list of entities
            var entityIndex = _data.Profiles[_profileIndex].Entities.IndexOf(_data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.Id));
            _data.Profiles[_profileIndex].Entities.RemoveAt(entityIndex);

            if (entity.ParentId == null)
            {
                // Remove from root
                _data.Profiles[_profileIndex].ChildIds.Remove(entity.Id);
            }
            else
            {
                // Try to remove from folder
                var folder = _data.Profiles[_profileIndex].Entities.First(x => x.Id == entity.ParentId);
                if (folder != null && folder is DatabaseFolder)
                    (folder as DatabaseFolder).ChildIds.Remove(entity.Id);
            }

            SaveData();
        }

        #endregion

        public List<string> GetProfiles()
        {
            return _data.Profiles.Select(x => x.Name).ToList();
        }

        public void SetProfileIndex(int index)
        {
            _profileIndex = index;
            _data.LastProfileIndex = _profileIndex;
            SaveData();
        }

        void SaveData()
        {
            var rawData = JsonConvert.SerializeObject(_data);
            File.WriteAllText(_dataPath, rawData);
        }
    }
}
