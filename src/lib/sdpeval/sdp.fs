namespace sdpeval

module sdp =
    
    open Microsoft.UpdateServices.Administration    
    open System
    open BaseTypes
    open BaseApplicabilityRules
        
    /// <summary>
    /// Load SoftwareDistributionPackage.
    /// </summary>
    /// <param name="sdpXmlFile"></param>
    let loadsdp (sdpXmlFile:string) =         
        let softwareDistributionPackage = new SoftwareDistributionPackage(sdpXmlFile)
        softwareDistributionPackage        

    
    type ApplicabilityRule =
        |True
        |False
        |And of ApplicabilityRule seq
        |Or of ApplicabilityRule seq
        |Not of ApplicabilityRule
        |WmiQuery of WmiQuery
        |Processor of Processor
        |WindowsVersion of WindowsVersion

    open sdpeval.Wmi 
    open sdpeval.SystemInfo
    open sdpeval.WindowsVersion
    open System.Xml.Linq
    open System.Xml

    let getAttribute (xElement:XElement) (attributeName:string) defaultValue =
        let attributeValue = xElement.Attribute(XName.Get(attributeName))
        if(attributeValue = null) then
            defaultValue()
        else
            attributeValue.Value
    
    let toProcessor xElement =
        let architecture = (getAttribute xElement "Architecture" (fun _ -> null))
        let level = (getAttribute xElement "Level" (fun _ -> null))
        let revision = (getAttribute xElement "Revision" (fun _ -> null))                
        Processor {Architecture=architecture;Level=level;Revision=revision}

    let toWindowsVersion xElement =
        let comparison =  (getAttribute xElement "Comparison" (fun _ -> "EqualTo"))
        let majorVersion =  (getAttribute xElement "MajorVersion" (fun _ -> null))
        let minorVersion =  (getAttribute xElement "MinorVersion" (fun _ -> null))
        let buildNumber =  (getAttribute xElement "BuildNumber" (fun _ -> null))
        let servicePackMajor =  (getAttribute xElement "ServicePackMajor" (fun _ -> null))
        let servicePackMinor =  (getAttribute xElement "ServicePackMinor" (fun _ -> null))
        let allSuitesMustBePresent =  (getAttribute xElement "AllSuitesMustBePresent" (fun _ -> "false"))
        let suiteMask =  (getAttribute xElement "SuiteMask" (fun _ -> null))
        let productType =  (getAttribute xElement "ProductType" (fun _ -> null))
        WindowsVersion {Comparison=comparison;MajorVersion=majorVersion;MinorVersion=minorVersion;BuildNumber=buildNumber;ServicePackMajor=servicePackMajor;ServicePackMinor=servicePackMinor;AllSuitesMustBePresent=allSuitesMustBePresent;SuiteMask=suiteMask;ProductType=productType}
     
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
        |"WindowsVersion" -> (toWindowsVersion xElement)
        |"And" -> 
            And (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Or" -> 
            Or (xElement.Descendants()
            |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString()))))
        |"Not" -> 
            Not (sdpXmlToApplicabilityRules ((xElement.Descendants()|>Seq.head).ToString()))
        |_ -> raise (new NotSupportedException(sprintf "Applicability rule for '%s' is not implemented." xElement.Name.LocalName))

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
        |WindowsVersion w -> (isWindowsVersion WindowsVersion.currentWindowsVersion w)
    