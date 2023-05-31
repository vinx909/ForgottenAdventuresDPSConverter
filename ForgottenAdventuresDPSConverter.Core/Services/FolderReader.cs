using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class FolderReader : IFAFolderReader
    {
        private IRepository<FAFolder> FAFolderRepository { get; }

        public FolderReader(IRepository<FAFolder> FAFolderRepository)
        {
            this.FAFolderRepository = FAFolderRepository;
        }

        public async Task<FAFolderUpdateReport> UpdateFolders(string folderPath)
        {
            Console.Write('.');//todo: remove with some other means of keeping track on if things keep happening

            if (Directory.Exists(folderPath) == false)
            {
                return null;
            }

            int folderPathStartLength = folderPath.Length; //saves the folderPath length to remove to split the relative folder path off.

            Dictionary<string, int> existingPathsAndIds = await GetDictionaryWithExistingPathsAndIds();

            FAFolderUpdateReport report = new();
            List<Task> updateTasks = new();

            updateTasks.Add(Task.Run(async () => await updateSubfolder(folderPath, null)));

            await Task.WhenAll(updateTasks);

            if(existingPathsAndIds.Count > 0)
            {
                foreach (int id in existingPathsAndIds.Values)
                {
                    report.NoLongerExisting.Add(id);
                }
            }

            return report;

            async Task updateSubfolder(string folderPath, int? parentId)
            {
                Console.Write('.');//todo: remove with some other means of keeping track on if things keep happening

                int pathLength = folderPath.Length + 1;

                foreach (string subfolderPath in Directory.GetDirectories(folderPath))
                {
                    string relativePath = subfolderPath.Remove(0, folderPathStartLength);

                    if (existingPathsAndIds.ContainsKey(relativePath))//if the relative path already exists check if it needs updating and if so update it. then remove that relative path from the dictionary so in the end it's left with only the directories that no longer exist.
                    {
                        FAFolder existingFolder = await FAFolderRepository.Get(existingPathsAndIds[relativePath]);
                        existingPathsAndIds.Remove(relativePath);
                        bool needsUpdating = false;

                        string name = subfolderPath.Remove(0, pathLength);
                        if (existingFolder.Name != name)
                        {
                            existingFolder.Name = name;
                            needsUpdating = true;
                        }

                        int? parentID = parentId;
                        if (existingFolder.ParentId != parentID)
                        {
                            existingFolder.ParentId = parentID;
                            needsUpdating = true;
                        }

                        bool hasItems = Directory.GetFiles(subfolderPath).Length > 0;
                        if(existingFolder.HasItems != hasItems)
                        {
                            existingFolder.HasItems = hasItems;
                            needsUpdating = true;
                        }

                        if(needsUpdating == true)//if the folder needs updating
                        {
                            if(await FAFolderRepository.Update(existingFolder))//try to update
                            {
                                report.Updated.Add(existingFolder.Id);//if updating succeeded add it to the updated list
                            }
                            else
                            {
                                report.FailedToUpdate.Add(existingFolder.Id);//if updating failed add the it ot the failed to update list
                            }
                        }
                        else
                        {
                            report.Unaltered.Add(existingFolder.Id);//if the folder didn't need updating instead add it to unaltered list
                        }

                        updateTasks.Add(Task.Run(() => updateSubfolder(subfolderPath, existingFolder.Id)));//continue updating all the folders subfolders
                    }
                    else//if the relative path doesn't already exists add it instead.
                    {
                        FAFolder newFolder = new()
                        {
                            Name = subfolderPath.Remove(0, pathLength),
                            RelativePath = relativePath,
                            ParentId = parentId,
                            HasItems = Directory.GetFiles(subfolderPath).Length > 0
                        };

                        (bool, int) createResult = await FAFolderRepository.Create(newFolder);

                        if(createResult.Item1 == true) //if the item was successfully added
                        {
                            report.Added.Add(newFolder.Id);//add the folder to the added list
                            updateTasks.Add(Task.Run(() => updateSubfolder(subfolderPath, createResult.Item2)));//continue updating all the folders subfolders
                        }
                        else //if the item wasn't successfully added
                        {
                            report.FailedToAdd.Add(newFolder.RelativePath);//add the folder to the failed to add list
                            //since continueing from this point with subfolders would at least result in folders with an incorrectly set parent no further updates are run.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// returns a dictionary with all the relative paths as key and ids as value of existing FAFolders.
        /// </summary>
        /// <returns>the dictionary</returns>
        private async Task<Dictionary<string, int>> GetDictionaryWithExistingPathsAndIds()
        {
            Dictionary<string, int> existingPathsAndIds = new(); //a dictionary with every relative folder path and id so not every subfolder needs to make a call if it's not needed.
            foreach (FAFolder folder in await FAFolderRepository.GetAll())
            {
                existingPathsAndIds.Add(folder.RelativePath, folder.Id);
            }
            return existingPathsAndIds;
        }


        #region probably legacy functions
        public List<FAFolder> GetFolders(string folderPath)
        {
            if (!Directory.Exists(folderPath)) //check if the folder exists
            {
                return null;
            }

            List<FAFolder> folders = new List<FAFolder>();
            folders.AddRange(GetSubfolders(folderPath, folderPath.Length, null));

            return folders;
        }

        private List<FAFolder> GetSubfolders(string folderPath, int origionalFolderPathLenth, int? parentID)
        {
            List<FAFolder> subfolders = new List<FAFolder>();

            int pathLength = folderPath.Length + 1;

            foreach (string subfolderPath in Directory.GetDirectories(folderPath))
            {
                FAFolder subfolder = new()
                {
                    Name = subfolderPath.Remove(0, pathLength),
                    RelativePath = subfolderPath.Remove(0, origionalFolderPathLenth),
                    ParentId = parentID,
                    HasItems = Directory.GetFiles(subfolderPath).Length > 0
                };
                if (FAFolderRepository.Create(subfolder).Result.Item1)
                {
                    Console.Out.Write(subfolder.Name + " yes - ");
                }
                else
                {
                    Console.Out.Write(subfolder.Name+" no - ");
                }
                subfolder = FAFolderRepository.GetAll(f => f.RelativePath.Equals(subfolder.RelativePath)).Result.First();
                subfolders.Add(subfolder);
                subfolders.AddRange(GetSubfolders(subfolderPath, origionalFolderPathLenth, subfolder.Id));
            }

            return subfolders;
        }

        public List<string> GetFoldersWithItems(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return null;
            }
            List<string> folderNames = new List<string>();
            folderNames.AddRange(Directory.GetDirectories(folderPath, "", SearchOption.AllDirectories));
            folderNames.Sort();
            //adds all subfolders to the list

            int pathLength = folderPath.Length + 1;
            for (int i = folderNames.Count - 1; i >= 0; i--)
            {
                if (Directory.GetFiles(folderNames[i]).Length == 0)
                {
                    folderNames.RemoveAt(i);
                    //checks if the folder contains any items. if it doesn't it removes the folder from the collection
                }
                else
                {
                    folderNames[i] = folderNames[i].Remove(0, pathLength);
                    //if the folder does contain items the path gets removed to avoid unsesisary space useage.
                }
            }
            return folderNames;
        }

        
        #endregion
    }
}
