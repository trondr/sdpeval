namespace sdpeval

module sdp =
    
    open Microsoft.UpdateServices.Administration
    open System.Reflection
        
    /// <summary>
    /// Load SoftwareDistributionPackage.
    /// </summary>
    /// <param name="sdpXmlFile"></param>
    let loadsdp (sdpXmlFile:string) =         
        let softwareDistributionPackage = new SoftwareDistributionPackage(sdpXmlFile)
        softwareDistributionPackage        

    type WmiQuery = {NameSpace:string;WqlQuery:string}

    type Processor = {Architecture:string;Level:string;Revision:string}

    type ApplicabilityRule =
        |True
        |False
        |And of ApplicabilityRule seq
        |Or of ApplicabilityRule seq
        |Not of ApplicabilityRule
        |WmiQuery of WmiQuery
        |Processor of Processor

    open System
    open sdpeval.Wmi 
    open sdpeval.SystemInfo
    
    open System.Xml.Linq
    open System.Xml

    let getAttribute (xElement:XElement) (attributeName:string) defaultValue =
        let attributeValue = xElement.Attribute(XName.Get(attributeName))
        if(attributeValue = null) then
            defaultValue()
        else
            attributeValue.Value
    
    let toUInt16 (value:string) =
        System.Convert.ToUInt16(value)

    let toInt16 (value:string) =
        System.Convert.ToInt16(value)

    let toProcessor xElement =
        let architecture = (getAttribute xElement "Architecture" (fun _ -> null))
        let level = (getAttribute xElement "Level" (fun _ -> null))
        let revision = (getAttribute xElement "Revision" (fun _ -> null))                
        Processor {Architecture=architecture;Level=level;Revision=revision}
            
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
        |"WmiQuery" -> WmiQuery{NameSpace=(getAttribute xElement "Namespace" (fun _ -> ""));WqlQuery=(getAttribute xElement "WqlQuery" (fun _ -> ""))}
        |"Processor" -> (toProcessor xElement)
        |"And" -> 
            And (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Or" -> 
            Or (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Not" -> 
            Not (sdpXmlToApplicabilityRules ((xElement.Descendants()|>Seq.head).ToString()))
        |_ -> raise (new NotSupportedException(sprintf "Applicability rule for '%s' is not implemented." xElement.Name.LocalName))

    let isProcessor architecture level revision =
        
        let all = [|architecture;level;revision|]
        let allIsNull = all|>Array.forall(fun i-> (i = null))

        if(allIsNull) then
            raise (new Exception("Invalid Processor definition in SDP.xml. At least one of the attributes must be set: Architecture,Level,Revision"))
                
        let isArchitecture =
            if(architecture = null) then
                true
            else
                (toUInt16 architecture) = processorArchitecture

        let isLevel =
            if(level = null) then
                true
            else
                (toInt16 level) = processorLevel

        let isRevision =
            if(revision = null) then
                true
            else
                (toInt16 revision) = processorRevision
        
        (isArchitecture && isLevel && isRevision)

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
        |Processor p -> (isProcessor p.Architecture p.Level p.Revision)
    