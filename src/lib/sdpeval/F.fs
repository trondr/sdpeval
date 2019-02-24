namespace sdpeval

module F =
    
    let getFiles searchPattern folder =
        System.IO.Directory.GetFiles(folder, searchPattern)
    
    let fileExists filePath =
        System.IO.File.Exists(filePath)

