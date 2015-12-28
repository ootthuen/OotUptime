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

let tryExecute m s = async{
    try
        m() |> ignore
        Console.WriteLine(s + "\tSUCCESS")
    with
        | ex -> 
            Console.WriteLine(s + "\tFAIL")
    }

let tests = new List<Async<unit>>()

type Service1 = WsdlService<ServiceUri="https://localhost:4711/MT_1_v1.svc?wsdl">    
let client1 = Service1.GetBasicHttpsBinding_ITimeService()
configure client1.DataContext "https://localhost:4711/MT_1_v1.svc" true
let c1 = tryExecute client1.GetTime "https"
tests.Add(c1)

type Service2 = WsdlService<ServiceUri="http://localhost:4710/MT_1_v1.svc?wsdl">    
let client2 = Service2.GetBasicHttpBinding_ITimeService()
configure client2.DataContext "http://localhost:4710/MT_1_v1.svc" false
let c2 = tryExecute client2.GetTime "http"
tests.Add(c2)

tests
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore