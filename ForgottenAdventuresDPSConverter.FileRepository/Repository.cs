using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public abstract class Repository<T> : IRepository<T> where T : IdEntity
    {
        //temp file creation designed with help from https://www.daveoncsharp.com/2009/09/how-to-use-temporary-files-in-csharp/

        protected readonly string filePath;
        protected readonly IFileRepositorySettings repositoryFactory;
        protected readonly static Dictionary<string, Semaphore> semaphorePerFile = new();

        protected const char itemBreak = (char)31;

        #region first line content
        private const int firstLineIdentifier = -1;
        private const int firstId = 1;
        private const string firstLineExplainTextString = "the first line of the file will serve the purpose of keeping track of some file specific things such as the next id. the rest of the file will contain one line per folder item. this file should practically never be touched by hand as that will near quarentied create problems. this line also functions as a check to make sure the file is a file to be edited by the program and should thus not be touched.";
        #endregion

        #region id of elements within a split line
        protected const int id = 0;

        private const int firstLineNextId = 1;
        private const int firstLineExplainText = 2;
        #endregion

        public Repository(string filePath, IFileRepositorySettings settings)
        {
            this.filePath = filePath;
            this.repositoryFactory = settings;
            if (semaphorePerFile.ContainsKey(filePath) == false)
            {
                semaphorePerFile.Add(filePath, new(1, 1));
            }

            if (File.Exists(filePath) == false)
            {
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine(CreateFirstLine(firstId));
                }
            }
            else
            {
                bool correctFile = false;

                using (StreamReader reader = File.OpenText(filePath))
                {
                    string? line = reader.ReadLine();
                    while (line != null)
                    {
                        try
                        {
                            if (ReadId(line) == firstLineIdentifier && line.Contains(firstLineExplainTextString))
                            {
                                correctFile = true;
                                break;
                            }
                        }
                        catch (FormatException e)
                        {
                            correctFile = false;
                            break;
                        }

                        line = reader.ReadLine();
                    }
                }

                if (correctFile == false)
                {
                    throw new InvalidDataException("the given path leads to a file that doesn't meet the requirements and should thus not be touched to avoid damaging files");
                }
            }
        }

        public Task<bool> Contains(System.Linq.Expressions.Expression<Func<T, bool>> query)
        {
            Func<T, bool> func = query.Compile();

            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))//read the file
                {
                    string? line = reader.ReadLine();//read the first line

                    while (line != null)//while the file has lines
                    {
                        if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                        {
                            if (func(ReadEntity(lineSplit)))//parse the line into an FAFolder and test is against the query
                            {
                                return Task.FromResult(true);//if the query tests true return true
                            }
                        }
                        line = reader.ReadLine();//get the next line
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }
            return Task.FromResult(false);//if no items tests true return false
        }

        internal Task<bool> Contains(int id)
        {
            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))//read the file
                {
                    string? line = reader.ReadLine();//read the first line

                    while (line != null)//while the file has lines
                    {
                        if (ReadId(line, out string[] lineSplit) == id)//if the id matches the looked for Id
                        {
                            return Task.FromResult(true);//return true
                        }
                        line = reader.ReadLine();//get the next line
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }
            return  Task.FromResult(false);//if no items tests true return false
        }

        public async Task<int> Count(System.Linq.Expressions.Expression<Func<T, bool>> query)
        {
            Func<T, bool> func = query.Compile();
            int count = 0;

            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))//read the file
                {
                    string? line = reader.ReadLine();//read the first line

                    while (line != null)//while the file has lines
                    {
                        if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                        {
                            if (func(ReadEntity(lineSplit)))//parse the line into an FAFolder and test is against the query
                            {
                                count++;//if the query tests true increase count by 1
                            }
                        }
                        line = reader.ReadLine();//get the next line
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }
            return count;//return the count
        }

        public async Task<(bool, int)> Create(T toCreate)
        {
            if (await EntityMeetsCreateRequirements(toCreate) == false)
            {
                return (false, 0);
            }
            
            toCreate.Id = await GetNextId();

            StreamWriter writer = null;
            semaphorePerFile[filePath].WaitOne();
            try
            {
                writer = File.AppendText(filePath);
                writer.WriteLine(CreateEntityLine(toCreate));
            }
            finally
            {
                writer?.Close();
                semaphorePerFile[filePath].Release();
            }

            return (true, toCreate.Id);
        }
        
        public async Task<bool> Delete(int id)
        {
            if(await DeleteReferences<T>(id) == true)
            {
                string tempFileName = string.Empty;

                semaphorePerFile[filePath].WaitOne();
                try
                {
                    tempFileName = GetTempFile();

                    #region make a copy with the update
                    using (StreamWriter writer = File.AppendText(tempFileName))//able to write to temp file
                    {
                        using (StreamReader reader = new(filePath))//able to read file
                        {
                            string? line = reader.ReadLine();//try to get the first line

                            while (line != null)//so long as not at the end of the file
                            {
                                if (ReadId(line) == id)//if the id is the one that needs to be removed
                                {
                                    line = reader.ReadLine();//we write nothing and read the next line
                                }
                                else//otherwise
                                {
                                    writer.WriteLine(line);//copy each line to the temp file
                                    line = reader.ReadLine();//tries to get the next line
                                }
                            }
                        }
                    }
                    #endregion

                    #region replace the origional file with the temp file with the update
                    using (StreamWriter writer = File.CreateText(filePath))//able to write to file
                    {
                        using (StreamReader reader = new(tempFileName))//able to read temp file
                        {
                            string? line = reader.ReadLine();//read the first line

                            while (line != null)//so long as not at the end of the file
                            {
                                writer.WriteLine(line);//replace it with the correct line
                                line = reader.ReadLine();//and get the next line
                            }
                        }
                    }
                    #endregion

                    return true;
                }
                finally
                {
                    if (File.Exists(tempFileName))
                    {
                        File.Delete(tempFileName);
                    }
                    semaphorePerFile[filePath].Release();
                }
            }
            else
            {
                return false;
            }
        }

        public Task<T> Get(int id)
        {
            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))//read the file
                {
                    string? line = reader.ReadLine();//read the first line

                    while (line != null)//while the file has lines
                    {
                        if (ReadId(line, out string[] lineSplit) == id)//if the line has the correct id
                        {
                            return Task.FromResult(ReadEntity(lineSplit));//parse the line into an entity of type T and return it
                        }
                        line = reader.ReadLine();//get the next line
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }
            return Task.FromResult((T)null);
        }

        public Task<IEnumerable<T>> GetAll()
        {
            List<T> entities = new();

            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    string[] lineSplit = default;
                    string? line = reader.ReadLine();
                    while (line != null)
                    {
                        if (ReadId(line, out lineSplit) != firstLineIdentifier)
                        {
                            entities.Add(ReadEntity(lineSplit));
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }

            IEnumerable<T> enumerableEntities = entities;
            return Task.FromResult(enumerableEntities);
        }

        public Task<IEnumerable<T>> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> query)
        {
            Func<T, bool> func = query.Compile();
            List<T> entities = new();

            semaphorePerFile[filePath].WaitOne();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))//read the file
                {
                    string? line = reader.ReadLine();//read the first line

                    while (line != null)//while the file has lines
                    {
                        if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                        {
                            T entity = ReadEntity(lineSplit);//parse the line into an FAFolder
                            if (func(entity))//test the folders against the query
                            {
                                entities.Add(entity);//if the query tests true add the folder to the folders to return
                            }
                        }
                        line = reader.ReadLine();//get the next line
                    }
                }
            }
            finally
            {
                semaphorePerFile[filePath].Release();
            }

            IEnumerable<T> enumerableEntities = entities;
            return Task.FromResult(enumerableEntities);
        }

        public async Task<bool> Update(T toUpdate)
        {
            if(await EntityMeetsUpdateRequirements(toUpdate))

            semaphorePerFile[filePath].WaitOne();
            string tempFileName = string.Empty;

            try
            {
                tempFileName = GetTempFile();

                bool found = false;//boolean to try and find the file

                #region make a copy with the update
                using (StreamWriter writer = File.AppendText(tempFileName))//able to write to temp file
                {
                    using (StreamReader reader = new(filePath))//able to read file
                    {

                        string? line = reader.ReadLine();//try to get the first line

                        while (line != null)//so long as not at the end of the file
                        {
                            if (ReadId(line) == toUpdate.Id)//if the line if the same line that needs to be replaced
                            {
                                writer.WriteLine(CreateEntityLine(toUpdate));//replace it with the correct line
                                line = reader.ReadLine();//and get the next line
                                if (found == false)
                                {
                                    found = true;//set found to true
                                }
                                else
                                {
                                    throw new InvalidDataException("two items were found with the same Id. this should never be possible and shows bad data");
                                }
                            }
                            else
                            {
                                writer.WriteLine(line);//copy each line to the temp file
                                line = reader.ReadLine();//tries to get the next line
                            }
                        }
                    }
                }
                #endregion

                #region replace the origional file with the temp file with the update
                if (found == true) //no need to replace the file if no changes were made
                {
                    using (StreamWriter writer = File.CreateText(filePath))//able to write to file
                    {
                        using (StreamReader reader = new(tempFileName))//able to read temp file
                        {
                            string? line = reader.ReadLine();//read the first line

                            while (line != null)//so long as not at the end of the file
                            {
                                writer.WriteLine(line);//replace it with the correct line
                                line = reader.ReadLine();//and get the next line
                            }
                        }
                    }
                }
                #endregion

                return found;
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
                semaphorePerFile[filePath].Release();
            }
        }


        /// <summary>
        /// create a string representation of an entity of type T to be saved in the repository
        /// </summary>
        /// <param name="entity">the T entity to be turned into a string</param>
        /// <returns>a string representation of the entity T</returns>
        protected abstract string CreateEntityLine(T entity);

        /// <summary>
        /// deletes all references to the entity of type U with the id if it's allowed to be deleted. this method can both delete foreign keys and self referential foreign keys
        /// </summary>
        /// <typeparam name="U">the type of entity that any reference to needs to be deleted</typeparam>
        /// <param name="id">the id of the entity to which references need to be deleted</param>
        /// <returns>returns a task with a boolean that's false if a foreign key is not allowed to be deleted and thus the origional is not allowed to be deleted, or if it failed to delete a foreing key and thus the entity is not allowed to be deleted. returns true if all foreign keys to the entity are deleted and the entity can thus be savely deleted</returns>
        internal abstract Task<bool> DeleteReferences<U>(int id);

        /// <summary>
        /// tests if an entity is allowed to be added to the repository, to be used in methods like Create
        /// </summary>
        /// <param name="entity">the entity that will be tested</param>
        /// <returns>a task containing a boolean that's true if the entity is allowed to be added and false if it's not</returns>
        protected abstract Task<bool> EntityMeetsCreateRequirements(T entity);

        /// <summary>
        /// tests if an entity is allowed to be added to the repository, to be used in methods like Update
        /// </summary>
        /// <param name="entity">the entity that will be tested</param>
        /// <returns>a task containing a boolean that's true if the entity is allowed to be updated with the new input and false if it's not</returns>
        protected abstract Task<bool> EntityMeetsUpdateRequirements(T entity);

        /// <summary>
        /// creates an entity of type T from a split string with all properties parsed
        /// </summary>
        /// <param name="lineSplit">the split string from which the entity will be parsed</param>
        /// <returns>an opject of type T</returns>
        protected abstract T ReadEntity(string[] lineSplit);


        /// <summary>
        /// creates the string for the first line
        /// </summary>
        /// <param name="nextId">the next id that should be used</param>
        /// <returns>the string for the first line</returns>
        /// <exception cref="Exception">will throw an exception if the identifying variables are not the same as the order they are set in here. can not pop up in runtime and means that constant values are different and would require an update to the CreatFirstLine method</exception>
        private static string CreateFirstLine(int nextId)
        {
            if ((id == 0 && firstLineNextId == 1 && firstLineExplainText == 2) == false)
            {
                throw new Exception("the CreateFirstLine method in ForgottenAdventuresDPSConverter.FileRepository.FolderRepository no longer matches with the first line Ids and would not make the correct first line");
            }

            return string.Empty + firstLineIdentifier + itemBreak + nextId + itemBreak + firstLineExplainTextString;
        }

        /// <summary>
        /// reads the id of a line
        /// </summary>
        /// <param name="line">the string the id will be read out of</param>
        /// <returns>the id of the line</returns>
        /// <exception cref="FormatException">will throw an exception if the id could not be parsed. should never happen</exception>
        private static int ReadId(string line)
        {
            string[] lineSplit = line.Split(itemBreak);
            if (int.TryParse(lineSplit[id], out int lineId) == false)
            {
                throw new FormatException("the id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or id was changed compared to the repository");
            }
            return lineId;
        }

        /// <summary>
        /// reads the id of a line, and splits the string on itembreaks as additional output
        /// </summary>
        /// <param name="line">the string the id will be read out of and that will be split</param>
        /// <param name="lineSplit">the string split on itemBreaks</param>
        /// <returns>the id of the line</returns>
        /// <exception cref="FormatException">will throw an exception if the id could not be parsed. should never happen</exception>
        private static int ReadId(string line, out string[] lineSplit)
        {
            lineSplit = line.Split(itemBreak);
            if (int.TryParse(lineSplit[id], out int lineId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            return lineId;
        }

        /// <summary>
        /// gets the next id for an item to be created
        /// </summary>
        /// <returns>int with the next id to be used</returns>
        private async Task<int> GetNextId()
        {
            string tempFileName = string.Empty;

            semaphorePerFile[filePath].WaitOne();
            try
            {
                int nextId;

                tempFileName = GetTempFile();

                using (StreamWriter writer = File.AppendText(tempFileName))//able to write to temp file
                {
                    using (StreamReader reader = new(filePath))//able to read file
                    {
                        string? line = reader.ReadLine();//try to get the first line

                        while (line != null)//so long as not at the end of the file
                        {
                            writer.WriteLine(line);//copy each line to the temp file
                            line = reader.ReadLine();//tries to get the next line
                        }
                    }
                }

                int foundOnLine = -1;
                using (StreamReader reader = new(tempFileName)) //able to read temp file
                {
                    string[] lineSplit = null; //the lines read split into it's component parts
                    int id = firstLineIdentifier - 1;//the id of the line
                    string? line = reader.ReadLine();
                    while (line != null && id != firstLineIdentifier) //so long as the reader doesn't read nothing    OR   lineSplit isn't empty or the first line id position is not equal to the first line id
                    {
                        id = ReadId(line, out lineSplit);
                        foundOnLine++;
                        line = reader.ReadLine();
                    }

                    if (lineSplit == null)
                    {
                        throw new Exception("no first line was found, this should never be possible");
                    }
                    if (int.TryParse(lineSplit[firstLineNextId], out nextId) == false)
                    {
                        throw new Exception("could not parse the next id location. this should never be possible");
                    }
                }

                using (StreamWriter writer = File.CreateText(filePath))//able to write to file
                {
                    using (StreamReader reader = new(tempFileName))//able to read temp file
                    {
                        int currentLine = 0;
                        while (!reader.EndOfStream)//so long as not at the end of the file
                        {
                            if (currentLine == foundOnLine)//if the line if the same line that needs to be replaced
                            {
                                writer.WriteLine(CreateFirstLine(nextId + 1));//replace it with the correct line
                                reader.ReadLine();//and jump the reader to the next line
                            }
                            else
                            {
                                writer.WriteLine(reader.ReadLine());//otherwise copy the line from the temp file to the main file
                            }
                            currentLine++;
                        }
                    }
                }

                return nextId;
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
                semaphorePerFile[filePath].Release();
            }
        }

        /// <summary>
        /// gets the path to a set up temp file.
        /// </summary>
        /// <returns>a string with the path to the temp file</returns>
        private string GetTempFile()
        {
            string tempFileName = Path.GetTempFileName(); //get the temp file name
            FileInfo tempFileInfo = new FileInfo(tempFileName); //get the attrubutes so they can be set
            tempFileInfo.Attributes = FileAttributes.Temporary; //set them to temporary
            return tempFileName;
        }


        public enum DeleteBehaviour
        {
            cascade, //this means that any enity that reference the deleted entity will also be deleted
            setNull, //this means that any reference to the entity will be set to null
            noAction //this means that the entity will only be deleted if there are no objects that reference it
        }
    }
}
