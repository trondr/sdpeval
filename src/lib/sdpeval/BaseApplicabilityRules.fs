namespace sdpeval

module BaseApplicabilityRules =
    
    type WmiQuery = {NameSpace:string;WqlQuery:string}
    type Processor = {Architecture:string;Level:string;Revision:string}
    type WindowsVersion = {Comparison:string;MajorVersion:string;MinorVersion:string;BuildNumber:string;ServicePackMajor:string;ServicePackMinor:string;AllSuitesMustBePresent:string;SuiteMask:string;ProductType:string}

