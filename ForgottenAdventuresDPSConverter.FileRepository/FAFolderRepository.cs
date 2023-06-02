using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class FAFolderRepository : Repository<FAFolder>
    {
        #region id of elements within a split line
        private const int folderName = 1;
        private const int folderRelativePath = 2;
        private const int folderParentId = 3;
        private const int folderHasItems = 4;
        private const int folderNumberId = 5;
        private const int folderFolderId = 6;
        private const int folderSubfolderId = 7;
        private const int folderIsFloor = 8;
        private const int folderOtherCommands = 9;
        private const int folderCommands = 10;
        private const int folderNotes = 11;
        #endregion
        
        public FAFolderRepository(IFileRepositorySettings settings) : base(settings.FAFolderRepositoryFilePath, settings) { }

        protected override string CreateEntityLine(FAFolder folder)
        {
            Dictionary<int, string> stringParts = new Dictionary<int, string>();
            stringParts.Add(id, folder.Id.ToString());
            stringParts.Add(folderName, folder.Name);
            stringParts.Add(folderRelativePath, folder.RelativePath);
            if (folder.ParentId == null)
            {
                stringParts.Add(folderParentId, string.Empty);
            }
            else
            {
                stringParts.Add(folderParentId, folder.ParentId.ToString());
            }
            stringParts.Add(folderHasItems, folder.HasItems.ToString());
            if (folder.NumberId == null)
            {
                stringParts.Add(folderNumberId, string.Empty);
            }
            else
            {
                stringParts.Add(folderNumberId, folder.NumberId.ToString());
            }
            if (folder.FolderId == null)
            {
                stringParts.Add(folderFolderId, string.Empty);
            }
            else
            {
                stringParts.Add(folderFolderId, folder.FolderId.ToString());
            }
            if (folder.SubfolderId == null)
            {
                stringParts.Add(folderSubfolderId, string.Empty);
            }
            else
            {
                stringParts.Add(folderSubfolderId, folder.SubfolderId.ToString());
            }
            stringParts.Add(folderIsFloor, folder.IsFloor.ToString());
            stringParts.Add(folderOtherCommands, folder.OtherCommands.ToString());
            stringParts.Add(folderCommands, folder.Commands);
            stringParts.Add(folderNotes, folder.Notes);

            string returnString = string.Empty;
            for (int i = 0; i < stringParts.Count; i++)
            {
                if (stringParts.ContainsKey(i))
                {
                    if (returnString != string.Empty)
                    {
                        returnString += itemBreak;
                    }
                    returnString += stringParts[i];
                }
                else
                {
                    throw new Exception("there is a gap in the Ids for creating a folder in the CreateEntityLine method ForgottenAdventuresDPSConverter.FileRepository.FAFolderRepository which means that either some property is missing within the method or that the folder identifiers are missing a number which will create problems in reading too");
                }
            }

            return returnString;
        }
        
        internal async override Task<bool> DeleteReferences<U>(int id)
        {
            if(typeof(U) == typeof(FAFolder))
            {
                /*
                 * if a type of FAFolder gets deleted all items with the same type need to get deleted
                 * aka
                 * on delete cascade
                 */

                IEnumerable<FAFolder> children = await GetAll(f => f.ParentId == id);//get all the children that will need to be deleted

                Task<bool>[] deleteTasks = new Task<bool>[children.Count()];//get a task for each child that needs to be deleted
                int index = 0;
                foreach (FAFolder child in children)
                {
                    deleteTasks[index] = Delete(child.Id);//get a task with each child that will be deleted
                    index++;
                }

                bool completed = true;//if the task was successful
                foreach(Task<bool> deleteTask in deleteTasks)
                {
                    completed = completed && await deleteTask;//completed is only true if every deletion task was successful
                }

                return completed;
            }
            else if(typeof(U) == typeof(DpsNumber))
            {
                /*
                 * if a type of DpsNumber gets deleted all items that reference that number need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                IEnumerable<FAFolder> folders = await GetAll(f => f.NumberId == id);//get all folders that need to be updated

                Task<bool>[] updateTasks = new Task<bool>[folders.Count()];//get a task for each folder that needs to be updated
                int index = 0;

                foreach(FAFolder folder in folders)
                {
                    folder.NumberId = null;//set reference to null
                    updateTasks[index] = Update(folder);//get a task for each folder with a deleted reference
                }

                bool completed = true;//if the task was successful
                foreach (Task<bool> updateTask in updateTasks)
                {
                    completed = completed && await updateTask;//completed is only true if every update task was successful
                }

                return completed;
            }
            else if (typeof(U) == typeof(DpsFolder))
            {
                /*
                 * if a type of DpsFolder gets deleted all items that reference that folder need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                IEnumerable<FAFolder> folders = await GetAll(f => f.FolderId == id);//get all folders that need to be updated

                Task<bool>[] updateTasks = new Task<bool>[folders.Count()];//get a task for each folder that needs to be updated
                int index = 0;

                foreach (FAFolder folder in folders)
                {
                    folder.FolderId = null;//set reference to null
                    updateTasks[index] = Update(folder);//get a task for each folder with a deleted reference
                }

                bool completed = true;//if the task was successful
                foreach (Task<bool> updateTask in updateTasks)
                {
                    completed = completed && await updateTask;//completed is only true if every update task was successful
                }

                return completed;
            }
            else if (typeof(U) == typeof(DpsSubfolder))
            {
                /*
                 * if a type of DpsSubfolder gets deleted all items that reference that folder need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                IEnumerable<FAFolder> folders = await GetAll(f => f.SubfolderId == id);//get all folders that need to be updated

                Task<bool>[] updateTasks = new Task<bool>[folders.Count()];//get a task for each folder that needs to be updated
                int index = 0;

                foreach (FAFolder folder in folders)
                {
                    folder.SubfolderId = null;//set reference to null
                    updateTasks[index] = Update(folder);//get a task for each folder with a deleted reference
                }

                bool completed = true;//if the task was successful
                foreach (Task<bool> updateTask in updateTasks)
                {
                    completed = completed && await updateTask;//completed is only true if every update task was successful
                }

                return completed;
            }
            else
            {
                throw new NotImplementedException("within FAFolderRepository the method DeleteReferences<U>(int) does not have an else if() for the type " + typeof(U).Name + " which is the generic type that was passed to it.");
            }
        }

        protected async override Task<bool> EntityMeetsCreateRequirements(FAFolder entity)
        {
            if (entity != null&&
                await Contains(f => f.RelativePath.Equals(entity.RelativePath)) == false && //if the relative path already exists in the repository it can't be added again as the relative path must be unique
                (entity.ParentId == null || (entity.Id != entity.ParentId && await Contains((int)entity.ParentId) == true))&&//if the ParentId isn't set to null then the ParentId must not be the same as the Id (it can't be it's own parent) and must match an Id that exists within the repository.
                (entity.NumberId == null || await repositoryFactory.DpsNumberRepository.Contains((int)entity.NumberId)) &&//if the numberId isn't set to null then the NumberId needs to exist within the DpsNumberRepository
                (entity.FolderId == null || await repositoryFactory.DpsFolderRepository.Contains((int)entity.FolderId)) &&//if the folderId isn't set to null then the folderId needs to exist within the DpsFolderRepository
                (entity.SubfolderId == null || await repositoryFactory.DpsSubfolderRepository.Contains((int)entity.SubfolderId))//if the subfolderId isn't set to null then the subfolder Id needs to exist within the DpsSubfolderRepository
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override async Task<bool> EntityMeetsUpdateRequirements(FAFolder entity)
        {
            if (entity != null&&
                await Contains(entity.Id) &&//the id must exist within the repository
                await Contains(f => f.RelativePath == entity.RelativePath && f.Id != entity.Id) == true &&//no entity exists that has the same relative path EXCEPT if it also has the same Id   aka it may only share a relative path with itself
                (entity.ParentId == null || (entity.Id != entity.ParentId && await Contains((int)entity.ParentId) == true))&&//if the ParentId isn't set to null then the ParentId must not be the same as the Id (it can't be it's own parent) and must match an Id that exists within the repository.
                (entity.NumberId == null || await repositoryFactory.DpsNumberRepository.Contains((int)entity.NumberId))&&//if the numberId isn't set to null then the NumberId needs to exist within the DpsNumberRepository
                (entity.FolderId == null || await repositoryFactory.DpsFolderRepository.Contains((int)entity.FolderId))&&//if the folderId isn't set to null then the folderId needs to exist within the DpsFolderRepository
                (entity.SubfolderId == null || await repositoryFactory.DpsSubfolderRepository.Contains((int)entity.SubfolderId))//if the subfolderId isn't set to null then the subfolder Id needs to exist within the DpsSubfolderRepository
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override FAFolder ReadEntity(string[] lineSplit)
        {
            if (int.TryParse(lineSplit[id], out int folderId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            int? parentIdNullable = null;
            if (int.TryParse(lineSplit[folderParentId], out int parentId))
            {
                parentIdNullable = parentId;
                //throw new FormatException("the FA folder Parentid could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            if (bool.TryParse(lineSplit[folderHasItems], out bool hasItems) == false)
            {
                throw new FormatException("the FA folder HasItems could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            int? numberIdNullable = null;
            if (int.TryParse(lineSplit[folderNumberId], out int numberId))
            {
                numberIdNullable = numberId;
                //throw new FormatException("the FA folder NumberId could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            int? dpsFolderIdNullable = null;
            if (int.TryParse(lineSplit[folderFolderId], out int dpsFolderId))
            {
                dpsFolderIdNullable = dpsFolderId;
                //throw new FormatException("the FA folder FolderId could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            int? subfolderIdNullable = null;
            if (int.TryParse(lineSplit[folderSubfolderId], out int subfolderId))
            {
                subfolderIdNullable = subfolderId;
                //throw new FormatException("the FA folder SubfolderId could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            if (bool.TryParse(lineSplit[folderIsFloor], out bool isFloor) == false)
            {
                throw new FormatException("the FA folder IsFloor could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            if (bool.TryParse(lineSplit[folderOtherCommands], out bool otherCommands) == false)
            {
                throw new FormatException("the FA folder OtherCommands could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }

            return new FAFolder()
            {
                Id = folderId,
                Name = lineSplit[folderName],
                RelativePath = lineSplit[folderRelativePath],
                ParentId = parentIdNullable,
                HasItems = hasItems,
                NumberId = numberIdNullable,
                FolderId = dpsFolderIdNullable,
                SubfolderId = subfolderIdNullable,
                IsFloor = isFloor,
                OtherCommands = otherCommands,
                Commands = lineSplit[folderCommands],
                Notes = lineSplit[folderNotes]
            };
        }

    }
}