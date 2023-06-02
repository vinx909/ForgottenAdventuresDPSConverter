using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class DpsFolderRepository : Repository<DpsFolder>
    {
        #region id of elements within a split line
        private const int folderName = 1;
        private const int folderNameAbriviation = 2;
        private const int folderDescription = 3;
        #endregion

        public DpsFolderRepository(IFileRepositorySettings settings) : base(settings.DpsFolderRepositoryFilePath, settings) { }

        protected override string CreateEntityLine(DpsFolder folder)
        {
            Dictionary<int, string> stringParts = new Dictionary<int, string>();
            stringParts.Add(id, folder.Id.ToString());
            stringParts.Add(folderName, folder.Name);
            stringParts.Add(folderNameAbriviation, folder.NameAbriviation);
            stringParts.Add(folderDescription, folder.Description);

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
            if (typeof(U) == typeof(DpsFolder))
            {
                /*
                 * if a type of DpsFolder gets deleted all FAFolder items that reference that folder need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                return await repositoryFactory.FAFolderRepository.DeleteReferences<DpsFolder>(id);//FAFolderRepository handles the removing of all references
            }
            else
            {
                throw new NotImplementedException("within DpsFolderRepository the method DeleteReferences<U>(int) does not have an else if() for the type " + typeof(U).Name + " which is the generic type that was passed to it.");
            }
        }

        protected async override Task<bool> EntityMeetsCreateRequirements(DpsFolder entity)
        {
            if (entity != null &&
                entity.NameAbriviation.Length <= DpsFolder.NameAbriviationMaxLength && //the name abriviation must be lower or equal to the max length
                await Contains(f => f.NameAbriviation == entity.NameAbriviation) == false //the name abriviation must not exist already
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected async override Task<bool> EntityMeetsUpdateRequirements(DpsFolder entity)
        {
            if (entity != null &&
                entity.NameAbriviation.Length <= DpsFolder.NameAbriviationMaxLength && //the name abriviation must be lower or equal to the max length
                await Contains(entity.Id) == true && //an entity with the same Id must exist
                await Contains(f => f.NameAbriviation == entity.NameAbriviation && f.Id != entity.Id) == false //the name abriviation must not exist already unless it's the entity with the same id
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override DpsFolder ReadEntity(string[] lineSplit)
        {
            if (int.TryParse(lineSplit[id], out int folderId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }

            return new DpsFolder()
            {
                Id = folderId,
                Name = lineSplit[folderName],
                NameAbriviation = lineSplit[folderNameAbriviation],
                Description = lineSplit[folderDescription],
            };
        }
    }
}