namespace sdpeval.tests

module Compression =
    open System    
    open F
    let logger = Common.Logging.LogManager.GetLogger("Compression")

    let unzipFile (zipFile, destinationFolderPath, logger:Common.Logging.ILog) =
        result{            
            logger.Debug(sprintf "Unzipping file: %s -> %s" zipFile destinationFolderPath)
            let! existingZipFilePath = F.ensureFileExistsWithMessage (sprintf "Zip file '%s' not found." (zipFile)) zipFile
            let! existingAndEmptyDestinationFolderPath = F.ensureDirectoryExistsAndIsEmptyWithMessage (sprintf "Cannot unzip '%s' to an allready existing folder '%s'." (zipFile) (destinationFolderPath)) destinationFolderPath true            
            use sevenZipExe = new EmbeddedResource.ExtractedEmbeddedResource("7za.exe",logger)
            let! sevenZipExeFilePath = sevenZipExe.FilePath
            let! existing7ZipExeFilePath = F.ensureFileExistsWithMessage (sprintf "%s not found." (sevenZipExeFilePath)) sevenZipExeFilePath
            let! sevenZipExitCode = sdpeval.tests.ProcessOperations.startConsoleProcess (existing7ZipExeFilePath, sprintf "x \"%s\"  -o\"%s\" -y" (existingZipFilePath) (existingAndEmptyDestinationFolderPath), existingAndEmptyDestinationFolderPath,-1,null,null,false)            
            logger.Debug(sprintf "Finished unzipping file: %A -> %A. ExitCode: %i" zipFile destinationFolderPath sevenZipExitCode)
            return sevenZipExitCode
        }
        
    let zipFolder (sourceFolderPath, zipFile, logger:Common.Logging.ILog) =
        result{
            logger.Debug(sprintf "Compressing folder: %A -> %A" sourceFolderPath zipFile)
            let! nonExistingZipFilePath = F.ensureFileDoesNotExistWithMessage (sprintf "Zip file allready exists: '%s'" (zipFile)) false zipFile
            let! existingSourceFolderPath = F.ensureDirectoryExistsWithMessage false (sprintf "Cannot zip down a non existing directory '%A'." sourceFolderPath) sourceFolderPath            
            use sevenZipExe = new EmbeddedResource.ExtractedEmbeddedResource("7za.exe",logger)
            let! sevenZipExeFilePath = sevenZipExe.FilePath
            let! existing7ZipExeFilePath = F.ensureFileExistsWithMessage (sprintf "%s not found." (sevenZipExeFilePath)) sevenZipExeFilePath
            let! sevenZipExitCode = ProcessOperations.startConsoleProcess (existing7ZipExeFilePath, sprintf "a \"%s\" \"%s\\*\"" (nonExistingZipFilePath)  (existingSourceFolderPath), existingSourceFolderPath,-1,null,null,false)
            logger.Debug(sprintf "Finished compressing folder: %A -> %A. ExitCode: %i" sourceFolderPath zipFile sevenZipExitCode)
            return sevenZipExitCode
        }