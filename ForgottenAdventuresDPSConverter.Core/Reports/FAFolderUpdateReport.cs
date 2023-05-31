﻿using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Reports
{
    public class FAFolderUpdateReport
    {
        private readonly List<int> added = new();
        private readonly List<string> failedToAdd = new();
        private readonly List<int> failedToUpdate = new();
        private readonly List<int> noLongerExisting = new();
        private readonly List<int> unaltered = new();
        private readonly List<int> updated = new();

        public List<int> Added { get => added; }
        public List<string> FailedToAdd { get => failedToAdd; }
        public List<int> FailedToUpdate { get => failedToUpdate; }
        public List<int> NoLongerExisting { get => noLongerExisting; }
        public List<int> Unaltered { get => unaltered; }
        public List <int> Updated { get => updated; }

        public string ToString(IRepository<FAFolder> repository)
        {
            const string nextLine = "\r\n";

            string returnString = string.Empty;

            if (FailedToAdd.Count > 0)
            {
                returnString += "failed to add:" + nextLine;
                foreach (string folder in FailedToAdd)
                {
                    returnString += folder + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders failed to add" + nextLine + nextLine;
            }

            if (FailedToUpdate.Count > 0)
            {
                returnString += "failed to update:" + nextLine;
                foreach (int folderId in FailedToUpdate)
                {
                    FAFolder folder = repository.Get(folderId).Result;
                    returnString += folder.Id.ToString().PadRight(5) + folder.RelativePath + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders failed to update" + nextLine + nextLine;
            }

            if (Added.Count > 0)
            {
                returnString += "added folders:" + nextLine;
                foreach (int folderId in Added)
                {
                    FAFolder folder = repository.Get(folderId).Result;
                    returnString += folder.Id.ToString().PadRight(5) + folder.RelativePath + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders added" + nextLine + nextLine;
            }

            if (Updated.Count > 0)
            {
                returnString += "updated folders:" + nextLine;
                foreach (int folderId in Updated)
                {
                    FAFolder folder = repository.Get(folderId).Result;
                    returnString += folder.Id.ToString().PadRight(5) + folder.RelativePath + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders updated" + nextLine + nextLine;
            }

            if (Unaltered.Count > 0)
            {
                returnString += "unaltered folders:" + nextLine;
                foreach (int folderId in Unaltered)
                {
                    FAFolder folder = repository.Get(folderId).Result;
                    returnString += folder.Id.ToString().PadRight(5) + folder.RelativePath + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders unaltered" + nextLine + nextLine;
            }

            if (NoLongerExisting.Count > 0)
            {
                returnString += "folder that no longer exist:" + nextLine;
                foreach (int folderId in NoLongerExisting)
                {
                    FAFolder folder = repository.Get(folderId).Result;
                    returnString += folder.Id.ToString().PadRight(5) + folder.RelativePath + nextLine;
                }
                returnString += nextLine;
            }
            else
            {
                returnString += "no folders are no longer represented" + nextLine + nextLine;
            }

            return returnString;
        }
    }
}
