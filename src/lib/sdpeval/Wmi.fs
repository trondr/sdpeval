
namespace sdpeval

module internal Wmi =

    open System.Management

    type Wql = {Namespace:string;Query:string}

    let wmiQueryIsMatch (logger:Common.Logging.ILog) (wql:Wql) = 
            let managementPath = new ManagementPath(wql.Namespace)
            let scope = new ManagementScope(managementPath)            
            let query = new ObjectQuery(wql.Query)
            use searcher = new ManagementObjectSearcher(scope, query)
            match logger.IsDebugEnabled with true->logger.Debug(sprintf "Executing Wmi Query (NameSpace: \"%s\")(Query: \"%s\")." wql.Namespace wql.Query)|false -> ()
            use managementObjectCollection = searcher.Get()
            if(managementObjectCollection.Count = 0) then                
                false
            else
                true

    let wmiQueryIsMatchMemoized logger =
        F.memoize (wmiQueryIsMatch logger)