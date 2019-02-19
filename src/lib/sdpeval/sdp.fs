namespace sdpeval

module sdp =
    
    open sdpeval.charp
    open System.Xml.Serialization
    open System.IO
    open Microsoft.UpdateServices.Administration

    let loadsdp (sdpXmlFile:string) =         
        let serializer = new XmlSerializer(typeof<SoftwareDistributionPackage>)
        use sr = new StreamReader(sdpXmlFile)
        let softwareDistributionPackage = serializer.Deserialize(sr) :?> SoftwareDistributionPackage
        softwareDistributionPackage        
