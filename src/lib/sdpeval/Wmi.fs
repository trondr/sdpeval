
namespace sdpeval

module internal Wmi =

    open System.Management

    type Wql = {Namespace:string;Query:string}

    let wmiQueryIsMatch (wql:Wql) = 
            let managementPath = new ManagementPath(wql.Namespace)
            let scope = new ManagementScope(managementPath)            
            let query = new ObjectQuery(wql.Query)
            use searcher = new ManagementObjectSearcher(scope, query)
            use managementObjectCollection = searcher.Get()
            if(managementObjectCollection.Count = 0) then                
                false
            else
                true

    let wmiQueryIsMatchMemoized =
        F.memoize wmiQueryIsMatch