namespace sdpeval

module sdp =
    
    open Microsoft.UpdateServices.Administration
        
    /// <summary>
    /// Load SoftwareDistributionPackage.
    /// </summary>
    /// <param name="sdpXmlFile"></param>
    let loadsdp (sdpXmlFile:string) =         
        let softwareDistributionPackage = new SoftwareDistributionPackage(sdpXmlFile)
        softwareDistributionPackage        

    type WmiQuery = {NameSpace:string;WqlQuery:string}

    type ApplicabilityRule =
        |True
        |False
        |And of ApplicabilityRule seq
        |Or of ApplicabilityRule seq
        |Not of ApplicabilityRule
        |WmiQuery of WmiQuery

    open System
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

    open System.Xml.Linq
    open System.Xml

    let rec sdpXmlToApplicabilityRules (applicabilityXml:string) =
        let nameTable = new NameTable()
        let namespaceManager = new XmlNamespaceManager(nameTable);
        namespaceManager.AddNamespace("lar","http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/LogicalApplicabilityRules.xsd")
        namespaceManager.AddNamespace("bar","http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/BaseApplicabilityRules.xsd")
        let xmlParserContext = XmlParserContext(null,namespaceManager,null,XmlSpace.None)
        use xmlReader = new XmlTextReader(applicabilityXml,XmlNodeType.Element,xmlParserContext)
        let xElement = XElement.Load(xmlReader)
        match xElement.Name.LocalName with
        |"True" -> True
        |"False" -> False
        |"WmiQuery" -> WmiQuery{NameSpace="";WqlQuery=""}
        |"And" -> 
            And (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Or" -> 
            Or (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Not" -> 
            Not (sdpXmlToApplicabilityRules (xElement.FirstNode.ToString()))
        |_ -> False

    let rec evaluateApplicabilityRule applicabilityRule =
        match applicabilityRule with
        |True -> true
        |False -> false
        |And al -> 
            al |> Seq.forall evaluateApplicabilityRule
        |Or al -> 
            al |> Seq.exists evaluateApplicabilityRule
        |Not al -> 
            not (evaluateApplicabilityRule al)
        |WmiQuery wq -> (wmiQueryIsMatch wq.NameSpace wq.WqlQuery)
    