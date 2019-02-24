namespace sdpeval

module sdp =
    
    open Microsoft.UpdateServices.Administration    
    open System    
    open sdpeval.BaseApplicabilityRules
        
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
        |FileVersion of FileVersion
        |RegSz of RegSz

    open sdpeval.Wmi 
    open sdpeval.SystemInfo
    open sdpeval.WindowsVersion
    open System.Xml.Linq
    open System.Xml    
    open sdpeval.charp
    open System.ComponentModel

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
        ApplicabilityRule.Processor {Architecture=architecture;Level=level;Revision=revision}
    
    let toOption value =
        match value with
        |null -> None
        |v -> Some v
        
    let toWindowsVersion xElement =
        let comparison =  (getAttribute xElement "Comparison" (fun _ -> "EqualTo"))
        let majorVersion =  toOption (getAttribute xElement "MajorVersion" (fun _ -> null))
        let minorVersion =  toOption (getAttribute xElement "MinorVersion" (fun _ -> null))
        let buildNumber =  toOption (getAttribute xElement "BuildNumber" (fun _ -> null))
        let servicePackMajor =  toOption (getAttribute xElement "ServicePackMajor" (fun _ -> null))
        let servicePackMinor =  toOption (getAttribute xElement "ServicePackMinor" (fun _ -> null))
        let allSuitesMustBePresent =  toOption (getAttribute xElement "AllSuitesMustBePresent" (fun _ -> "false"))
        let suiteMask =  toOption (getAttribute xElement "SuiteMask" (fun _ -> null))
        let productType =  toOption (getAttribute xElement "ProductType" (fun _ -> null))
        ApplicabilityRule.WindowsVersion {Comparison=comparison;MajorVersion=majorVersion;MinorVersion=minorVersion;BuildNumber=buildNumber;ServicePackMajor=servicePackMajor;ServicePackMinor=servicePackMinor;AllSuitesMustBePresent=allSuitesMustBePresent;SuiteMask=suiteMask;ProductType=productType}
    
    let toFileVersion xElement = 
        let comparison =  (getAttribute xElement "Comparison" (fun _ -> raise (new Exception(sprintf "'Comparison' attribute not specified for FileVersion applicability rule: %A" xElement))))
        let csidl =  toOption (getAttribute xElement "Csidl" (fun _ -> null))
        let path =  (getAttribute xElement "Path" (fun _ -> raise (new Exception(sprintf "'Path' attribute not specified for FileVersion applicability rule: %A" xElement))))
        let version =  (getAttribute xElement "Version" (fun _ -> raise (new Exception(sprintf "'Version' attribute not specified for FileVersion applicability rule: %A" xElement))))
        ApplicabilityRule.FileVersion {Csidl=csidl;Path=path;Comparison=comparison;Version=version}

    let toRegSz xElement = 
        let comparison =  (getAttribute xElement "Comparison" (fun _ -> raise (new Exception(sprintf "'Comparison' attribute not specified for RegSz applicability rule: %A" xElement))))
        let key =  (getAttribute xElement "Key" (fun _ -> raise (new Exception(sprintf "'Key' attribute not specified for RegSz applicability rule: %A" xElement))))
        let subkey =  (getAttribute xElement "Subkey" (fun _ -> raise (new Exception(sprintf "'Subkey' attribute not specified for RegSz applicability rule: %A" xElement))))
        let value =  (getAttribute xElement "Value" (fun _ -> raise (new Exception(sprintf "'Value' attribute not specified for RegSz applicability rule: %A" xElement))))
        let data =  (getAttribute xElement "Data" (fun _ -> raise (new Exception(sprintf "'Data' attribute not specified for RegSz applicability rule: %A" xElement))))
        let regType32 =  toOption (getAttribute xElement "RegType32" (fun _ -> "false"))        
        ApplicabilityRule.RegSz {Comparison=comparison;Key=key;Subkey=subkey;RegType32=regType32;Value=value;Data=data}

    let rec sdpXmlToApplicabilityRules (applicabilityXml:string) :ApplicabilityRule =
        let nameTable = new NameTable()
        let namespaceManager = new XmlNamespaceManager(nameTable);
        namespaceManager.AddNamespace("lar","http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/LogicalApplicabilityRules.xsd")
        namespaceManager.AddNamespace("bar","http://schemas.microsoft.com/wsus/2005/04/CorporatePublishing/BaseApplicabilityRules.xsd")
        let xmlParserContext = XmlParserContext(null,namespaceManager,null,XmlSpace.None)
        use xmlReader = new XmlTextReader(applicabilityXml,XmlNodeType.Element,xmlParserContext)
        let xElement = XElement.Load(xmlReader)
        match xElement.Name.LocalName with
        |"True" -> ApplicabilityRule.True
        |"False" -> ApplicabilityRule.False
        |"WmiQuery" -> ApplicabilityRule.WmiQuery{NameSpace=(getAttribute xElement "Namespace" (fun _ -> ""));WqlQuery=(getAttribute xElement "WqlQuery" (fun _ -> ""))}
        |"Processor" -> (toProcessor xElement)
        |"WindowsVersion" -> (toWindowsVersion xElement)
        |"FileVersion" -> (toFileVersion xElement)
        //|"RegSz" -> (toRegSz xElement)
        |"And" -> 
            ApplicabilityRule.And (
                xElement.Elements()
                |>Seq.map (fun x -> (                                        
                                        sdpXmlToApplicabilityRules (x.ToString()))
                            )
                |>Seq.toArray
                )
        |"Or" -> 
            ApplicabilityRule.Or (    
                xElement.Elements()
                |>Seq.map (fun x -> (sdpXmlToApplicabilityRules (x.ToString())))
                |>Seq.toArray 
                )
        |"Not" -> 
            ApplicabilityRule.Not (sdpXmlToApplicabilityRules ((xElement.Descendants()|>Seq.head).ToString()))
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
        |FileVersion fv -> (sdpeval.FileVersion.isFileVersion fv)
        |RegSz r -> 
            raise (new NotImplementedException("RegSz"))            
    