using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public interface IFileRepositorySettings
    {
        public string FAFolderRepositoryFilePath { get; }
        public string DpsFolderRepositoryFilePath { get; }
        public string DpsNumberRepositoryFilePath { get; }
        public string DpsSubfolderRepositoryFilePath { get; }

        public Repository<DpsFolder> DpsFolderRepository { get; } //todo: maybe change this. it's ugly but i don't know how to avoid it as DpsFolderRepository needs to call FAFolderRepository in DeleteReferences, but FAFolderRepository needs to call DpsFolderRepository in EntityMeetsCreateRequirements and especially EntityMeetsUpdateRequirements to make sure that FolderId matches and existing id.
        public Repository<DpsNumber> DpsNumberRepository { get; } //todo: maybe change this. it's ugly but i don't know how to avoid it as DpsNumberRepository needs to call FAFolderRepository in DeleteReferences, but FAFolderRepository needs to call DpsNumberRepository in EntityMeetsCreateRequirements and especially EntityMeetsUpdateRequirements to make sure that NumberId matches and existing id.
        public Repository<DpsSubfolder> DpsSubfolderRepository { get; } //todo: maybe change this. it's ugly but i don't know how to avoid it as DpsSubfolderRepository needs to call FAFolderRepository in DeleteReferences, but FAFolderRepository needs to call DpsSubfolderRepository in EntityMeetsCreateRequirements and especially EntityMeetsUpdateRequirements to make sure that SubfolderId matches and existing id.
        public Repository<FAFolder> FAFolderRepository { get; } //todo: maybe change this. it's ugly but i don't know how to avoid it as DpsSubfolderRepository needs to call FAFolderRepository in DeleteReferences, but FAFolderRepository needs to call DpsSubfolderRepository in EntityMeetsCreateRequirements and especially EntityMeetsUpdateRequirements to make sure that SubfolderId matches and existing id.
    }
}
