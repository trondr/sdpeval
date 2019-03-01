
namespace sdpeval

module internal Logging=
    
    open Common.Logging

    let getLoggerByName (name:string) = 
        LogManager.GetLogger(name)

