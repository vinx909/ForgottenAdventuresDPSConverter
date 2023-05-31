using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class FAFolderRepository : Repository<FAFolder>
    {
        //temp file creation designed with help from https://www.daveoncsharp.com/2009/09/how-to-use-temporary-files-in-csharp/

        private readonly string filePath;

        /*
         * the first line of the file will serve the purpose of keeping track of some file specific things such as the next id. the rest of the file will contain one line per folder
         */

        #region first line content
        private const int firstLineIdentifier = -1;
        private const int firstId = 1;
        private const string firstLineExplainTextString = "the first line of the file will serve the purpose of keeping track of some file specific things such as the next id. the rest of the file will contain one line per folder item. this file should practically never be touched by hand as that will near quarentied create problems. this line also functions as a check to make sure the file is a file to be edited by the program and should thus not be touched.";
        #endregion

        #region id of elements within a split line
        private const int id = 0;

        private const int firstLineNextId = 1;
        private const int firstLineExplainText = 2;

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

        private const string linebreak = "\r\n";
        private const char itemBreak = (char)31;

        public FAFolderRepository(IFileRepositorySettings settings)
        {
            filePath = settings.FAFolderRepositoryFilePath;

            if (!File.Exists(filePath))
            {
                using(StreamWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine(CreateFirstLine(firstId));
                }
            }
            else
            {
                bool correctFile = false;

                using(StreamReader reader = File.OpenText(filePath))
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
                        catch(FormatException e)
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

        public override async Task<bool> Contains(Expression<Func<FAFolder, bool>> query)
        {
            Func<FAFolder, bool> func = query.Compile();

            using (StreamReader reader = File.OpenText(filePath))//read the file
            {
                string? line = reader.ReadLine();//read the first line

                while (line != null)//while the file has lines
                {
                    if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                    {
                        if (func(ReadFolder(lineSplit)))//parse the line into an FAFolder and test is against the query
                        {
                            return true;//if the query tests true return true
                        }
                    }
                    line = reader.ReadLine();//get the next line
                }
            }
            return false;//if no items tests true return false
        }
        public async Task<bool> Contains(int id)
        {
            using (StreamReader reader = File.OpenText(filePath))//read the file
            {
                string? line = reader.ReadLine();//read the first line

                while (line != null)//while the file has lines
                {
                    if (ReadId(line, out string[] lineSplit) == id)//if the id matches
                    {
                        return true;//return true
                    }
                    line = reader.ReadLine();//get the next line
                }
            }
            return false;//if no items tests true return false
        }

        public override async Task<int> Count(Expression<Func<FAFolder, bool>> query)
        {
            Func<FAFolder, bool> func = query.Compile();
            int count = 0;

            using (StreamReader reader = File.OpenText(filePath))//read the file
            {
                string? line = reader.ReadLine();//read the first line

                while (line != null)//while the file has lines
                {
                    if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                    {
                        if (func(ReadFolder(lineSplit)))//parse the line into an FAFolder and test is against the query
                        {
                            count++;//if the query tests true increase count by 1
                        }
                    }
                    line = reader.ReadLine();//get the next line
                }
            }
            return count;//return the count
        }

        public override async Task<(bool, int)> Create(FAFolder toCreate)
        {
            if(await Contains(f => f.RelativePath.Equals(toCreate.RelativePath)) == true)
            {
                return (false, 0);
            }

            if(toCreate.ParentId != null && await Contains(f => f.Id == toCreate.ParentId) == false)
            {
                return (false, 0);
            }
            toCreate.Id = await GetNextId();

            int writerQueueNumber = await AwaitWritingQueueNumber(filePath);
            StreamWriter writer = null;
            try
            {
                writer = File.AppendText(filePath);
                writer.WriteLine(CreateFolderLine(toCreate));
            }
            finally
            {
                writer?.Close();
                releaseWritingQueueNumber(filePath, writerQueueNumber);
            }

            return (true, toCreate.Id);
        }

        /// <summary>
        /// this delete function will delete the FAFolder AND ANY folder that has it as a parent or has any of it's childeren as a parent. this can thus remove a lot of folders if a folder with a lot of children gets deleted
        /// </summary>
        public override async Task<bool> Delete(int id)
        {
            if (await Contains(id) == true)
            {
                List<int> idsToDelete = await getChilderen(id);
                idsToDelete.Add(id);
                //now that all the ids of the items that need to be deleted have been gathered we can move on to making a copy of the file without any of the folders with those ids and then replacing the file with that updated copy

                string tempFileName = string.Empty;
                int writerQueueNumber = await AwaitWritingQueueNumber(filePath);

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
                                if (idsToDelete.Contains(ReadId(line)))//if the id is one of the ones that needs to be removed
                                {
                                    line = reader.ReadLine();//we write nothing and read the next line
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
                    releaseWritingQueueNumber(filePath, writerQueueNumber);
                }
            }
            else
            {
                return false;
            }

            async Task<List<int>> getChilderen(int id)
            {
                List<int> childerenIds = new();
                foreach(FAFolder child in await GetAll(f => f.ParentId == id))
                {
                    childerenIds.Add(child.Id);
                    childerenIds.AddRange(await getChilderen(child.Id));
                }
                return childerenIds;
            } 
        }

        public override async Task<FAFolder> Get(int id)
        {
            using (StreamReader reader = File.OpenText(filePath))//read the file
            {
                string? line = reader.ReadLine();//read the first line

                while (line != null)//while the file has lines
                {
                    if (ReadId(line, out string[] lineSplit) == id)//if the line has the correct id
                    {
                        return ReadFolder(lineSplit);//parse the line into an FAFolder and return it
                    }
                    line = reader.ReadLine();//get the next line
                }
            }
            return null;
        }

        public override async Task<IEnumerable<FAFolder>> GetAll()
        {
            List<FAFolder> folders = new List<FAFolder>();

            using(StreamReader reader = File.OpenText(filePath))
            {
                string[] lineSplit = default;
                string? line = reader.ReadLine();
                while (line != null)
                {
                    if (ReadId(line, out lineSplit) != firstLineIdentifier)
                    {
                        folders.Add(ReadFolder(lineSplit));
                    }
                    line = reader.ReadLine();
                }
            }

            IEnumerable<FAFolder> enumerableFolders = folders;
            return enumerableFolders;
        }

        public override async Task<IEnumerable<FAFolder>> GetAll(Expression<Func<FAFolder, bool>> query)
        {
            Func<FAFolder, bool> func = query.Compile();
            List<FAFolder> folders = new();

            using (StreamReader reader = File.OpenText(filePath))//read the file
            {
                string? line = reader.ReadLine();//read the first line

                while (line != null)//while the file has lines
                {
                    if (ReadId(line, out string[] lineSplit) != firstLineIdentifier)//if the line isn't the first line
                    {
                        FAFolder folder = ReadFolder(lineSplit);//parse the line into an FAFolder
                        if (func(folder))//test the folders against the query
                        {
                            folders.Add(folder);//if the query tests true add the folder to the folders to return
                        }
                    }
                    line = reader.ReadLine();//get the next line
                }
            }
            IEnumerable<FAFolder> enumerableFolders = folders;
            return enumerableFolders;
        }

        public override async Task<bool> Update(FAFolder toUpdate)
        {
            string tempFileName = string.Empty;
            int writerQueueNumber = await AwaitWritingQueueNumber(filePath);

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
                                writer.WriteLine(CreateFolderLine(toUpdate));//replace it with the correct line
                                line = reader.ReadLine();//and get the next line
                                if(found == false)
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
                releaseWritingQueueNumber(filePath, writerQueueNumber);
            }
        }

        /// <summary>
        /// creates the string for the first line
        /// </summary>
        /// <param name="nextId">the next id that should be used</param>
        /// <returns>the string for the first line</returns>
        /// <exception cref="Exception">will throw an exception if the identifying variables are not the same as the order they are set in here. can not pop up in runtime and means that constant values are different and would require an update to the CreatFirstLine method</exception>
        private static string CreateFirstLine(int nextId)
        {
            if((id == 0 && firstLineNextId == 1 && firstLineExplainText == 2) == false)
            {
                throw new Exception("the CreateFirstLine method in ForgottenAdventuresDPSConverter.FileRepository.FolderRepository no longer matches with the first line Ids and would not make the correct first line");
            }

            return string.Empty+firstLineIdentifier + itemBreak + nextId + itemBreak + firstLineExplainTextString;
        }

        /// <summary>
        /// create a string representation of an FAFolder to be saved in the repository
        /// </summary>
        /// <param name="folder">the folder to be turned into a string</param>
        /// <returns>a string representation of the folder</returns>
        /// <exception cref="Exception">throws an exception if there's a gap in the seperation of properties of the folder. should never come up in runtime and indicates the method needs reworking</exception>
        private static string CreateFolderLine(FAFolder folder)
        {
            Dictionary<int, string> stringParts = new Dictionary<int, string>();
            stringParts.Add(id, folder.Id.ToString());
            stringParts.Add(folderName, folder.Name);
            stringParts.Add(folderRelativePath, folder.RelativePath);
            if(folder.ParentId == null)
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
                    if(returnString != string.Empty)
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

        /// <summary>
        /// creates a FAFolder object from a string with all properties parsed
        /// </summary>
        /// <param name="folderLine">the string from which the FAFolder will be parsed</param>
        /// <returns>an FAFolder opjects</returns>
        /// <exception cref="FormatException">will throw an exception if the data can not be parsed, should never happen unless wrong data is loaded into the repository</exception>
        private static FAFolder ReadFolder(string folderLine)
        {
            return ReadFolder(folderLine.Split(itemBreak));
        }

        /// <summary>
        /// creates a FAFolder object from a split string with all properties parsed
        /// </summary>
        /// <param name="lineSplit">the split string from which the FAFolder will be parsed</param>
        /// <returns>an FAFolder opjects</returns>
        /// <exception cref="FormatException">will throw an exception if the data can not be parsed, should never happen unless wrong data is loaded into the repository</exception>
        private static FAFolder ReadFolder(string[] lineSplit)
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

            FAFolder folder = new()
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

            return folder;
        }

        /// <summary>
        /// reads the id of a line
        /// </summary>
        /// <param name="folderLine">the string the id will be read out of</param>
        /// <returns>the id of the line</returns>
        /// <exception cref="FormatException">will throw an exception if the id could not be parsed. should never happen</exception>
        private static int ReadId(string folderLine)
        {
            string[] lineSplit = folderLine.Split(itemBreak);
            if (int.TryParse(lineSplit[id], out int folderId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            return folderId;
        }

        /// <summary>
        /// reads the id of a line, and splits the string on itembreaks as additional output
        /// </summary>
        /// <param name="folderLine">the string the id will be read out of and that will be split</param>
        /// <param name="lineSplit">the string split on itemBreaks</param>
        /// <returns>the id of the line</returns>
        /// <exception cref="FormatException">will throw an exception if the id could not be parsed. should never happen</exception>
        private static int ReadId(string folderLine, out string[] lineSplit)
        {
            lineSplit = folderLine.Split(itemBreak);
            if (int.TryParse(lineSplit[id], out int folderId) == false)
            {
                throw new FormatException("the FA folder id could not be parsed, this means either bad data was put into the repository, someone changed data by hand (which they should never do), or folderId was changed compared to the repository");
            }
            return folderId;
        }

        /// <summary>
        /// gets the next id for an item to be created
        /// </summary>
        /// <returns>int with the next id to be used</returns>
        private async Task<int> GetNextId()
        {
            string tempFileName = string.Empty;
            int writerQueueNumber = await AwaitWritingQueueNumber(filePath);

            try
            {
                int nextId;

                tempFileName = GetTempFile();

                using (StreamWriter writer = File.AppendText(tempFileName))//able to write to temp file
                {
                    using(StreamReader reader = new(filePath))//able to read file
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
                    int id= firstLineIdentifier-1;//the id of the line
                    string? line = reader.ReadLine();
                    while (line!=null && id!= firstLineIdentifier) //so long as the reader doesn't read nothing    OR   lineSplit isn't empty or the first line id position is not equal to the first line id
                    {
                        id = ReadId(line, out lineSplit);
                        foundOnLine++;
                        line = reader.ReadLine();
                    }

                    if (lineSplit == null)
                    {
                        throw new Exception("no first line was found, this should never be possible");
                    }
                    if(int.TryParse(lineSplit[firstLineNextId], out nextId) == false)
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
                            if(currentLine == foundOnLine)//if the line if the same line that needs to be replaced
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
                releaseWritingQueueNumber(filePath, writerQueueNumber);
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
    }
}