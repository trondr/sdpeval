namespace sdpeval

module BaseApplicabilityRules =
    
    type WmiQuery = {NameSpace:string;WqlQuery:string}
    type Processor = {Architecture:string;Level:string;Revision:string}
    type WindowsVersion = {Comparison:string;MajorVersion:string option;MinorVersion:string option;BuildNumber:string option;ServicePackMajor:string option;ServicePackMinor:string option;AllSuitesMustBePresent:string option;SuiteMask:string option;ProductType:string option}

