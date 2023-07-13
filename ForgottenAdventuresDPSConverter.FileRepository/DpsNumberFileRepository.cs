using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class DpsNumberFileRepository : FileRepository<DpsNumber>
    {
        #region id of elements within a split line
        private const int numberNumber = 1;
        private const int numberName = 2;
        private const int numberDescription = 3;
        #endregion

        public DpsNumberFileRepository(IFileRepositorySettings settings) : base(settings.DpsNumberRepositoryFilePath, settings) { }

        protected override string CreateEntityLine(DpsNumber number)
        {
            Dictionary<int, string> stringParts = new Dictionary<int, string>();
            stringParts.Add(id, number.Id.ToString());
            stringParts.Add(numberNumber, number.Number.ToString());
            stringParts.Add(numberName, number.Name);
            stringParts.Add(numberDescription, number.Description);

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
            if (typeof(U) == typeof(DpsNumber))
            {
                /*
                 * if a type of DpsNumberfolder gets deleted all FAFolder items that reference that number need to have their reference deleted
                 * aka
                 * on delete set null
                 */

                return await repositoryFactory.FAFolderRepository.DeleteReferences<DpsNumber>(id);//FAFolderRepository handles the removing of all references
            }
            else
            {
                throw new NotImplementedException("within DpsNumberRepository the method DeleteReferences<U>(int) does not have an else if() for the type " + typeof(U).Name + " which is the generic type that was passed to it.");
            }
        }

        protected async override Task<bool> EntityMeetsCreateRequirements(DpsNumber entity)
        {
            if (entity != null &&
                await Contains(n => n.Number == entity.Number) == false //the number does not already exist within the repository
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected async override Task<bool> EntityMeetsUpdateRequirements(DpsNumber entity)
        {
            if (entity != null &&
                await Contains(entity.Id) == true && //an enitity with the same Id exists
                await Contains(n => n.Number == entity.Number && n.Id != entity.Id) == false //the number does not already exist within the repository unless it's in the entity with the same Id
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override DpsNumber ReadEntity(string[] lineSplit)
        {
            if (int.TryParse(lineSplit[id], out int numberId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            if (int.TryParse(lineSplit[numberNumber], out int number) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }

            return new DpsNumber()
            {
                Id = numberId,
                Number = number,
                Name = lineSplit[numberName],
                Description = lineSplit[numberDescription]
            };
        }
    }
}