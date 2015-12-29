open System
open System.Collections.Generic
open System.ServiceModel
open Microsoft.FSharp.Data.TypeProviders

let binding isHttps : Channels.Binding =
    match isHttps with
    | true -> 
        let b = new BasicHttpsBinding()
        b.Security.Mode <- BasicHttpsSecurityMode.Transport
        b.Security.Transport.ClientCredentialType <- HttpClientCredentialType.Windows
        b :> Channels.Binding
    | false ->        
        let b = new BasicHttpBinding()
        b :> Channels.Binding

let configure (dataContext : ClientBase<'T>) address isHttps = 
    dataContext.Endpoint.Address <- new EndpointAddress(address)
    //do client.DataContext.ClientCredentials.UserName.UserName <- "Username"
    //do client.DataContext.ClientCredentials.UserName.Password <- "Password"
    dataContext.Endpoint.Binding <- binding isHttps
    
type Test = {name: string; execution: unit -> unit}
type TestResult = {name: string; success: bool; message: Option<string>}

let tryExecute (t:Test) = async{
    try
        t.execution()
        return {name=t.name; success=true; message=None}
    with
        | ex -> 
            return {name=t.name; success=false; message=Some(ex.ToString())}
    }

let tests = new List<Test>()

type Service1 = WsdlService<ServiceUri="https://localhost:4711/MT_1_v1.svc?wsdl">
let client1 = Service1.GetBasicHttpsBinding_ITimeService()
configure client1.DataContext "https://localhost:4711/MT_1_v1.svc" true
let c1 = {name="https"; execution=(fun()->client1.GetTime()|>ignore)}
tests.Add(c1)

type Service2 = WsdlService<ServiceUri="http://localhost:4710/MT_1_v1.svc?wsdl">
let client2 = Service2.GetBasicHttpBinding_ITimeService()
configure client2.DataContext "http://localhost:4710/MT_1_v12.svc" false
let c2 = {name="http"; execution=(fun()->client2.GetTime()|>ignore)}
tests.Add(c2)

let testResults = 
    tests
        |> Seq.map tryExecute
        |> Async.Parallel
        |> Async.RunSynchronously

testResults
    |> Seq.iter (
        fun (testResult) -> 
            let suc = if testResult.success then "Success" else "Fail" 
            Console.WriteLine(testResult.name + ":\t" + suc))