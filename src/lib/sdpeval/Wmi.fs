
namespace sdpeval

module Wmi =

    open System.Management

    let wmiQueryIsMatch (nameSpace:string) (wqlQuery:string) :bool = 
            let managementPath = new ManagementPath(nameSpace)
            let scope = new ManagementScope(managementPath)            
            let query = new ObjectQuery(wqlQuery)
            use searcher = new ManagementObjectSearcher(scope,query)
            use managementObjectCollection = searcher.Get()
            if(managementObjectCollection.Count = 0) then                
                false
            else
                true