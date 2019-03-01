namespace sdpeval

module internal BaseTypes=
    open System

    let toUInt16 (value:string) =
        System.Convert.ToUInt16(value)

    let toUInt32 (value:string) =
        System.Convert.ToUInt32(value)
        
    let toInt16 (value:string) =
        System.Convert.ToInt16(value)

    let toInt32 (value:string) =
        System.Convert.ToInt32(value)

    let toBoolean (value:string) =
        System.Convert.ToBoolean(value)

    let toByte (value:string) =
        System.Convert.ToByte(value)

    type ScalarComparison =
        |LessThan
        |LessThanOrEqualTo
        |EqualTo
        |GreaterThanOrEqualTo
        |GreaterThan
    
    let toScalarComparison comparison =
        match comparison with
        |"LessThan" -> ScalarComparison.LessThan
        |"LessThanOrEqualTo" -> ScalarComparison.LessThanOrEqualTo
        |"EqualTo" -> ScalarComparison.EqualTo
        |"GreaterThanOrEqualTo" -> ScalarComparison.GreaterThanOrEqualTo
        |"GreaterThan" -> ScalarComparison.GreaterThan
        |_ -> raise(new Exception(sprintf "Unknow scalar comparison operator: %A" comparison))

    let toScalarComparisonString comparison =
        match comparison with
        |ScalarComparison.LessThan -> "LessThan"
        |ScalarComparison.LessThanOrEqualTo -> "LessThanOrEqualTo"
        |ScalarComparison.EqualTo -> "EqualTo"
        |ScalarComparison.GreaterThanOrEqualTo -> "GreaterThanOrEqualTo"
        |ScalarComparison.GreaterThan -> "GreaterThan"

    let compareScalar compareToken value1 value2 = 
        match compareToken with
        |LessThan -> 
            value1 < value2
        |LessThanOrEqualTo -> 
            value1 <= value2
        |EqualTo -> 
            value1 = value2
        |GreaterThanOrEqualTo -> 
            value1 >= value2
        |GreaterThan -> 
            value1 > value2

    type StringComparison =
        |EqualTo
        |BeginsWith
        |Contains
        |EndsWith

    let toStringComparison comparison =
        match comparison with
        |"EqualTo" -> StringComparison.EqualTo
        |"BeginsWith" -> StringComparison.BeginsWith
        |"Contains" -> StringComparison.Contains
        |"EndsWith" -> StringComparison.EndsWith        
        |_ -> raise(new Exception("Unknow string comparison operator: " + comparison))

    let toStringComparisonString comparison =
        match comparison with
        |StringComparison.EqualTo -> "EqualTo"
        |StringComparison.BeginsWith -> "BeginsWith"        
        |StringComparison.Contains -> "Contains"
        |StringComparison.EndsWith -> "EndsWith"

    let compareString compareToken (string1:string) (string2:string) =
        match compareToken with
        |EqualTo -> 
            string1 = string2
        |BeginsWith -> 
            string1.StartsWith(string2)
        |Contains -> 
            string1.Contains(string2)
        |EndsWith -> 
            string1.EndsWith(string2)
