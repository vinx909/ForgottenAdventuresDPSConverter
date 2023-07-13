using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class DpsSubfolderFileRepository : FileRepository<DpsSubfolder>
    {
        #region id of elements within a split line
        private const int subfolderName = 1;
        private const int subfolderDescription = 2;
        #endregion

        public DpsSubfolderFileRepository(IFileRepositorySettings settings) : base(settings.DpsSubfolderRepositoryFilePath, settings) { }

        protected override string CreateEntityLine(DpsSubfolder subfolder)
        {
            Dictionary<int, string> stringParts = new Dictionary<int, string>();
            stringParts.Add(id, subfolder.Id.ToString());
            stringParts.Add(subfolderName, subfolder.Name);
            stringParts.Add(subfolderDescription, subfolder.Description);

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
                    throw new Exception("there is a gap in the Ids for creating a folder in the CreateFolderLine method ForgottenAdventuresDPSConverter.FileRepository.FolderRepository which means that either some property is missing within the method or that the folder identifiers are missing a number which will create problems in reading too");
                }
            }

            return returnString;
        }

        internal async override Task<bool> DeleteReferences<U>(int id)
        {
            if (typeof(U) == typeof(DpsSubfolder))
            {
                /*
                 * if a type of DpsSubfolder gets deleted all FAFolder items that reference that folder need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                return await repositoryFactory.FAFolderRepository.DeleteReferences<DpsSubfolder>(id);//FAFolderRepository handles the removing of all references
            }
            else
            {
                throw new NotImplementedException("within DpsSubfolderRepository the method DeleteReferences<U>(int) does not have an else if() for the type " + typeof(U).Name + " which is the generic type that was passed to it.");
            }
        }

        protected override Task<bool> EntityMeetsCreateRequirements(DpsSubfolder entity)
        {
            if(entity != null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        protected async override Task<bool> EntityMeetsUpdateRequirements(DpsSubfolder entity)
        {
            if (entity != null&&
                await Contains(entity.Id) == true //an entity must exist with the same Id
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override DpsSubfolder ReadEntity(string[] lineSplit)
        {
            if (int.TryParse(lineSplit[id], out int subfolderId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            
            return new DpsSubfolder()
            {
                Id = subfolderId,
                Name = lineSplit[subfolderName],
                Description = lineSplit[subfolderDescription],
            };
        }
    }
}